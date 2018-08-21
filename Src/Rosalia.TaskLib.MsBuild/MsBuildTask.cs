using Rosalia.Core.Environment;

namespace Rosalia.TaskLib.MsBuild
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Win32;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.FileSystem;
    using Rosalia.TaskLib.Standard.Tasks;

    /// <summary>
    /// This task uses MSBuild.exe command line tool to run a build process.
    /// </summary>
    public class MsBuildTask : ExternalToolTask
    {
        public MsBuildTask()
        {
            Switches = new List<MsBuildSwitch>();
        }

        public IFile ProjectFile { get; set; }

        public MsBuildToolVersion ToolVersion { get; set; }

        public IList<MsBuildSwitch> Switches { get; set; }

        protected override string DefaultToolPath
        {
            get { return "msbuild"; }
        }

        protected override void FillMessageLevelDetectors(IList<Func<string, MessageLevel?>> detectors)
        {
            base.FillMessageLevelDetectors(detectors);

            detectors.Add(message => message.IndexOf(" error ") >= 0 ? MessageLevel.Error : (MessageLevel?)null);
            detectors.Add(message => message.IndexOf(" warning ") >= 0 ? MessageLevel.Warn : (MessageLevel?)null);
        }

        protected override string GetToolPath(TaskContext context)
        {
            if (context.Environment.IsMono)
            {
                return "xbuild";
            }

            return base.GetToolPath(context);
        }

        protected override IEnumerable<IFile> GetToolPathLookup(TaskContext context)
        {
            return 
                /*
                 * Fresh MsBuild is no longer in windows registry
                 * so hardcoded program files locations go first.
                 */
                GetToolPathLookupFromProgramFiles(context) 
                /*
                 * MSBuild version prior to v15 could be read from registry.
                 */
                .Concat(GetToolPathLookupFromRegistry(context));
        }

        protected virtual IEnumerable<IFile> GetToolPathLookupFromProgramFiles(TaskContext context)
        {
            if (ToolVersion == null || ToolVersion == MsBuildToolVersion.V150)
            {
                yield return context.Environment.ProgramFilesX86()/"Microsoft Visual Studio"/"2017"/"Community"/"MSBuild"/"15.0"/"Bin"/ "MSBuild.exe";
                yield return context.Environment.ProgramFiles()/"Microsoft Visual Studio"/"2017"/"Community"/"MSBuild"/"15.0"/"Bin"/ "MSBuild.exe";
                yield return context.Environment.ProgramFilesX86()/"Microsoft Visual Studio"/"2017"/"BuildTools"/"MSBuild"/"15.0"/"Bin"/ "MSBuild.exe";
                yield return context.Environment.ProgramFiles()/"Microsoft Visual Studio"/"2017"/ "BuildTools" / "MSBuild"/"15.0"/"Bin"/ "MSBuild.exe";
            }

            // Note: add newer MsBuild references here as they come up.
        }

        protected virtual IEnumerable<IFile> GetToolPathLookupFromRegistry(TaskContext context)
        {
            try
            {
                string[] registryKeys = ToolVersion == null ?
                    /*
                     * If no specific version provided we look up for 
                     * the most fresh version of MsBuild
                     */
                    LookUpMsBuildRegistryKeys()
                    : new[] { ToolVersion.Value };

                if (registryKeys.Length == 0)
                {
                    context.Log.Warning("No MsBuild registry entries have been found. MsBuild probably have not been installed.");
                }

                return registryKeys
                    .Select(key =>
                    {
                        string registryKey = string.Format(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\{0}", key);
                        string toolDirectory = (string)Registry.GetValue(registryKey, "MSBuildToolsPath", null);

                        return new DefaultFile(Path.Combine(toolDirectory, "MsBuild.exe"));
                    });
            }
            catch (Exception ex)
            {
                context.Log.Warning("Error occured while reading registry: {0}", ex.Message);
            }

            return new IFile[0];
        }

        private static string[] LookUpMsBuildRegistryKeys()
        {
            RegistryKey msBuildToolsVersions = Registry
                .LocalMachine
                .OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions");

            if (msBuildToolsVersions == null)
            {
                return new string[0];
            }

            return msBuildToolsVersions
                .GetSubKeyNames()
                .Select(key =>
                {
                    double version;
                    bool isParsed = double.TryParse(key, NumberStyles.Float, CultureInfo.InvariantCulture, out version);

                    return new
                    {
                        Key = key,
                        isParsed = isParsed,
                        Version = version
                    };
                })
                .Where(item => item.isParsed)
                .OrderByDescending(item => item.Version)
                .Select(item => item.Key)
                .ToArray();
        }

        protected override string GetToolArguments(TaskContext context)
        {
            string projectFile = GetProjectFile(context);
            string switches = string.Join(" ", Switches.Select(s => s.CommandLinePart));

            return string.Format("{0} {1}", switches, projectFile);
        }

        protected virtual string GetProjectFile(TaskContext context)
        {
            if (ProjectFile != null)
            {
                return ProjectFile.AbsolutePath;
            }

            context.Log.Info(
@"No project file defined. Start solution file lookup from directory 
{0}",
                context.WorkDirectory.AbsolutePath);

            var currentDirectory = context.WorkDirectory;
            while (currentDirectory != null)
            {
                var solutionFiles = currentDirectory.Files.IncludeByExtension("sln").ToArray();
                if (solutionFiles.Length > 0)
                {
                    if (solutionFiles.Length > 1)
                    {
                        context.Log.Info(
@"Multiple solution files found in directory 
{0}
The first file will be used.",
                             currentDirectory.AbsolutePath);
                    }

                    var solutionFile = solutionFiles.First().AbsolutePath;

                    context.Log.Info("Solution file found: {0}", solutionFile);

                    return solutionFile;
                }

                context.Log.Info(
@"No solution files found in 
{0}
Go to parent directory.",
                        currentDirectory.AbsolutePath);
                currentDirectory = currentDirectory.Parent;
            }

            context.Log.Warning("No solution file found. Default MSBuild lookup mechanism will be used.");

            return null;
        }
    }
}
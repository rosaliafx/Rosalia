namespace Rosalia.TaskLib.MsBuild
{
    using System;
    using System.Collections.Generic;
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
            ToolVersion = MsBuildToolVersion.V40;
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

        protected override IEnumerable<IFile> GetToolPathLookup(TaskContext context)
        {
            try
            {
                var toolDirectory = (string)Registry.GetValue(string.Format(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\{0}", ToolVersion.Value), "MSBuildToolsPath", null);
                return new[] { new DefaultFile(Path.Combine(toolDirectory, "MsBuild.exe")) };
            }
            catch (Exception ex)
            {
                context.Log.Warning("Error occured while reading registry: {0}", ex.Message);
            }

            return new IFile[0];
        }

        protected override string GetToolArguments(TaskContext context)
        {
            var projectFile = GetProjectFile(context);
            var switches = string.Join(" ", Switches.Select(s => s.CommandLinePart));
            
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
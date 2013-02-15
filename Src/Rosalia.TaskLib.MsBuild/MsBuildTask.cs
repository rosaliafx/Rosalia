namespace Rosalia.TaskLib.MsBuild
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Win32;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.Standard.Tasks;

    /// <summary>
    /// This task uses MSBuild.exe command line tool to run a build process.
    /// </summary>
    public class MsBuildTask<T> : ExternalToolTask<T, MsBuildInput, object>
    {
        public MsBuildTask() : this(new MsBuildInput())
        {
        }

        public MsBuildTask(MsBuildInput input) : base(input)
        {
        }

        public override ExtendedTask<T, MsBuildInput, object> ApplyResult(Action<object, T> applyResultFunc)
        {
            throw new InvalidOperationException();
        }

        protected override string DefaultToolPath
        {
            get { return "msbuild"; }
        }

        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }

        protected override void FillMessageLevelDetectors(IList<Func<string, MessageLevel?>> detectors)
        {
            base.FillMessageLevelDetectors(detectors);

            detectors.Add(message => message.IndexOf(" error ") >= 0 ? MessageLevel.Error : (MessageLevel?)null);
            detectors.Add(message => message.IndexOf(" warning ") >= 0 ? MessageLevel.Warning : (MessageLevel?)null);
        }

        protected override IEnumerable<IFile> GetToolPathLookup(TaskContext<T> context, MsBuildInput input, ResultBuilder result)
        {
            try
            {
                var toolDirectory = (string)Registry.GetValue(string.Format(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\{0}", input.ToolVersion.Value), "MSBuildToolsPath", null);
                return new[] { new DefaultFile(Path.Combine(toolDirectory, "MsBuild.exe")) };
            }
            catch (Exception ex)
            {
                result.AddWarning("Error occured while reading registry: {0}", ex.Message);
            }

            return new IFile[0];
        }

        protected override string GetToolArguments(MsBuildInput input, TaskContext<T> context, ResultBuilder result)
        {
            var projectFile = GetProjectFile(input, context, result);
            var switches = string.Join(" ", input.Switches.Select(s => s.CommandLinePart));
            
            return string.Format("{0} {1}", switches, projectFile);
        }

        protected virtual string GetProjectFile(MsBuildInput input, TaskContext<T> context, ResultBuilder result)
        {
            if (input.ProjectFile != null)
            {
                return input.ProjectFile.AbsolutePath;
            }

            result.AddInfo(
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
                        result.AddInfo(
@"Multiple solution files found in directory 
{0}
The first file will be used.", 
                             currentDirectory.AbsolutePath);
                    }

                    var solutionFile = solutionFiles.First().AbsolutePath;

                    result.AddInfo("Solution file found: {0}", solutionFile);

                    return solutionFile;
                }

                result.AddInfo(
@"No solution files found in 
{0}
Go to parent directory.", 
                        currentDirectory.AbsolutePath);
                currentDirectory = currentDirectory.Parent;
            }

            result.AddWarning("No solution file found. Default MSBuild lookup mechanism will be used.");

            return null;
        }
    }
}
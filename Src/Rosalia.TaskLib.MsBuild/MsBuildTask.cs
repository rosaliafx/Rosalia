namespace Rosalia.TaskLib.MsBuild
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Win32;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.Standard;

    /// <summary>
    /// This task uses MSBuild.exe command line tool to run a build process.
    /// </summary>
    public class MsBuildTask<T> : ExternalToolTask<T, MsBuildInput, object>
    {
        public override ExtendedTask<T, MsBuildInput, object> ApplyResult(Action<object, T> applyResultFunc)
        {
            throw new InvalidOperationException();
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

        protected override string GetToolPath(MsBuildInput input, TaskContext<T> context)
        {
            if (!string.IsNullOrEmpty(input.ExactMsBuildExeLocation))
            {
                return input.ExactMsBuildExeLocation;
            }

            var toolDirectory = (string)Registry.GetValue(string.Format(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\{0}", input.ToolVersion.Value), "MSBuildToolsPath", null);
            return Path.Combine(toolDirectory, "MsBuild.exe");
        }

        protected override string GetToolArguments(MsBuildInput input, TaskContext<T> context)
        {
            return string.Join(" ", input.Switches.Select(s => s.CommandLinePart)) + " " + input.ProjectFile.AbsolutePath;
        }
    }
}
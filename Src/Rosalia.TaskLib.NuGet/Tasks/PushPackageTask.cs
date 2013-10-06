namespace Rosalia.TaskLib.NuGet.Tasks
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.NuGet.Input;
    using Rosalia.TaskLib.Standard.Tasks;

    /// <summary>
    /// NuGet push task input. See <see cref="http://docs.nuget.org/docs/reference/command-line-reference#Push_Command"/>
    /// for options details.
    /// </summary>
    public class PushPackageTask<T> : NoResultExternalToolTask<T>
    {
        public PushPackageTask()
        {
            Options = new List<Option>();
        }

        /// <summary>
        /// Gets or sets a package file to push.
        /// </summary>
        public IFile PackageFile { get; set; }

        /// <summary>
        /// Gets or sets your personal NuGet API key. Set this property for internal workflows only!
        /// For public workflows set the key before executing this task:
        /// <c>nuget setapikey API_KEY [options]</c>
        /// <see cref="http://docs.nuget.org/docs/reference/command-line-reference#Setapikey_Command"/>
        /// </summary>
        public string ApiKey { get; set; }

        public IList<Option> Options { get; set; }

        public PushPackageTask<T> Package(IFile file)
        {
            PackageFile = file;
            return this;
        }

        public PushPackageTask<T> WithSource(string source)
        {
            Options.Add(new Option("Source", source));
            return this;
        }

        public PushPackageTask<T> WithVerbosityLevel(string level)
        {
            Options.Add(new Option("Verbosity", level));
            return this;
        }

        public PushPackageTask<T> WithVerbosityNormal()
        {
            return WithVerbosityLevel("normal");
        }

        public PushPackageTask<T> WithVerbosityQuiet()
        {
            return WithVerbosityLevel("quiet");
        }

        public PushPackageTask<T> WithVerbosityDetailed()
        {
            return WithVerbosityLevel("detailed");
        }

        /// <summary>
        /// Specifies the timeout for pushing to a server in seconds. Defaults to 300 seconds (5 minutes).
        /// </summary>
        public PushPackageTask<T> WithTimeout(int timeout)
        {
            Options.Add(new Option("Timeout", timeout.ToString(CultureInfo.InvariantCulture)));
            return this;
        }

        public PushPackageTask<T> WithApiKey(string apiKey)
        {
            ApiKey = apiKey;
            return this;
        }

        public void AddUniqueOption(Option option)
        {
            var optionWithSameName = Options.FirstOrDefault(x => x.Name == option.Name);
            if (optionWithSameName != null)
            {
                Options.Remove(optionWithSameName);
            }

            Options.Add(optionWithSameName);
        }

        protected override string GetToolPath(TaskContext<T> context, ResultBuilder result)
        {
            return "nuget";
        }

        protected override string GetToolArguments(TaskContext<T> context, ResultBuilder result)
        {
            return string.Format(
                "push \"{1}\" {0} {2} -NonInteractive", 
                ApiKey,
                PackageFile.AbsolutePath, 
                string.Join(" ", Options.Select(option => option.CommandLinePart)));
        }
    }
}
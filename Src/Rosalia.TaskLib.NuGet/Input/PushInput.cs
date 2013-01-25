namespace Rosalia.TaskLib.NuGet.Input
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Rosalia.Core.FileSystem;

    /// <summary>
    /// NuGet push task input. See <see cref="http://docs.nuget.org/docs/reference/command-line-reference#Push_Command"/>
    /// for options details.
    /// </summary>
    public class PushInput
    {
        public PushInput()
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

        public PushInput Package(IFile file)
        {
            PackageFile = file;
            return this;
        }

        public PushInput WithSource(string source)
        {
            Options.Add(new Option("Source", source));
            return this;
        }

        public PushInput WithVerbosityLevel(string level)
        {
            Options.Add(new Option("Verbosity", level));
            return this;
        }

        public PushInput WithVerbosityNormal()
        {
            return WithVerbosityLevel("normal");
        }

        public PushInput WithVerbosityQuiet()
        {
            return WithVerbosityLevel("quiet");
        }

        public PushInput WithVerbosityDetailed()
        {
            return WithVerbosityLevel("detailed");
        }

        /// <summary>
        /// Specifies the timeout for pushing to a server in seconds. Defaults to 300 seconds (5 minutes).
        /// </summary>
        public PushInput WithTimeout(int timeout)
        {
            Options.Add(new Option("Timeout", timeout.ToString(CultureInfo.InvariantCulture)));
            return this;
        }

        public PushInput WithApiKey(string apiKey)
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
    }
}
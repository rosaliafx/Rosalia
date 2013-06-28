namespace Rosalia.Runner.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OptionsParser
    {
        private const string OptionPrefix = "/";

        public void Parse(string[] args, OptionsConfig options)
        {
            foreach (var arg in args)
            {
                if (IsOption(arg))
                {
                    ProcessArg(arg, options);
                }
            }
        }

        private void ProcessArg(string arg, OptionsConfig options)
        {
            arg = arg.Substring(OptionPrefix.Length);
            var separatorIndex = arg.IndexOf("=", StringComparison.InvariantCultureIgnoreCase);

            var optionName = arg;
            var optionSuffix = (string) null;
            var optionValue = (string) null;

            if (separatorIndex >= 0)
            {
                optionName = arg.Substring(0, separatorIndex);
                optionValue = arg.Substring(separatorIndex + 1);
            }

            var nameParts = optionName.Split(':');
            if (nameParts.Length > 1)
            {
                optionName = nameParts[0];
                optionSuffix = nameParts[1];
            }

            foreach (var option in options.Keys)
            {
                if (option.Aliases.Any(alias => alias.Equals(optionName)))
                {
                    options[option].Invoke(optionValue, optionSuffix);
                }
            }
        }

        private bool IsOption(string arg)
        {
            return arg.StartsWith(OptionPrefix);
        }
    }
}
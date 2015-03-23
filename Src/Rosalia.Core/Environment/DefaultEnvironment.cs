namespace Rosalia.Core.Environment
{
    using System;

    public class DefaultEnvironment : IEnvironment
    {
        public string this[string key]
        {
            get { return Environment.GetEnvironmentVariable(key); }
            set { Environment.SetEnvironmentVariable(key, value); }
        }

        public bool IsMono
        {
            get { return Type.GetType("Mono.Runtime") != null; }
        }
    }
}
namespace Rosalia.Core.Tests.Stubs
{
    using System.Collections.Generic;
    using Rosalia.Core.Context.Environment;
    using Rosalia.Core.FileSystem;

    public class EnvironmentStub : IEnvironment
    {
        public EnvironmentStub()
        {
            FakeVariables = new Dictionary<string, string>();
        }

        public IDictionary<string, string> FakeVariables { get; set; }

        public IDirectory ProgramFilesX86 { get; set; }

        public IDirectory ProgramFiles { get; set; }

        public string GetVariable(string variable)
        {
            if (FakeVariables.ContainsKey(variable))
            {
                return FakeVariables[variable];    
            }

            return null;
        }

        public void SetVariable(string variable, string value)
        {
            FakeVariables[value] = value;
        }
    }
}
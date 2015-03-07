namespace Rosalia.Core.Environment
{
    public class DefaultEnvironment : IEnvironment
    {
        public string this[string key]
        {
            get { return System.Environment.GetEnvironmentVariable(key); }
            set { System.Environment.SetEnvironmentVariable(key, value); }
        }
    }
}
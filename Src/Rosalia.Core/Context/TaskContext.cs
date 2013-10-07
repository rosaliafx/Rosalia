namespace Rosalia.Core.Context
{
    using Rosalia.Core.Context.Environment;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;

    /// <summary>
    /// Task execution context.
    /// </summary>
    public class TaskContext
    {
        public TaskContext(
            IExecuter executer, 
            IDirectory workDirectory, 
            IEnvironment environment)
        {
            Environment = environment;
            WorkDirectory = workDirectory;
            //Data = data;
            Executer = executer;
            FileSystem = new FileSystemHelper();
        }

        //public T Data { get; private set; }

        public IExecuter Executer { get; private set; }

        public IDirectory WorkDirectory { get; private set; }

        public FileSystemHelper FileSystem { get; private set; }

        public IEnvironment Environment { get; private set; }
    }
}
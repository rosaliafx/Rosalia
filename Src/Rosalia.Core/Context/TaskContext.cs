namespace Rosalia.Core.Context
{
    using Rosalia.Core.Context.Environment;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;

    /// <summary>
    /// Task execution context.
    /// </summary>
    public class TaskContext<T>
    {
        public TaskContext(
            T data, 
            IExecuter<T> executer, 
            ILogger logger, 
            IDirectory workDirectory, IEnvironment environment)
        {
            Environment = environment;
            Logger = logger;
            WorkDirectory = workDirectory;
            Data = data;
            Executer = executer;
            FileSystem = new FileSystemHelper();
        }

        public T Data { get; private set; }

        public IExecuter<T> Executer { get; private set; }

        public ILogger Logger { get; private set; }

        public IDirectory WorkDirectory { get; private set; }

        public FileSystemHelper FileSystem { get; private set; }

        public IEnvironment Environment { get; private set; }
    }
}
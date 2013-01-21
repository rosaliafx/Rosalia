namespace Rosalia.Core
{
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;

    public class ExecutionContext<T>
    {
        public ExecutionContext(T data, IExecuter<T> executer, ILogger logger, IDirectory workDirectory)
        {
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
    }
}
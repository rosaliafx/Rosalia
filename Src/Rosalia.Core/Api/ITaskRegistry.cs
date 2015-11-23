namespace Rosalia.Core.Api
{
    using Rosalia.Core.Environment;
    using Rosalia.FileSystem;

    public interface ITaskRegistry<out T>
    {
        /// <summary>
        /// Gets work directory
        /// </summary>
        IDirectory WorkDirectory { set; }

        /// <summary>
        /// Gets environment helper
        /// </summary>
        IEnvironment Environment { set; }

        RegisteredTasks GetRegisteredTasks();
    }
}
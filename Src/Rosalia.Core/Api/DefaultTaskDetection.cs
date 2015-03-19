namespace Rosalia.Core.Api
{
    /// <summary>
    /// Determines how the default tasks should be defined if no explicit 
    /// behaviors were assigned.
    /// </summary>
    public enum DefaultTaskDetection
    {
        /// <summary>
        /// The very bottom task considered to be default.
        /// </summary>
        LastTask,

        /// <summary>
        /// All the tasks considered to be default.
        /// </summary>
        AllTasks
    }
}
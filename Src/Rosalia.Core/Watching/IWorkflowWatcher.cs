namespace Rosalia.Core.Watching
{
    public interface IWorkflowWatcher
    {
        void Register(IWorkflowEventsAware eventsAware);
    }
}
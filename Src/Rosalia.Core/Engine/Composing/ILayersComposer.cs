namespace Rosalia.Core.Engine.Composing
{
    using Rosalia.Core.Api;

    public interface ILayersComposer
    {
        Layer[] Compose(RegisteredTasks definitions, Identities tasksToExecute);
    }
}
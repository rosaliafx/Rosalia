namespace Rosalia.Core.Api
{
    public interface ITaskRegistry<out T>
    {
        RegisteredTasks GetRegisteredTasks();
    }
}
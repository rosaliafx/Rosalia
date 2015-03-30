namespace Rosalia.Core.Api
{
    public abstract class Subflow : TaskRegistry
    {
    }
    
    public abstract class Subflow<T> : TaskRegistry<T> where T : class
    {
    }
}
namespace Rosalia.Core.Tasks
{
    public interface IResultsStorage
    {
        T GetValueOf<T>(Identity identity);

        IResultsStorage CreateDerived<T>(Identity task, T value);
    }
}
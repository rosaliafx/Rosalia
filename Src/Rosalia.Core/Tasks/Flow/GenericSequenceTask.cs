namespace Rosalia.Core.Tasks.Flow
{
    public abstract class GenericSequenceTask<T> : AbstractSequenceTask
    {
        protected new T Data
        {
            get { return (T) base.Data; }
            set { base.Data = value; }
        }
    }
}
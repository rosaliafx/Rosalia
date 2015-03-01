namespace Rosalia.Core.Logging
{
    public interface ILogRenderer
    {
        void Render(Message message, Identity source);
    }
}
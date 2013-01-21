namespace Rosalia.Core.Logging
{
    public interface ILogger
    {
        void Log(MessageLevel level, string message, params object[] args);
    }

    public static class LoggerExtensions
    {
        public static void Error(this ILogger logger, string message, params object[] args)
        {
            logger.Log(MessageLevel.Error, message, args);
        }

        public static void Warning(this ILogger logger, string message, params object[] args)
        {
            logger.Log(MessageLevel.Warning, message, args);
        }

        public static void Info(this ILogger logger, string message, params object[] args)
        {
            logger.Log(MessageLevel.Info, message, args);
        }

        public static void Success(this ILogger logger, string message, params object[] args)
        {
            logger.Log(MessageLevel.Success, message, args);
        }
    }
}
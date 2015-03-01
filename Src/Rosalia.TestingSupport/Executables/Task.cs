using System;

namespace Rosalia.TestingSupport.Executables
{
    using Rosalia.Core.Tasks;

    public static class Task
    {
        public static ITask<T> Const<T>(T value) where T : class
        {
            return new ValTask<T>(value);
        }

        public static ITask<T> Failure<T>(Exception exception) where T : class
        {
            return new FailureTask<T>(exception);
        }

        public static ITask<T> Failure<T>() where T : class
        {
            return Failure<T>((Exception) null);
        }

        public static ITask<T> Failure<T>(string errorMessage) where T : class
        {
            return Failure<T>(new Exception(errorMessage));
        }
    }
}
namespace Rosalia.Core.Logging
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Rosalia.Core.Interception;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;

    public class LoggingInterceptor : ITaskInterceptor
    {
        public void BeforeTaskExecute(Identity id, ITask<object> task, TaskContext context)
        {
        }

        public void AfterTaskExecute(Identity id, ITask<object> task, TaskContext context, ITaskResult<object> result)
        {
            if (result.IsSuccess)
            {
                object data = result.Data;
                if (data == null || data is Nothing)
                {
                    context.Log.Info("Completed");
                }
                else
                {
                    context.Log.Info("Completed. Result is {0}", FormatData(data));    
                }
            }
            else
            {
                context.Log.Error(result.Error == null ? "Unknown error occured" : result.Error.Message);
            }
        }

        private static string FormatData(object data, int level = 1)
        {
            if (data == null)
            {
                return "NULL";
            }

            Type dataType = data.GetType();
            if (dataType.IsPrimitive || dataType == typeof(string))
            {
                return data.ToString();
            }

            if (level > 3)
            {
                return "...";
            }

            FileSystemItem item = data as FileSystemItem;
            if (item != null)
            {
                return string.Format("[{0}]", item.AbsolutePath);
            }

            var result = new StringBuilder();
            result.AppendLine();
            foreach (PropertyInfo propertyInfo in dataType.GetProperties().Where(prop => prop.CanRead))
            {
                result.AppendFormat(
                    "{0}{1} = {2}",
                    new string(' ', level * 2),
                    propertyInfo.Name,
                    FormatData(propertyInfo.GetValue(data), level + 1));
                result.AppendLine();
            }

            return result.ToString();
        }
    }
}
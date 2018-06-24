namespace Rosalia.Runner.Console.CommandLine.Support
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public class ProgramSteps : IEnumerable
    {
        private readonly IList<IProgramStep> _steps = new List<IProgramStep>();

        public void Add(IProgramStep programStep)
        {
            _steps.Add(programStep);
        }

        public IEnumerator GetEnumerator()
        {
            return _steps.GetEnumerator();
        }

        public int Execute(ProgramContext context)
        {
            for (int index = 0; index < _steps.Count; index++)
            {
                var step = _steps[index];
                try
                {
                    var stepResult = step.Execute(context);
                    if (stepResult.HasValue)
                    {
                        return stepResult.Value;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorMessage(index, context, ex);

                    return GetStepErrorCode(index);
                }
            }

            return ExitCode.Ok;
        }

        private static void LogErrorMessage(int stepIndex, ProgramContext context, Exception ex)
        {
            var errorMessage = string.Format("Unexpected error occured at step {0}: {1}", stepIndex, ex.Message);
            if (context.Log != null)
            {
                try
                {
                    context.Log.Error(errorMessage);
                }
                catch
                {
                    Console.WriteLine(errorMessage);
                }
            }
            else
            {
                Console.WriteLine(errorMessage);
            }

            var innerException = ex.InnerException;
            if (innerException != null)
            {
                LogErrorMessage(stepIndex, context, innerException);
            }
        }

        private static int GetStepErrorCode(int index)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;

                return - (version.Major * 10000 + version.Minor * 100 + index);
            }
            catch (Exception)
            {
                return ExitCode.Error.UnknownError;
            }
        }
    }
}
namespace Rosalia.TaskLib.Standard
{
    using System;
    using Rosalia.Core;
    using Rosalia.Core.Fluent;

    public class SimpleExternalToolTask<TContext> : ExternalToolTask<TContext, SimpleExternalToolTask<TContext>.Input, object>
    {
        public SimpleExternalToolTask(Func<ExecutionContext<TContext>, Input> contextToInput)
            : base(contextToInput)
        {
        }

        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }

        public class Input : IExternalToolAware
        {
            public Input(string path, string arguments)
            {
                Path = path;
                Arguments = arguments;
            }

            public string Path { get; private set; }

            public string Arguments { get; private set; }
        }
    }
}
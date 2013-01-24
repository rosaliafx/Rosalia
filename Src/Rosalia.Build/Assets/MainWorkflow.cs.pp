namespace $rootnamespace$
{
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;

    public class MainWorkflow : Workflow<Context>
    {
        public override ITask<Context> RootTask
        {
            get
            {
                return Sequence(
                    //// Task 1
                    Task((result, c) => c.Logger.Info("Hello...")),

                    //// Task 2
                    Task((result, c) => c.Data.Message = "...world"),

                    //// Task 3
                    Task((result, c) => c.Logger.Info(c.Data.Message))
                );
            }
        }
    }
}
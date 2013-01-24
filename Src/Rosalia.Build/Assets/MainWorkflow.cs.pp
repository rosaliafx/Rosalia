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
                    Task((result, c) => c.Log.Info("Hello World!")),

                    //// Task 1
                    Task((result, c) => c.Log.Info(c.Data.Message))
                );
            }
        }
    }
}
using System;

namespace Rosalia.TestingSupport.Helpers
{
    public class QuickWorkflow : Core.Api.Workflow
    {
        private readonly Action<QuickWorkflow> _defineAction;

        private QuickWorkflow(Action<QuickWorkflow> defineAction)
        {
            _defineAction = defineAction;
        }

        protected override void RegisterTasks()
        {
            _defineAction.Invoke(this);
        }

        public static QuickWorkflow Define(Action<QuickWorkflow> defineAction)
        {
            return new QuickWorkflow(defineAction);
        }
    }
}
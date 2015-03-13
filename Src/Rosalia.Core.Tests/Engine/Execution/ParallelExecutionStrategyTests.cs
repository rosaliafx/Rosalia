namespace Rosalia.Core.Tests.Engine.Execution
{
    using Rosalia.Core.Engine.Execution;

    public class ParallelExecutionStrategyTests : ExecutionStrategyTests
    {
        protected override IExecutionStrategy CreateStrategy()
        {
            return new ParallelExecutionStrategy();
        }
    }
}
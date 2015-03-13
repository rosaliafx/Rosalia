namespace Rosalia.Core.Tests.Engine.Execution
{
    using Rosalia.Core.Engine.Execution;

    public class SequenceExecutionStrategyTests : ExecutionStrategyTests
    {
        protected override IExecutionStrategy CreateStrategy()
        {
            return new SequenceExecutionStrategy();
        }
    }
}
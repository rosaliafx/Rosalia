namespace Rosalia.Core.Tests.Engine.Execution
{
    using NUnit.Framework;
    using Rosalia.Core.Engine.Execution;

    [TestFixture]
    public class ParallelExecutionStrategyTests : ExecutionStrategyTests
    {
        protected override IExecutionStrategy CreateStrategy()
        {
            return new ParallelExecutionStrategy();
        }
    }
}
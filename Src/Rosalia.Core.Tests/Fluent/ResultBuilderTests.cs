namespace Rosalia.Core.Tests.Fluent
{
    using System.Linq;
    using NUnit.Framework;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Result;

    [TestFixture]
    public class ResultBuilderTests
    {
        private ResultBuilder _builder;

        [SetUp]
        public void Init()
        {
            _builder = new ResultBuilder();
        }

        [Test]
        public void AddMessage_ShouldFormat()
        {
            _builder.AddMessage(MessageLevel.Info, "{0}", "test");

            ExecutionResult result = _builder;

            Assert.That(result.Messages, Is.Not.Null);
            Assert.That(result.Messages.Count(), Is.EqualTo(1));
            Assert.That(result.Messages.ToList()[0].Text, Is.EqualTo("test"));
        }
    }
}
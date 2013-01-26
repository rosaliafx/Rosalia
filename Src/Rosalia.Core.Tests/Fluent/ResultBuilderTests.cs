namespace Rosalia.Core.Tests.Fluent
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;

    [TestFixture]
    public class ResultBuilderTests
    {
        [Test]
        public void AddMessage_ShouldPassToListener()
        {
            var messages = new List<Message>();
            var builder = new ResultBuilder(messages.Add);

            builder.AddMessage(MessageLevel.Info, "{0}", "test");

            Assert.That(messages.Count, Is.EqualTo(1));
        }
    }
}
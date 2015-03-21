namespace Rosalia.Core.Tests.Logging
{
    using Moq;
    using NUnit.Framework;
    using Rosalia.Core.Logging;
    using Rosalia.TestingSupport;

    [TestFixture]
    public class FilterLogRendererTests
    {
        [Test]
        public void Render_AppropriateMessage_ShouldPassToNested()
        {
            var nestedRenderer = new SpyLogRenderer();
            var logger = new FilterLogRenderer(nestedRenderer, MessageLevel.Info);

            logger.Render(new Message("foo", MessageLevel.Info), "test");

            Assert.That(nestedRenderer.Messages, Is.Not.Null);
            Assert.That(nestedRenderer.Messages, Is.Not.Empty);
            
            nestedRenderer.AssertHasMessage(MessageLevel.Info, "foo");
        }

        [Test]
        public void Render_NonAppropriateMessage_ShouldFilterOut()
        {
            var nestedRenderer = new SpyLogRenderer();
            var logger = new FilterLogRenderer(nestedRenderer, MessageLevel.Error);

            logger.Render(new Message("foo", MessageLevel.Info), "test");

            Assert.That(nestedRenderer.Messages, Is.Empty);
        }

        [Test]
        public void Init_ShouldCallNested()
        {
            var nested = new Mock<ILogRenderer>();
            var logger = new FilterLogRenderer(nested.Object, MessageLevel.Info);

            logger.Init();

            nested.Verify(x => x.Init());
        }

        [Test]
        public void Dispose_ShouldCallNested()
        {
            var nested = new Mock<ILogRenderer>();
            var logger = new FilterLogRenderer(nested.Object, MessageLevel.Info);

            logger.Dispose();

            nested.Verify(x => x.Dispose());
        }
    }
}
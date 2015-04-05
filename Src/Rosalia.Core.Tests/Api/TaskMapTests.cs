namespace Rosalia.Core.Tests.Api
{
    using System;
    using NUnit.Framework;
    using Rosalia.Core.Api;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.TestingSupport.Executables;

    [TestFixture]
    public class TaskMapTests
    {
        [Test]
        public void Register_DuplicateId_ShouldThrowError()
        {
            var map = new TaskMap();
            var ex = Assert.Throws<Exception>(() =>
            {
                map.Register("foo", Task.Const(""), new ITaskBehavior[0]);
                map.Register("foo", Task.Const(""), new ITaskBehavior[0]);
            });

            Assert.That(ex.Message, Is.EqualTo("Task with id [foo] has already been registered"));
        }
    }
}
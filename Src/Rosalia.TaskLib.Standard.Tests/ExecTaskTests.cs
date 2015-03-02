namespace Rosalia.TaskLib.Standard.Tests
{
    using NUnit.Framework;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.Standard.Tasks;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class ExecTaskTests
    {
        [Test]
        public void Execute_EchoTask_ShouldSucceed()
        {
            var echoTask = new ExecTask
            {
                ToolPath = "cmd.exe",
                Arguments = string.Format("/c echo {0}", "Hello, Rosalia!")
            };

            echoTask.Execute().AssertSuccess();
        }

        [Test]
        public void Execute_EchoTask_ShouldLogMessage()
        {
            var echoTask = new ExecTask
            {
                ToolPath = "cmd.exe",
                Arguments = string.Format("/c echo {0}", "Hello, Rosalia!")
            };

            echoTask
                .Execute()
                .AssertSuccess()
                .AssertHasMessage(MessageLevel.Info, "Hello, Rosalia!");
        }
    }
}
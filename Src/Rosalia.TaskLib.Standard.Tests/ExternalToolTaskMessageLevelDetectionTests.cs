namespace Rosalia.TaskLib.Standard.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.TaskLib.Standard.Tasks;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class ExternalToolTaskMessageLevelDetectionTests 
    {
        [Test]
        public void Execute_DetectorsDefined_ShouldUseDetectors()
        {
            var task = new Task(new List<Func<string, MessageLevel?>>
            {
                message => MessageLevel.Error
            });

            task.AssertProcessOutputParsing(
                "Test message",
                result =>
                    {
                        result.AssertHasMessage(MessageLevel.Error, "Test message");
                    });
        }

        internal class Task : ExternalToolTask<object>
        {
            private readonly IList<Func<string, MessageLevel?>> _messageLevelDetectors;

            public Task(IList<Func<string, MessageLevel?>> messageLevelDetectors)
            {
                _messageLevelDetectors = messageLevelDetectors;
            }

            protected override string GetToolPath(TaskContext context)
            {
                return "fake";
            }

            protected override void FillMessageLevelDetectors(IList<Func<string, MessageLevel?>> detectors)
            {
                foreach (var messageLevelDetector in _messageLevelDetectors)
                {
                    detectors.Add(messageLevelDetector);
                }
            }

            protected override object CreateResult(int exitCode, TaskContext context, IList<Message> aggregatedOutput)
            {
                return null;
            }
        }
    }
}
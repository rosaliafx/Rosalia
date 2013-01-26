namespace Rosalia.TaskLib.Standard.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;

    public class ExternalToolTaskMessageLevelDetectionTests : ExternalToolTaskTestsBase<object, object, object>
    {
        [Test]
        public void Execute_DetectorsDefined_ShouldUseDetectors()
        {
            var task = new Task(new List<Func<string, MessageLevel?>>
            {
                message => MessageLevel.Error
            });

            AssertProcessOutputParsing(
                task,
                "Test message",
                (result, execution) =>
                    {
                        Logger.AssertHasError();

                        Assert.That(Logger.LastError.Text, Is.EqualTo("Test message"));
                    });
        }

        internal class Task : ExternalToolTask<object, object, object>
        {
            private readonly IList<Func<string, MessageLevel?>> _messageLevelDetectors;

            public Task(IList<Func<string, MessageLevel?>> messageLevelDetectors)
            {
                _messageLevelDetectors = messageLevelDetectors;
            }

            protected override string GetToolPath(object input, TaskContext<object> context)
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

            protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
            {
                return null;
            }
        }
    }
}
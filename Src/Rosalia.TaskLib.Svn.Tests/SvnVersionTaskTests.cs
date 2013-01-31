namespace Rosalia.TaskLib.Svn.Tests
{
    using System;
    using NUnit.Framework;
    using Rosalia.Core;
    using Rosalia.TaskLib.Standard.Tests;

    public class SvnVersionTaskTests : ExternalToolTaskTestsBase<object, SvnVersionInput, SvnVersionResult>
    {
        [Test]
        public void Execute_CommitedIsTrue_ShouldAddCommitedOption()
        {
            AssertCommand(
                new SvnVersionInput("fake") { Commited = true }, 
                (path, args) => Assert.That(args, Is.StringStarting("-c ").Or.EqualTo("-c")));
        }
        
        [Test]
        public void Execute_CommitedIsFaluse_ShouldNotAddCommitedOption()
        {
            AssertCommand(
                new SvnVersionInput("fake") { Commited = false }, 
                (path, args) => Assert.That(args, Is.Not.StringStarting("-c ").And.Not.EqualTo("-c")));
        }

        [Test]
        public void Execute_WcPath_ShouldAddWcPath()
        {
            AssertCommand(
                new SvnVersionInput("WC_PATH"),
                (path, args) => Assert.That(args, Is.StringContaining("WC_PATH")));
        }

        [Test]
        public void Execute_WcPathAndTrail_ShouldAddWcPathAndTrail()
        {
            AssertCommand(
                new SvnVersionInput("WC_PATH", "TRAIL"), 
                (path, args) => Assert.That(args, Is.StringContaining("WC_PATH TRAIL")));
        }

        [Test]
        public void Execute_WrongOutput_ShouldFail()
        {
            AssertProcessOutputParsing(
                new SvnVersionTask<object>(c => new SvnVersionInput((string) "WC_PATH")), 
                "12:13:14",
                (svnVersionResult, taskResult) =>
                {
                    Assert.That(svnVersionResult, Is.Null);
                    Assert.That(taskResult.ResultType == ResultType.Failure);
                });

            Logger.AssertHasError();
            Assert.That(Logger.LastError.Text, Is.EqualTo("Unexpected tool output: 12:13:14"));
        }

        [Test]
        public void Execute_UnversionedDirectory_ShouldFail()
        {
            AssertProcessOutputParsing(
                new SvnVersionTask<object>(c => new SvnVersionInput((string) "WC_PATH")), 
                "Unversioned directory",
                (svnVersionResult, taskResult) =>
                {
                    Assert.That(svnVersionResult, Is.Null);
                    Assert.That(taskResult.ResultType == ResultType.Failure);
                });

            Logger.AssertHasError();
        }

        [Test]
        public void Execute_SingleRevisionNumber_ShouldSucceed()
        {
            AssertProcessOutputParsing(
                new SvnVersionTask<object>(c => new SvnVersionInput((string) "WC_PATH")), 
                "42",
                (svnVersionResult, taskResult) =>
                {
                    Assert.That(svnVersionResult, Is.Not.Null);
                    Assert.That(svnVersionResult.Min, Is.Not.Null);
                    Assert.That(svnVersionResult.Min.Number, Is.EqualTo(42));
                    Assert.That(svnVersionResult.Max, Is.Not.Null);
                    Assert.That(svnVersionResult.Max.Number, Is.EqualTo(42));

                    Assert.That(taskResult.ResultType == ResultType.Success);
                });
        }

        [Test]
        public void Execute_RevisionsWithTrailNumber_ShouldSucceed()
        {
            AssertProcessOutputParsing(
                new SvnVersionTask<object>(c => new SvnVersionInput((string) "WC_PATH")),
                "41MSP:42S",
                (svnVersionResult, taskResult) =>
                {
                    Assert.That(svnVersionResult, Is.Not.Null);

                    Assert.That(svnVersionResult.Min, Is.Not.Null);
                    Assert.That(svnVersionResult.Min.Number, Is.EqualTo(41));
                    Assert.That(svnVersionResult.Min.Trail, Is.EqualTo("MSP"));

                    Assert.That(svnVersionResult.Max, Is.Not.Null);
                    Assert.That(svnVersionResult.Max.Number, Is.EqualTo(42));
                    Assert.That(svnVersionResult.Max.Trail, Is.EqualTo("S"));

                    Assert.That(taskResult.ResultType == ResultType.Success);
                });
        }

        [Test]
        public void Execute_RevisionRangeNumber_ShouldSucceed()
        {
            AssertProcessOutputParsing(
                new SvnVersionTask<object>(c => new SvnVersionInput("WC_PATH")), 
                "41:42",
                (svnVersionResult, taskResult) =>
                {
                    Assert.That(taskResult.ResultType == ResultType.Success);

                    Assert.That(svnVersionResult, Is.Not.Null);
                    Assert.That(svnVersionResult.Min, Is.Not.Null);
                    Assert.That(svnVersionResult.Min.Number, Is.EqualTo(41));
                    Assert.That(svnVersionResult.Max, Is.Not.Null);
                    Assert.That(svnVersionResult.Max.Number, Is.EqualTo(42));
                });
        }

        private void AssertCommand(SvnVersionInput input, Action<string, string> assertAction)
        {
            var task = new SvnVersionTask<object>(context => input);

            AssertCommand(task, assertAction);
        }
    }
}
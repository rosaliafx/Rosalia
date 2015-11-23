namespace Rosalia.TaskLib.Svn.Tests
{
    using NUnit.Framework;
    using Rosalia.Core.Logging;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class SvnVersionTaskTests
    {
        [Test]
        public void Execute_CommitedIsTrue_ShouldAddCommitedOption()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "fake",
                Commited = true
            };

            task.AssertCommand(
                (path, args) => Assert.That(args, Does.StartWith("-c ").Or.EqualTo("-c")));
        }

        [Test]
        public void Execute_CommitedIsFalse_ShouldNotAddCommitedOption()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "fake",
                Commited = false
            };

            task.AssertCommand(
                (path, args) => Assert.That(args, Is.Not.StringStarting("-c ").And.Not.EqualTo("-c")));
        }

        [Test]
        public void Execute_WcPath_ShouldAddWcPath()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "WC_PATH"
            };
            
            task.AssertCommand(
                (path, args) => Assert.That(args, Is.StringContaining("WC_PATH")));
        }

        [Test]
        public void Execute_WcPathAndTrail_ShouldAddWcPathAndTrail()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "WC_PATH",
                TrailUrl = "TRAIL"
            };
            
            task.AssertCommand(
                (path, args) => Assert.That(args, Is.StringContaining("WC_PATH TRAIL")));
        }

        [Test]
        public void Execute_WrongOutput_ShouldFail()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "WC_PATH"
            };

            task.AssertProcessOutputParsing(
                "12:13:14",
                result =>
                {
                    result.AssertFailedWith("Unexpected tool output: 12:13:14");
                });
        }

        [Test]
        public void Execute_UnversionedDirectory_ShouldFail()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "WC_PATH"
            };

            task.AssertProcessOutputParsing(
                "Unversioned directory",
                result =>
                {
                    result.AssertFailure();
                });
        }

        [Test]
        public void Execute_SingleRevisionNumber_ShouldSucceed()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "WC_PATH"
            };

            task.AssertProcessOutputParsing(
                "42",
                result =>
                {
                    result.AssertSuccess();
                    Assert.That(result.Data, Is.Not.Null);
                    Assert.That(result.Data.Min, Is.Not.Null);
                    Assert.That(result.Data.Min.Number, Is.EqualTo(42));
                    Assert.That(result.Data.Max, Is.Not.Null);
                    Assert.That(result.Data.Max.Number, Is.EqualTo(42));
                });
        }

        [Test]
        public void Execute_RevisionsWithTrailNumber_ShouldSucceed()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "WC_PATH"
            };

            task.AssertProcessOutputParsing(
                "41MSP:42S",
                result =>
                {
                    result.AssertSuccess();

                    Assert.That(result.Data, Is.Not.Null);
                    Assert.That(result.Data.Min, Is.Not.Null);
                    Assert.That(result.Data.Min.Number, Is.EqualTo(41));
                    Assert.That(result.Data.Min.Trail, Is.EqualTo("MSP"));
                    Assert.That(result.Data.Max, Is.Not.Null);
                    Assert.That(result.Data.Max.Number, Is.EqualTo(42));
                    Assert.That(result.Data.Max.Trail, Is.EqualTo("S"));
                });
        }

        [Test]
        public void Execute_RevisionRangeNumber_ShouldSucceed()
        {
            var task = new SvnVersionTask
            {
                WorkingCopyPath = "WC_PATH"
            };

            task.AssertProcessOutputParsing(
                "41:42",
                result =>
                {
                    result.AssertSuccess();

                    Assert.That(result.Data, Is.Not.Null);
                    Assert.That(result.Data.Min, Is.Not.Null);
                    Assert.That(result.Data.Min.Number, Is.EqualTo(41));
                    Assert.That(result.Data.Max, Is.Not.Null);
                    Assert.That(result.Data.Max.Number, Is.EqualTo(42));
                });
        }
    }
}
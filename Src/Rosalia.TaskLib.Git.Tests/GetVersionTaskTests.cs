namespace Rosalia.TaskLib.Git.Tests
{
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.Git.Results;
    using Rosalia.TaskLib.Git.Tasks;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class GetVersionTaskTests 
    {
        [Test]
        public void Execute_ValidOutput_SholdParse()
        {
            var task = new GetVersionTask();
            task.AssertProcessOutputParsing(
                "v0.1.0-10-gcaa0502",
                result =>
                    {
                        result.AssertSuccess();

                        Assert.That(result.Data, Is.Not.Null);
                        Assert.That(result.Data.CommitsCount, Is.EqualTo(10));
                    });
        }

        [Test]
        public void Execute_NoTagOutput_SholdFail()
        {
            var task = new GetVersionTask();
            task.AssertProcessOutputParsing(
                "fatal: No names found, cannot describe anything.",
                (result) =>
                    {
                        result.Log.AssertHasMessage(MessageLevel.Error, "fatal: No names found, cannot describe anything.");
                        result.AssertFailure();
                    });
        }
    }
}
namespace Rosalia.TaskLib.Git.Tests
{
    using NUnit.Framework;
    using Rosalia.Core;
    using Rosalia.TaskLib.Git.Output;
    using Rosalia.TaskLib.Git.Tasks;
    using Rosalia.TaskLib.Standard.Tests;

    public class GetVersionTaskTests : ExternalToolTaskTestsBase<GetVersionOutput>
    {
        [Test]
        public void Execute_ValidOutput_SholdParse()
        {
            var task = new GetVersionTask();
            AssertProcessOutputParsing(
                task,
                "v0.1.0-10-gcaa0502",
                (output, result) =>
                    {
                        Assert.That(result.ResultType, Is.EqualTo(ResultType.Success));
                        Assert.That(output, Is.Not.Null);
                        Assert.That(output.CommitsCount, Is.EqualTo(10));
                    });
        }

        [Test]
        public void Execute_NoTagOutput_SholdFail()
        {
            var task = new GetVersionTask();
            AssertProcessOutputParsing(
                task,
                "fatal: No names found, cannot describe anything.",
                (output, result) =>
                    {
                        //Assert.That(result.ResultType, Is.EqualTo(ResultType.Failure));
                        Logger.AssertHasError();
                        Assert.That(Logger.LastError.Text, Is.EqualTo("fatal: No names found, cannot describe anything."));
                    });
        }
    }
}
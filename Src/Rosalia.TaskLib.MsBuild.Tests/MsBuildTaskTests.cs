namespace Rosalia.TaskLib.MsBuild.Tests
{
    using NUnit.Framework;
    using Rosalia.FileSystem;
    using Rosalia.TestingSupport.FileSystem;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class MsBuildTaskTests
    {
        [Test]
        public void Execute_ToolPathPrivided_ShouldUseToolPath()
        {
            var task = new MsBuildTask
            {
                ToolPath = "myMsBuild",
                ProjectFile = new DefaultFile("projectFile")
            };

            task.AssertCommand(
                (msBuildPath, msBuildArguments) =>
                {
                    Assert.That(msBuildPath, Is.EqualTo("myMsBuild"));
                });
        }

        [Test]
        public void Execute_NoProjectFile_ShouldLookupProjectFileInWorkDirectory()
        {
            var workDirectory = new DirectoryStub("Src")
            {
                new DirectoryStub("DotNet")
                {
                    new DirectoryStub("Build"),
                    new FileStub("Rosalia.sln")
                },
                new DirectoryStub("Artifacts"),
                new DirectoryStub("Scripts")
            };

            var task = new MsBuildTask
            {
                ToolPath = "myMsBuild"
            };

            task.AssertCommand(
                workDirectory["DotNet"]["Build"],
                (msBuildPath, msBuildArguments) =>
                {
                    Assert.That(msBuildArguments.Trim(), Is.EqualTo("Src\\DotNet\\Rosalia.sln"));
                });
        }

        [Test]
        public void Execute_NoProjectFileAndNoSolutionInWorkDirectory_ShouldLeaveProjectFileBlank()
        {
            var workDirectory = new DirectoryStub("Src")
            {
                new DirectoryStub("DotNet")
                {
                    new DirectoryStub("Build"),
                },
                new DirectoryStub("Artifacts"),
                new DirectoryStub("Scripts")
            };

            var task = new MsBuildTask
            {
                ToolPath = "myMsBuild"
            };

            task.AssertCommand(
                workDirectory["DotNet"]["Build"],
                (msBuildPath, msBuildArguments) =>
                {
                    Assert.That(msBuildArguments.Trim(), Is.Empty);
                });
        }

        [Test]
        public void Execute_HasProjectFile_ShouldGenerateCommandLine()
        {
            var task = new MsBuildTask
            {
                ToolPath = "myMsBuild",
                ProjectFile = new DefaultFile("projectFile")
            };

            task.AssertCommand((msBuildPath, msBuildArguments) =>
            {
                Assert.That(msBuildArguments.Trim(), Is.EqualTo("projectFile"));
            });
        }
    }
}
namespace Rosalia.Demo
{
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Flow;
    using Rosalia.TaskLib.AssemblyInfo;
    using Rosalia.TaskLib.Compression;
    using Rosalia.TaskLib.MsBuild;

    public class DemoWorkflow : Workflow
    {
        public override void RegisterTasks()
        {
            /* ======================================================================================== */
            Register(
                name: "Print simple info message",
                task: () => Result.AddInfo("Start executing the demo workflow"));
            
            /* ======================================================================================== */
            Register(
                name: "Copy actual artifacts to tools",
                task: () =>
                {
                    var toolsDirectory = Context.WorkDirectory
                        .Parent
                        .Parent
                        .GetDirectory("Tools")
                        .GetDirectory("Rosalia");

                    Context.WorkDirectory
                        .Parent
                        .GetDirectory("Rosalia.Runner.Console")
                        .GetDirectory(@"bin\Debug")
                        .Files
                        .IncludeByExtension(".dll", ".exe")
                        .CopyAllTo(toolsDirectory);
                });

            /* ======================================================================================== */
            Register(
                name: "Generate assempbly info",
                task: new GenerateAssemblyInfo()
                    .WithAttribute(_ => new AssemblyCompanyAttribute("DemoCompany"))
                    .WithAttribute(_ => new AssemblyFileVersionAttribute("1.0.0.1")),
                beforeExecute: task => task
                    .ToFile(Context.WorkDirectory.GetFile("CommonAssemblyInfo.cs")));

            /* ======================================================================================== */
            Register(
                name: "Build main solution",
                task: new FailoverTask<MsBuildTask, SimpleTask>(
                    new MsBuildTask(), 
                    new SimpleTask((builder, context) =>
                    {
                        builder.AddWarning("Build task fails because the solition file does not exist");                                    
                    })),
                beforeExecute: task => task.Target.WithProjectFile("NoFile.sln"));

            /* ======================================================================================== */
            Register(
                name: "List current directory files",
                task: () =>
                {
                    Result.AddInfo("This task lists current directory files");
                    foreach (var file in Context.WorkDirectory.Files)
                    {
                        Result.AddInfo(file.AbsolutePath);
                    }
                });

            /* ======================================================================================== */
            Register(
                task: new CompressTask(),
                beforeExecute: task =>
                {
                    var allCsFiles = Context.FileSystem
                        .SearchFilesIn(Context.WorkDirectory)
                        .IncludeByExtension(".cs");

                    foreach (var file in allCsFiles.All)
                    {
                        task.WithFile(Path.GetFileName(file.AbsolutePath), file);
                    }

                    task.ToFile(Context.WorkDirectory.GetFile("cs_files.zip"));
                });

            /* ======================================================================================== */

            Register(
                name: "Test sequence",
                task: new SequenceTask()
                    .Register(
                        name: "sub 1",
                        task: () => Result.AddInfo("sub 1 info"))
                    .Register(
                        name: "sub 2",
                        task: () => Result.AddWarning("sub 2 info"))
                    .Register(
                        name: "sub 3",
                        task: () => Result.AddInfo("sub 3 info")));
        }
    }
}
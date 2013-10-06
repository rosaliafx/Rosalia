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

    public class DemoWorkflow : Workflow<DemoContext>
    {
        public override void RegisterTasks()
        {
            /* ======================================================================================== */
            Register(
                name: "Print simple info message",
                task: (result, context) => result.AddInfo("Start executing the demo workflow"));
            
            /* ======================================================================================== */
            Register(
                name: "Copy actual artifacts to tools",
                task: (builder, context) =>
                {
                    var toolsDirectory = context.WorkDirectory
                        .Parent
                        .Parent
                        .GetDirectory("Tools")
                        .GetDirectory("Rosalia");

                    context.WorkDirectory
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
                task: new GenerateAssemblyInfo<DemoContext>()
                    .WithAttribute(_ => new AssemblyCompanyAttribute("DemoCompany"))
                    .WithAttribute(_ => new AssemblyFileVersionAttribute("1.0.0.1")),
                beforeExecute: (context, task) => task
                    .ToFile(context.WorkDirectory.GetFile("CommonAssemblyInfo.cs")));

            /* ======================================================================================== */
            Register(
                name: "Build main solution",
                task: new FailoverTask<DemoContext, MsBuildTask<DemoContext>, SimpleTask<DemoContext>>(
                    new MsBuildTask<DemoContext>(), 
                    new SimpleTask<DemoContext>((builder, context) =>
                    {
                        builder.AddWarning("Build task fails because the solition file does not exist");                                    
                    })),
                beforeExecute: (context, task) => task.Target.WithProjectFile("NoFile.sln"));

            /* ======================================================================================== */
            Register(
                name: "List current directory files",
                task: (builder, context) =>
                {
                    builder.AddInfo("This task lists current directory files");
                    foreach (var file in context.WorkDirectory.Files)
                    {
                        builder.AddInfo(file.AbsolutePath);
                    }
                });

            /* ======================================================================================== */
            Register(
                task: new CompressTask<DemoContext>(),
                beforeExecute: (context, task) =>
                {
                    var allCsFiles = context.FileSystem
                        .SearchFilesIn(context.WorkDirectory)
                        .IncludeByExtension(".cs");

                    foreach (var file in allCsFiles.All)
                    {
                        task.WithFile(Path.GetFileName(file.AbsolutePath), file);
                    }

                    task.ToFile(context.WorkDirectory.GetFile("cs_files.zip"));
                });

            /* ======================================================================================== */

            Register(
                name: "Test sequence",
                task: Sequence()
                    .Register(
                        name: "sub 1",
                        task: (builder, context) => builder.AddInfo("sub 1 info"))
                    .Register(
                        name: "sub 2",
                        task: (builder, context) => builder.AddWarning("sub 2 info"))
                    .Register(
                        name: "sub 3",
                        task: (builder, context) => builder.AddInfo("sub 3 info")));
        }
    }
}
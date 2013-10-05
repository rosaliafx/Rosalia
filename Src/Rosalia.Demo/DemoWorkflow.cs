﻿namespace Rosalia.Demo
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
                    .WithAttribute(c => new AssemblyCompanyAttribute("DemoCompany"))
                    .WithAttribute(c => new AssemblyFileVersionAttribute("1.0.0.1")),
                beforeExecute: (context, task) => task
                    .ToFile(c => context.WorkDirectory.GetFile("CommonAssemblyInfo.cs")));

            /* ======================================================================================== */
            Register(
                name: "Build main solution",
                task: new FailoverTask<DemoContext>(
                    new MsBuildTask<DemoContext>()
                        .FillInput(context => new MsBuildInput().WithProjectFile("NoFile.sln")), 
                    new SimpleTask<DemoContext>((builder, context) =>
                        {
                            builder.AddWarning("Build task fails because the solition file does not exist");                                    
                        })));

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
                new CompressTask<DemoContext>()
                    .FillInput(context =>
                    {
                        var allCsFiles = context.FileSystem
                            .SearchFilesIn(context.WorkDirectory)
                            .IncludeByExtension(".cs");

                        var compressTaskInput = new CompressTaskInput();

                        foreach (var file in allCsFiles.All)
                        {
                            compressTaskInput.WithFile(Path.GetFileName(file.AbsolutePath), file);
                        }

                        return compressTaskInput.ToFile(context.WorkDirectory.GetFile("cs_files.zip"));
                    }));

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
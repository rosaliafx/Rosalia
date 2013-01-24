namespace Rosalia.Demo
{
    using System.IO;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.AssemblyInfo;
    using Rosalia.TaskLib.Compression;
    using Rosalia.TaskLib.MsBuild;

    public class DemoWorkflow : Workflow<DemoContext>
    {
        public override ITask<DemoContext> RootTask
        {
            get
            {
                return Sequence
                    .WithSubtask((builder, c) => c.Logger.Info("Start executing the demo workflow"))
                    .WithSubtask(CopyActualArtifactsToTools())
                    .WithSubtask(new GenerateAssemblyInfo<DemoContext>()
                                     .WithAttribute(c => new AssemblyCompanyAttribute("DemoCompany"))
                                     .WithAttribute(c => new AssemblyFileVersionAttribute("1.0.0.1"))
                                     .ToFile(context => context.WorkDirectory.GetFile("CommonAssemblyInfo.cs")))
                    .WithSubtask(BuildSolution)
                    .WithSubtask((builder, context) =>
                                     {
                                         context.Logger.Info("This task lists current directory files");
                                         foreach (var file in context.WorkDirectory.Files)
                                         {
                                             context.Logger.Info(file.AbsolutePath);
                                         }
                                     })
                    .WithSubtask(new CompressTask<DemoContext>()
                                     .FillInput(context =>
                                                    {
                                                        context.Logger.Info(
                                                            "This task compresses all *.cs files in the current directory");

                                                        var allCsFiles = context.FileSystem
                                                            .SearchFilesIn(context.WorkDirectory)
                                                            .Include(fileName => Path.GetExtension(fileName) == ".cs");

                                                        var compressTaskInput = new CompressTaskInput();

                                                        foreach (var file in allCsFiles.All)
                                                        {
                                                            compressTaskInput.WithFile(Path.GetFileName(file.AbsolutePath), file);
                                                        }

                                                        return compressTaskInput
                                                            .ToFile(context.WorkDirectory.GetFile("cs_files.zip"));
                                                    }));
            }
        }

        private ITask<DemoContext> CopyActualArtifactsToTools()
        {
            return new SimpleTask<DemoContext>((builder, context) =>
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
                    .Include(file => file.Name.EndsWith(".dll") || file.Name.EndsWith(".exe"))
                    .CopyAllTo(toolsDirectory);
            });
        }

        protected ITask<DemoContext> BuildSolution
        {
            get
            {
                return new FailoverTask<DemoContext>(
                    new MsBuildTask<DemoContext>()
                        .FillInput(context => new MsBuildInput()
                            .WithProjectFile("NoFile.sln")), 
                    new SimpleTask<DemoContext>((builder, context) =>
                    {
                        context.Logger.Warning("Build task fails because the solition file does not exist");                                    
                    }));
            }
        }
    }
}
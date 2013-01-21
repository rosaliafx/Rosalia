namespace Rosalia.Demo
{
    using System.Reflection;
    using System.Security;
    using Rosalia.Core;
    using Rosalia.Core.Logging;
    using Rosalia.TaskLib.AssemblyInfo;
    using Rosalia.TaskLib.Compression;
    using Rosalia.TaskLib.Standard;
    using Rosalia.TaskLib.Svn;

    public class DemoWorkflow : Workflow<DemoContext>
    {
        public override ITask<DemoContext> CreateRootTask()
        {
            return MainSequence();
        }

        private SequenceTask<DemoContext> MainSequence()
        {
            return Sequence
                .WithSubtask(MainSvnVersion)
                .WithSubtask(new GenerateAssemblyInfo<DemoContext>()
                    .WithAttribute(c => new AssemblyCompanyAttribute("Zadv"))
                    .WithAttribute(c => new AssemblyProductAttribute("ZTrac Enterpris"))
                    .WithAttribute(c => new AssemblyCopyrightAttribute("ZTrac Enterpris"))
                    .WithAttribute(c => new AssemblyVersionAttribute(c.VersionNumber))
                    .WithAttribute(c => new AssemblyFileVersionAttribute(c.VersionNumber))
                    .WithAttribute(c => new AssemblyInformationalVersionAttribute(c.VersionNumber))
                    .WithAttribute(c => new AllowPartiallyTrustedCallersAttribute())
                    .ToFile(c => c.WorkDirectory.GetFile("test.cs")))
                .WithSubtask(new CompressTask<DemoContext>()
                    .FillInput(c => new CompressTaskInput()
                        .WithFile("a/b/c/testAssemblyInfo.cs", c.WorkDirectory.GetFile("test.cs"))
                        .ToFile(c.WorkDirectory.GetFile("my_test.zip"))))
                .WithSubtask(NestedSequence())
//                .WithSubtask(new MsBuildTask<DemoContext>()
//                    .FillInput(c => new MsBuildInput()
//                        //.WithSwitch("verbosity", "m")
//                        .WithProjectFile(@"C:\Projects\Codemasters\ZTracEnterprise\trunk\DotNet\ZTrac.Enterprise.sln")
//                        .WithConfiguration("Staging")
//                        .WithBuildTargets("Rebuild")))
                .WithSubtask(FailSvnVersion)
                .WithSubtask((builder, context) => context.Logger.Warning(context.Data.VersionNumber))
                .WithSubtask(new SimpleExternalToolTask<DemoContext>(context => new SimpleExternalToolTask<DemoContext>.Input("cmd.exe", "/c echo test")))
                .WithSubtask((result, context) =>
                {
                    context.Logger.Info("Hello, Rosalia!");
                    context.Logger.Warning("Test warning");
                });
        }

        private static ITask<DemoContext> FailSvnVersion
        {
            get
            {
                return new SvnVersionTask<DemoContext>(
                    context => new SvnVersionInput(
                    @"C:\Projects\Codemasters\ZTracEnterprise\trunk\Tools\Svn\svnversion.exe",
                    @"C:\Projects"))
                    .ApplyResult((result, context) => { context.VersionNumber = result.Min.Number.ToString(); });
            }
        }

        private static ExtendedTask<DemoContext, SvnVersionInput, SvnVersionResult> MainSvnVersion
        {
            get
            {
                return new SvnVersionTask<DemoContext>(context => new SvnVersionInput(
                                                                      @"C:\Projects\Codemasters\ZTracEnterprise\trunk\Tools\Svn\svnversion.exe",
                                                                      @"C:\Projects\Codemasters\ZTracEnterprise"))
                    .ApplyResult((result, context) =>
                    {
                        context.VersionNumber = string.Format("1.0.{0}.0", result.Min.Number);
                    });
            }
        }

        private SequenceTask<DemoContext> NestedSequence()
        {
            return Sequence
                .WithSubtask(new ForkTask<DemoContext>
                {
                    { c => c.Data.FirstName != null, (result, c) => c.Logger.Info("Variant1") },
                    { c => c.Data.FirstName == null, (result, c) => c.Logger.Info("Variant2") },
                })
                .WithSubtask((result, context) => context.Logger.Info("Hello, {0} {1}!", context.Data.FirstName, context.Data.LastName))
                .WithSubtask((result, context) =>
                {
                    var directory = context.WorkDirectory.Parent.Parent;

                    context.Logger.Warning("==== Directories:");
                    foreach (var item in directory.Directories)
                    {
                        context.Logger.Info(item.AbsolutePath);
                    }

                    context.Logger.Warning("==== Files:");
                    foreach (var item in directory.Files)
                    {
                        context.Logger.Info(item.AbsolutePath);
                    }
                })
                //.WithSubtask((result, context) => { throw new Exception("Fatal error!"); })
                ;
        }
    }
}
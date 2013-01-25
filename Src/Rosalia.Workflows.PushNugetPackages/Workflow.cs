namespace Rosalia.Workflows.PushNugetPackages
{
    using Rosalia.Core;
    using Rosalia.TaskLib.NuGet.Input;
    using Rosalia.TaskLib.NuGet.Tasks;

    public class MainWorkflow : Workflow<object>
    {
        public override ITask<object> RootTask
        {
            get
            {
                return ForEach(c => c.WorkDirectory.Files.Include(fileName => fileName.EndsWith(".nupkg")))
                    .Do((c, file) => new PushPackageTask<object>(new PushInput().Package(file)));
            }
        }
    }
}
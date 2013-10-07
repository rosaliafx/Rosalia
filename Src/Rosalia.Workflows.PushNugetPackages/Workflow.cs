namespace Rosalia.Workflows.PushNugetPackages
{
    using Rosalia.Core;
    using Rosalia.TaskLib.NuGet.Tasks;

    public class MainWorkflow : Workflow
    {
        public override void RegisterTasks()
        {
            Register(ForEach(c => c.WorkDirectory.Files.Include(fileName => fileName.EndsWith(".nupkg")))
                .Do((c, file) => new PushPackageTask().Package(file)));
        }
    }
}
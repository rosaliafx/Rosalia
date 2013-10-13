namespace Rosalia.Workflows.PushNugetPackages
{
    using Rosalia.Core;
    using Rosalia.TaskLib.NuGet.Tasks;

    public class MainWorkflow : Workflow
    {
        public override void RegisterTasks()
        {
            Register(
                name: "Push compiled NUGET packages to server",
                task: ForEach(
                    () => Context.WorkDirectory.Files.Include(fileName => fileName.EndsWith(".nupkg")),
                    file => Register(
                        name: "Push package " + file.Name,
                        task: new PushPackageTask().Package(file))));
        }
    }
}
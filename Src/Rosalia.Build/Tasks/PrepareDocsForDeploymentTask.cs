namespace Rosalia.Build.Tasks
{
    using System.Yaml.Serialization;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Tasks.Flow;
    using Rosalia.TaskLib.Git.Tasks;

    public class PrepareDocsForDeploymentTask : GenericSequenceTask<BuildRosaliaContext>
    {
        public override void RegisterTasks()
        {
            ////
            Register(
                name: "Read private data",
                task: () =>
                {
                    var serializer = new YamlSerializer();
                    Data.PrivateData = (PrivateData)serializer.DeserializeFromFile(Context.WorkDirectory.GetFile("private_data.yaml").AbsolutePath, typeof(PrivateData))[0];
                });

            ////
            Register(
                name: "Copy docs artifats to GhPages repo",
                task: () =>
                {
                    var docsHost = Data.Src.GetDirectory("Rosalia.Docs");
                    var files = docsHost
                        .SearchFilesIn()
                        .IncludeByRelativePath(relative => relative.Equals("index.html") || relative.StartsWith("content") || relative.StartsWith("topics"));

                    files.CopyRelativelyTo(new DefaultDirectory(Data.PrivateData.GhPagesRoot));
                });

            ////
            Register(
                name: "Add all doc files to gh-pages repo",
                task: new GitCommandTask
                {
                    RawCommand = "add ."
                },
                beforeExecute: task =>
                {
                    task.WorkDirectory = new DefaultDirectory(Data.PrivateData.GhPagesRoot);
                });

            ////
            Register(
                name: "Do auto commit to gh-pages repo",
                task: new GitCommandTask(),
                beforeExecute: task =>
                {
                    task.RawCommand = string.Format("commit -a -m \"Docs auto commit v{0}\"", Data.Version);
                    task.WorkDirectory = new DefaultDirectory(Data.PrivateData.GhPagesRoot);
                });
        }
    }
}
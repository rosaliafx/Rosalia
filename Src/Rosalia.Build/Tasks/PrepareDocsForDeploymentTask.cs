namespace Rosalia.Build.Tasks
{
    using System.Yaml.Serialization;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Tasks.Flow;
    using Rosalia.TaskLib.Git.Tasks;

    public class PrepareDocsForDeploymentTask : AbstractSequenceTask<BuildRosaliaContext>
    {
        public override void RegisterTasks()
        {
            Register(
                name: "Read private data",
                task: (builder, c) =>
                {
                    var serializer = new YamlSerializer();
                    c.Data.PrivateData = (PrivateData)serializer.DeserializeFromFile(c.WorkDirectory.GetFile("private_data.yaml").AbsolutePath, typeof(PrivateData))[0];
                });

            Register(
                name: "Copy docs artifats to GhPages repo",
                task: (builder, context) =>
                {
                    var docsHost = context.Data.Src.GetDirectory("Rosalia.Docs");
                    var files = docsHost
                        .SearchFilesIn()
                        .IncludeByRelativePath(relative => relative.Equals("index.html") || relative.StartsWith("content") || relative.StartsWith("topics"));

                    files.CopyRelativelyTo(new DefaultDirectory(context.Data.PrivateData.GhPagesRoot));
                });

            Register(
                name: "Add all doc files to gh-pages repo",
                task: new GitCommandTask<BuildRosaliaContext>
                {
                    RawCommand = "add ."
                },
                beforeExecute: (context, task) =>
                {
                    task.WorkDirectory = new DefaultDirectory(context.Data.PrivateData.GhPagesRoot);
                });

            Register(
                name: "Do auto commit to gh-pages repo",
                task: new GitCommandTask<BuildRosaliaContext>(),
                beforeExecute: (context, task) =>
                {
                    task.RawCommand = string.Format("commit -a -m \"Docs auto commit v{0}\"", context.Data.Version);
                    task.WorkDirectory = new DefaultDirectory(context.Data.PrivateData.GhPagesRoot);
                });
        }
    }
}
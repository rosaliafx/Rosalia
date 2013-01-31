namespace Rosalia.TaskLib.Standard.Input
{
    using Rosalia.Core.FileSystem;

    public class ExternalToolInput
    {
        public virtual IDirectory WorkDirectory { get; set; }

        public virtual string ToolPath { get; set; }

        public virtual string Arguments { get; set; }
    }
}
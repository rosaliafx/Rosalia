namespace Rosalia.TaskLib.Standard
{
    using System;

    public interface IProcessStarter
    {
        int StartProcess(
            string path,
            string arguments,
            string workDirectory,
            Action<string> onStantardOutput,
            Action<string> onErrorOutput);
    }
}
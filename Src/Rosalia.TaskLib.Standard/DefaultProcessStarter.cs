namespace Rosalia.TaskLib.Standard
{
    using System;
    using System.Diagnostics;

    public class DefaultProcessStarter : IProcessStarter
    {
        private static object _lockObject = new object();

        public int StartProcess(
            string path, 
            string arguments, 
            string workDirectory,
            Action<string> onStantardOutput, 
            Action<string> onErrorOutput)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = workDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (sender, args) =>
                {
                    lock (_lockObject)
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            onStantardOutput(args.Data);
                        }
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    lock (_lockObject)
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            onErrorOutput(args.Data);
                        }
                    }
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                return process.ExitCode;
            }
        }
    }
}
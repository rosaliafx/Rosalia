﻿namespace Rosalia.TaskLib.NuGet
{
    public class FrameworkAssembly
    {
        public FrameworkAssembly(string assemblyName, string targetFramework)
        {
            AssemblyName = assemblyName;
            TargetFramework = targetFramework;
        }

        public string AssemblyName { get; private set; }

        public string TargetFramework { get; private set; }
    }
}
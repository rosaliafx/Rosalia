namespace Rosalia.TaskLib.NuGet.Tasks
{
    using System;
    using System.IO;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.NuGet.Input;
    using Rosalia.TaskLib.Standard;

    public class GenerateNuGetSpecTask<T> : ExtendedTask<T, SpecInput, object>
    {
        public GenerateNuGetSpecTask()
        {
        }

        public GenerateNuGetSpecTask(Action<TaskContext<T>, SpecInput> configureInput)
        {
            FillInput(context =>
            {
                var input = new SpecInput();
                configureInput(context, input);
                return input;
            });
        }

        protected override object Execute(SpecInput input, TaskContext<T> context, ResultBuilder resultBuilder)
        {
            using (var writer = new StreamWriter(input.Destination.WriteStream))
            {
                writer.WriteLine("<?xml version='1.0' encoding='utf-8'?>");
                writer.WriteLine("<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>");
                writer.WriteLine("  <metadata>");
                foreach (var metadata in input.Metadata)
                {
                    writer.WriteLine("    <{0}>{1}</{0}>", metadata.Key, metadata.Value);
                }

                if (input.Dependencies.Count > 0)
                {
                    writer.WriteLine("    <dependencies>");

                    var targetFrameworks = input.Dependencies.Select(x => x.TargetFramework).Distinct();
                    foreach (var targetFramework in targetFrameworks)
                    {
                        var framework = targetFramework;
                        writer.Write("      <group");
                        WriteOptionalAttribute("targetFramework", targetFramework, writer);
                        writer.WriteLine(">");

                        foreach (var dependency in input.Dependencies.Where(d => d.TargetFramework == framework))
                        {
                            writer.Write("        <dependency id='{0}'", dependency.Id);
                            WriteOptionalAttribute("version", dependency.Version, writer);
                            writer.WriteLine(" />");
                        }

                        writer.WriteLine("      </group>");
                    }

                    writer.WriteLine("    </dependencies>");
                }

                if (input.References.Count > 0)
                {
                    writer.WriteLine("    <references>");
                    foreach (var reference in input.References)
                    {
                        writer.WriteLine("      <reference file='{0}' />", reference);
                    }

                    writer.WriteLine("    </references>");
                }

                if (input.FrameworkAssemblies.Count > 0)
                {
                    writer.WriteLine("    <frameworkAssemblies>");
                    foreach (var frameworkAssembly in input.FrameworkAssemblies)
                    {
                        writer.Write("      <frameworkAssembly assemblyName='{0}'", frameworkAssembly.AssemblyName);
                        WriteOptionalAttribute("targetFramework", frameworkAssembly.TargetFramework, writer);
                        writer.WriteLine(" />");
                    }

                    writer.WriteLine("    </frameworkAssemblies>");
                }

                writer.WriteLine("  </metadata>");

                if (input.Files.Count > 0)
                {
                    writer.WriteLine("  <files>");
                    foreach (var file in input.Files)
                    {
                        writer.Write("    <file src='{0}'", file.Src);
                        WriteOptionalAttribute("target", file.Target, writer);
                        WriteOptionalAttribute("exclude", file.Exclude, writer);
                        writer.WriteLine(" />");
                    }

                    writer.WriteLine("  </files>");
                }

                writer.WriteLine("</package>");
            }

            return null;
        }

        private void WriteOptionalAttribute(string name, string value, TextWriter writer)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.Write(" {0}='{1}'", name, value);
            }
        }
    }
}
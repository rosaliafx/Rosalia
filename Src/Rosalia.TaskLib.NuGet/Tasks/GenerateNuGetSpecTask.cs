namespace Rosalia.TaskLib.NuGet.Tasks
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;
    using Rosalia.TaskLib.NuGet.Input;
    using File = Rosalia.TaskLib.NuGet.Input.File;

    public class GenerateNuGetSpecTask : AbstractTask
    {
        private readonly IDictionary<string, string> _metadata = new Dictionary<string, string>();
        private readonly IList<Dependency> _dependencies = new List<Dependency>();
        private readonly IList<string> _references = new List<string>();
        private readonly IList<FrameworkAssembly> _frameworkAssemblies = new List<FrameworkAssembly>();
        private readonly IList<File> _files = new List<File>();
        private readonly IFile _destination;

        public GenerateNuGetSpecTask(IFile destination)
        {
            _destination = destination;
        }

        public IDictionary<string, string> Metadata
        {
            get { return _metadata; }
        }

        public IList<Dependency> Dependencies
        {
            get { return _dependencies; }
        }

        public IList<string> References
        {
            get { return _references; }
        }

        public IList<FrameworkAssembly> FrameworkAssemblies
        {
            get { return _frameworkAssemblies; }
        }

        public IList<File> Files
        {
            get { return _files; }
        }

        public IFile Destination
        {
            get { return _destination; }
        }

        public GenerateNuGetSpecTask Id(string id)
        {
            _metadata[Input.Metadata.Id] = id;
            return this;
        }

        public GenerateNuGetSpecTask Version(string version)
        {
            _metadata[Input.Metadata.Version] = version;
            return this;
        }

        public GenerateNuGetSpecTask Title(string title)
        {
            _metadata[Input.Metadata.Title] = title;
            return this;
        }

        public GenerateNuGetSpecTask Authors(params string[] authors)
        {
            _metadata[Input.Metadata.Authors] = string.Join(",", authors);
            return this;
        }

        public GenerateNuGetSpecTask Owners(params string[] owners)
        {
            _metadata[Input.Metadata.Owners] = string.Join(",", owners);
            return this;
        }

        public GenerateNuGetSpecTask Description(string description)
        {
            _metadata[Input.Metadata.Description] = description;
            return this;
        }

        public GenerateNuGetSpecTask ReleaseNotes(string releaseNotes)
        {
            _metadata[Input.Metadata.ReleaseNotes] = releaseNotes;
            return this;
        }

        public GenerateNuGetSpecTask Summary(string summary)
        {
            _metadata.Add(Input.Metadata.Summary, summary);
            return this;
        }

        public GenerateNuGetSpecTask Language(string localeId)
        {
            _metadata[Input.Metadata.Language] = localeId;
            return this;
        }

        public GenerateNuGetSpecTask Language(CultureInfo locale)
        {
            _metadata[Input.Metadata.Language] = locale.Name;
            return this;
        }

        public GenerateNuGetSpecTask ProjectUrl(string projectUrl)
        {
            _metadata[Input.Metadata.ProjectUrl] = projectUrl;
            return this;
        }

        public GenerateNuGetSpecTask IconUrl(string iconUrl)
        {
            _metadata[Input.Metadata.IconUrl] = iconUrl;
            return this;
        }

        public GenerateNuGetSpecTask LicenseUrl(string licenseUrl)
        {
            _metadata[Input.Metadata.LicenseUrl] = licenseUrl;
            return this;
        }

        public GenerateNuGetSpecTask Copyright(string copyright)
        {
            _metadata[Input.Metadata.Copyright] = copyright;
            return this;
        }

        public GenerateNuGetSpecTask RequireLicenseAcceptance(bool requireLicenseAcceptance)
        {
            _metadata[Input.Metadata.RequireLicenseAcceptance] = requireLicenseAcceptance.ToString().ToLower();
            return this;
        }

        public GenerateNuGetSpecTask Tags(params string[] tags)
        {
            _metadata[Input.Metadata.Tags] = string.Join(" ", tags);
            return this;
        }

        public GenerateNuGetSpecTask WithDependency(string id, string version = null, string frameworkVersion = null)
        {
            Dependencies.Add(new Dependency(id, version, frameworkVersion));
            return this;
        }

        /// <summary>
        /// Reads dependencies from packages.config file.
        /// </summary>
        public GenerateNuGetSpecTask WithDependenciesFromPackagesConfig(IDirectory projectDirectory)
        {
            var packagesConfigFile = projectDirectory.GetFile("packages.config");
            if (packagesConfigFile.Exists)
            {
                WithDependenciesFromPackagesConfig(packagesConfigFile);
            }

            return this;
        }

        /// <summary>
        /// Reads dependencies from packages.config file.
        /// </summary>
        public GenerateNuGetSpecTask WithDependenciesFromPackagesConfig(IFile packagesConfigFile)
        {
            using (var stream = packagesConfigFile.ReadStream)
            {
                var document = XDocument.Load(stream);
                foreach (var package in document.Descendants("package"))
                {
                    var versionAttribute = package.Attribute("version");
                    var frameworkVersionAttribute = package.Attribute("targetFramework");

                    WithDependency(
                        package.Attribute("id").Value,
                        versionAttribute == null ? null : versionAttribute.Value,
                        frameworkVersionAttribute == null ? null : frameworkVersionAttribute.Value);
                }
            }

            return this;
        }

        public GenerateNuGetSpecTask WithReference(string referenceDll)
        {
            References.Add(referenceDll);
            return this;
        }

        public GenerateNuGetSpecTask WithFrameworkAssembly(string assemblyName, string frameworkVersion = null)
        {
            FrameworkAssemblies.Add(new FrameworkAssembly(assemblyName, frameworkVersion));
            return this;
        }

        public GenerateNuGetSpecTask WithFile(string src, string target, string exclude = null)
        {
            Files.Add(new File(src, target, exclude));
            return this;
        }

        public GenerateNuGetSpecTask WithFile(IFile file, string target, string exclude = null)
        {
            return WithFile(file.AbsolutePath, target, exclude);
        }

        public GenerateNuGetSpecTask WithContentFiles(FileList files, string target = null)
        {
            if (target == null)
            {
                target = "content";
            }

            foreach (var file in files)
            {
                Files.Add(new File(file.AbsolutePath, Path.Combine(target, file.GetRelativePath(files.BaseDirectory)), null));
            }

            return this;
        }

        public GenerateNuGetSpecTask WithFiles(IEnumerable<IFile> files, string target, string exclude = null)
        {
            foreach (var file in files)
            {
                Files.Add(new File(file.AbsolutePath, target, exclude));
            }

            return this;
        }

        private void WriteOptionalAttribute(string name, string value, TextWriter writer)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.Write(" {0}='{1}'", name, value);
            }
        }

        protected override ITaskResult<Nothing> SafeExecute(TaskContext context)
        {
            using (var writer = new StreamWriter(Destination.WriteStream))
            {
                writer.WriteLine("<?xml version='1.0' encoding='utf-8'?>");
                writer.WriteLine("<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>");
                writer.WriteLine("  <metadata>");
                foreach (var metadata in Metadata)
                {
                    writer.WriteLine("    <{0}>{1}</{0}>", metadata.Key, metadata.Value);
                }

                if (Dependencies.Count > 0)
                {
                    writer.WriteLine("    <dependencies>");

                    var targetFrameworks = Dependencies.Select(x => x.TargetFramework).Distinct();
                    foreach (var targetFramework in targetFrameworks)
                    {
                        var framework = targetFramework;
                        writer.Write("      <group");
                        WriteOptionalAttribute("targetFramework", targetFramework, writer);
                        writer.WriteLine(">");

                        foreach (var dependency in Dependencies.Where(d => d.TargetFramework == framework))
                        {
                            writer.Write("        <dependency id='{0}'", dependency.Id);
                            WriteOptionalAttribute("version", dependency.Version, writer);
                            writer.WriteLine(" />");
                        }

                        writer.WriteLine("      </group>");
                    }

                    writer.WriteLine("    </dependencies>");
                }

                if (References.Count > 0)
                {
                    writer.WriteLine("    <references>");
                    foreach (var reference in References)
                    {
                        writer.WriteLine("      <reference file='{0}' />", reference);
                    }

                    writer.WriteLine("    </references>");
                }

                if (FrameworkAssemblies.Count > 0)
                {
                    writer.WriteLine("    <frameworkAssemblies>");
                    foreach (var frameworkAssembly in FrameworkAssemblies)
                    {
                        writer.Write("      <frameworkAssembly assemblyName='{0}'", frameworkAssembly.AssemblyName);
                        WriteOptionalAttribute("targetFramework", frameworkAssembly.TargetFramework, writer);
                        writer.WriteLine(" />");
                    }

                    writer.WriteLine("    </frameworkAssemblies>");
                }

                writer.WriteLine("  </metadata>");

                if (Files.Count > 0)
                {
                    writer.WriteLine("  <files>");
                    foreach (var file in Files)
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

            return Success;
        }
    }
}
﻿namespace Rosalia.TaskLib.NuGet.Input
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Linq;
    using Rosalia.Core.FileSystem;

    public class SpecInput
    {
        private readonly IDictionary<string, string> _metadata = new Dictionary<string, string>();
        private readonly IList<Dependency> _dependencies = new List<Dependency>();
        private readonly IList<string> _references = new List<string>();
        private readonly IList<FrameworkAssembly> _frameworkAssemblies = new List<FrameworkAssembly>();
        private readonly IList<File> _files = new List<File>();
        private IFile _destination;

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

        public SpecInput Id(string id)
        {
            _metadata[Input.Metadata.Id] = id;
            return this;
        }

        public SpecInput Version(string version)
        {
            _metadata[Input.Metadata.Version] = version;
            return this;
        }

        public SpecInput Title(string title)
        {
            _metadata[Input.Metadata.Title] = title;
            return this;
        }

        public SpecInput Authors(params string[] authors)
        {
            _metadata[Input.Metadata.Authors] = string.Join(",", authors);
            return this;
        }

        public SpecInput Owners(params string[] owners)
        {
            _metadata[Input.Metadata.Owners] = string.Join(",", owners);
            return this;
        }

        public SpecInput Description(string description)
        {
            _metadata[Input.Metadata.Description] = description;
            return this;
        }

        public SpecInput ReleaseNotes(string releaseNotes)
        {
            _metadata[Input.Metadata.ReleaseNotes] = releaseNotes;
            return this;
        }

        public SpecInput Summary(string summary)
        {
            _metadata.Add(Input.Metadata.Summary, summary);
            return this;
        }

        public SpecInput Language(string localeId)
        {
            _metadata[Input.Metadata.Language] = localeId;
            return this;
        }

        public SpecInput Language(CultureInfo locale)
        {
            _metadata[Input.Metadata.Language] = locale.Name;
            return this;
        }

        public SpecInput ProjectUrl(string projectUrl)
        {
            _metadata[Input.Metadata.ProjectUrl] = projectUrl;
            return this;
        }

        public SpecInput IconUrl(string iconUrl)
        {
            _metadata[Input.Metadata.IconUrl] = iconUrl;
            return this;
        }

        public SpecInput LicenseUrl(string licenseUrl)
        {
            _metadata[Input.Metadata.LicenseUrl] = licenseUrl;
            return this;
        }

        public SpecInput Copyright(string copyright)
        {
            _metadata[Input.Metadata.Copyright] = copyright;
            return this;
        }

        public SpecInput RequireLicenseAcceptance(bool requireLicenseAcceptance)
        {
            _metadata[Input.Metadata.RequireLicenseAcceptance] = requireLicenseAcceptance.ToString().ToLower();
            return this;
        }

        public SpecInput Tags(params string[] tags)
        {
            _metadata[Input.Metadata.Tags] = string.Join(" ", tags);
            return this;
        }

        public SpecInput WithDependency(string id, string version = null, string frameworkVersion = null)
        {
            Dependencies.Add(new Dependency(id, version, frameworkVersion));
            return this;
        }

        /// <summary>
        /// Reads dependencies from packages.config file.
        /// </summary>
        public SpecInput WithDependenciesFromPackagesConfig(IDirectory projectDirectory)
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
        public SpecInput WithDependenciesFromPackagesConfig(IFile packagesConfigFile)
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

        public SpecInput WithReference(string referenceDll)
        {
            References.Add(referenceDll);
            return this;
        }
        
        public SpecInput WithFrameworkAssembly(string assemblyName, string frameworkVersion = null)
        {
            FrameworkAssemblies.Add(new FrameworkAssembly(assemblyName, frameworkVersion));
            return this;
        }

        public SpecInput WithFile(string src, string target, string exclude = null)
        {
            Files.Add(new File(src, target, exclude));
            return this;
        }

        public SpecInput WithFile(IFile file, string target, string exclude = null)
        {
            return WithFile(file.AbsolutePath, target, exclude);
        }

        public SpecInput WithContentFiles(FileList files)
        {
            foreach (var file in files)
            {
                Files.Add(new File(file.AbsolutePath, file.GetRelativePath(files.BaseDirectory), null));    
            }
            
            return this;
        }

        public SpecInput WithFiles(IEnumerable<IFile> files, string target, string exclude = null)
        {
            foreach (var file in files)
            {
                Files.Add(new File(file.AbsolutePath, target, exclude));    
            }
            
            return this;
        }

        public SpecInput ToFile(IFile destination)
        {
            _destination = destination;
            return this;
        }
    }
}
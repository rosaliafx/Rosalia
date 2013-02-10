# NuGet Tasks #

<h2 class='task'>GenerateNuGetSpecTask</h2>

Generates NuGet package specification by user-provided data (package id, dependencies etc) and saves it to a file.

<h3 class="input">Input</h3>

* NuGet package metadata (see [reference][nugetReferenceMetadata]);
* NuGet dependencies (see [reference][nugetReferenceDependencies]);
* NuGet references (see [reference][nugetReferenceAssemblyReferences]);
* NuGet framework assembly references (see [reference][nugetReferenceFrameworkAssemblyReferences]);
* files to include in the package.

### Examples ###

A task that generates a NuGet spec for Rosalia.Core package:

    new GenerateNuGetSpecTask<BuildRosaliaContext>((c, input) =>
        input
            .Id("Rosalia.Core")                                      // package metadata 
            .Version(context.Data.Version)                           // ...
            .ProjectUrl("https://github.com/guryanovev/Rosalia")     // ...
            .Tags("automation", "build", "msbuild", "nant", "psake") // ...
            .Description("Core libs for Rosalia framework.")         // ...
            .WithFiles(c.GetCoreLibFiles(), "lib")                   // files to include in the package
            .ToFile(c.Data.NuSpecRosaliaCore))                       // destination file

Different ways to specify dependencies:

    new GenerateNuGetSpecTask<BuildRosaliaContext>((c, input) => input
        .Id("Rosalia")
            // more options go here
        .WithDependency("Rosalia.Core", c.Data.Version)       // simple dependency
        .WithDependency("NuGetPowerTools", version: "0.29")   // simple dependency
        .WithDependenciesFromPackagesConfig(projectDirectory) // this method reads ALL dependencies from packages.config file
        .ToFile(c.Data.NuSpecRosalia))

<h2 class='task'>GeneratePackageTask</h2>

Generates NuGet package by specification.

<h3 class="input">Input</h3>

* path to nuspec file (required).

### Examples ###

<pre><code class="cs">new GeneratePackageTask<BuildRosaliaContext>(file)</code></pre>

<h2 class='task'>PushPackageTask</h2>

Pushes a package to NuGet Gallery.

<h3 class="input">Input</h3>

* path to nupkg file (required);
* API-Key (optional, for private workflows only);
* push command options (Source, Timeout and Verbosity);

### Examples ###

<pre><code class="cs">new PushPackageTask<Context>(new PushInput().Package(file))</code></pre>
    
[nugetReferenceMetadata]: http://docs.nuget.org/docs/reference/nuspec-reference#Metadata_Section
[nugetReferenceDependencies]: http://docs.nuget.org/docs/reference/nuspec-reference#Specifying_Dependencies
[nugetReferenceAssemblyReferences]: http://docs.nuget.org/docs/reference/nuspec-reference#Specifying_Explicit_Assembly_References
[nugetReferenceFrameworkAssemblyReferences]: http://docs.nuget.org/docs/reference/nuspec-reference#Specifying_Framework_Assembly_References_(GAC)
# MSBuild Tasks #

<h2 class='task'>MsBuildTask</h2>

Runs MSBuild.exe with specified arguments. 
See [MSBuild Command-Line reference](http://msdn.microsoft.com/en-us/library/vstudio/ms164311.aspx) for configuration details.

<h3 class="input">Input</h3>

* Project file;
* MSBuild switches;

### Examples ###

A task that generates a NuGet spec for Rosalia.Core package:

<pre><code class="cs">new MsBuildTask<Context>()
    .FillInput(c => new MsBuildInput()
        .WithProjectFile(c.Data.SolutionFile)
        .WithConfiguration(c.Data.Configuration))
</code></pre>

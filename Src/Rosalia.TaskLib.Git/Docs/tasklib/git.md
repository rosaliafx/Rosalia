# Git Tasks #

All git tasks allow to configure a path to git executable.
It could be done by setting `GitToolPath` property or by calling `WithGitToolPath` method of input object:

    new SomeGitTask(new GitInput()
		.WithGitToolPath("C:\\MyGitInstallationDirectory\\bin"));

This approach is not always sufficient because in general a workflow should not be wired to environment things like absolute path.
You have a few options to avoid it:

1. Include git bin directory to `PATH` environment variable.
2. Specify GIT_HOME environment variable with path to your git installation directory.
3. If your git is installed in `Programm Files\Git` or `Program Files (x86)` it will be found automatically.

In all three cases you should omit setting `GitToolPath` variable.

<div class='alert'>
	<code>GIT_HOME</code> could point to <code>GIT_INSTALL_DIRECTORY</code> or to <code>GIT_INSTALL_DIRECTORY\bin</code> and it should not include <code>git.exe</code> part.
</div>

## GetVersion ##

Extracts version info from a git repository using a command:

    git describe --tags

It is required for this task that a tag was created earlier. Use this command to create a tag:

    git tag -a [VERSION] -m "[TAG MESSAGE]"

Make sure you pushed your tag so it could be visible globally:
    
	git push --tags to share tags
    
### Input ###

### Output ###
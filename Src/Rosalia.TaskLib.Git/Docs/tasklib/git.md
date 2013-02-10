# Git Tasks #

Git tasks call `git.exe` tool with specified arguments.

Threre are a few ways to configure a path to git executable:

1. explicitly specify a path: `ToolPath = "C:\Git\bin"`;
2. include git bin directory to `PATH` environment variable;
3. specify GIT_HOME environment variable with a path to your git installation directory;
4. if your git is installed in `Program Files\Git` or `Program Files (x86)` it will be found automatically.

<div class='alert'>
	<code>GIT_HOME</code> could point to <code>GIT_INSTALL_DIRECTORY</code> or to <code>GIT_INSTALL_DIRECTORY\bin</code> and it should not include <code>git.exe</code> part.
</div>

<h2 class='task'>GetVersion</h2>

Extracts version info from a git repository using a command:

    git describe --tags

It is required for this task that a tag was created earlier. Use this command to create a tag:

    git tag -a [VERSION] -m "[TAG MESSAGE]"

Make sure you pushed your tag so it could be visible globally:
    
	git push --tags to share tags
    
<h3 class="input">Input</h3>

* path to git executable (optional);
* working directory (optional). Current directory is used if no working directory provided.

<h3 class="output">Output</h3>

An instance of `GetVersionOutput` class.

<table class="table table-bordered">
	<tr>
		<td class="property">Tag</td>
		<td class="type">string</td>
		<td>Tag text.</td>
	</tr>
	<tr>
		<td class="property">CommitsCount</td>
		<td class="type">int</td>
		<td>Count of commits since the tag was added.</td>
	</tr>
	<tr>
		<td class="property">LastCommitKey</td>
		<td class="type">string</td>
		<td>Abbreviated object name of the most recent commit.</td>
	</tr>
</table>

<h2 class='task'>GitCommandTask</h2>

Calls `git.exe` with specified arguments.

<h3 class="input">Input</h3>

* path to git executable (optional);
* working directory (optional). Current directory is used if no working directory provided.

### Example ###

    //// Do auto commit to gh-pages repo
    new GitCommandTask<BuildRosaliaContext>
    {
        InputProvider = context => new GitInput
        {
            RawCommand = string.Format("commit -a -m \"Docs auto commit v{0}\"", context.Data.Version),
            WorkDirectory = new DefaultDirectory(context.Data.PrivateData.GhPagesRoot)
        }
    }
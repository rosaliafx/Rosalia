# Rosalia #

Rosalia is a concurrency-aware build automation tool that utilize monadic query-comprehension syntax for writing tasks in concise and strongly-typed manner using C#.

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/rosaliafx/Rosalia?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

## Why?

* use the same language for both main codebase and build script
* get debugging, refactoring and testing tools to work for build "scripts" out of the box
* utilize any third-party .NET library for build task purposes

## Quick example

```C#
var fooTask = Task(                   /**********************/
    "fooTask",                        /* define a task as   */
    () => {                           /* a simple action... */
        // do something here          /**********************/
    });

var barTask = Task(                   /**********************/
    "barTask",                        /* ..or use a func if */
    () => {                           /* you need to return */
        return "bar".AsTaskResult();  /* a result...        */
    });                               /**********************/

var bazTask = Task(                   /**********************/
	"bazTask",                        /* ...or use a class  */
	new MyCustomTask());              /* to encapsulate     */
                                      /* task logic         */
                                      /**********************/

var mainTask = Task(                  /***********************************************/
    "mainTask",                       /* Use Linq query-comprehension to fetch       */
    from barResult in barTask         /* results from prior tasks (actually monads)  */
    from bazResult in bazTask         /* and define dependencies at the same time.   */
    select new MyMainTask(            /***********************************************/
        barResult,                 
        bazResult).AsTask(),

    Default(),                        /* This task is default */
    DependsOn(fooTask));              /* Add one more dependency manually */
```

- **[Getting Started with Rosalia &rarr;](https://github.com/rosaliafx/Rosalia/wiki/Getting-Started)**

---

[![Rosalia uses Rosalia to build itself on Travis CI](https://travis-ci.org/rosaliafx/Rosalia.svg?branch=master)](https://travis-ci.org/rosaliafx/Rosalia)
[![NuGet package](https://img.shields.io/nuget/vpre/Rosalia.svg)](https://www.nuget.org/packages/Rosalia/)
[![Build status](https://ci.appveyor.com/api/projects/status/wpj45p2yw44lkkjd/branch/master?svg=true)](https://ci.appveyor.com/project/rosaliafx/rosalia/branch/master)

---

## Explore Wiki

<ul>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Home">Home</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Getting-Started">Getting Started</a></li>
</ul>

<p><strong>Writing Tasks</strong></p>

<ul>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Creating-a-Workflow">Creating a Workflow</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Defining-Tasks">Defining Tasks</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Share-state-accross-tasks">Share State accross Tasks</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Mastering-Dependencies">Mastering Dependencies</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Using-result-transformers">Using result transformers</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Using-Subflows-to-organize-tasks">Using Subflows to organize tasks</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Tasks-Preconditions">Tasks Preconditions</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Recovering-failure-results">Recovering failure results</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/Declaring-dynamic-tasks" style="color: #FF0000"><code>absent</code> Declaring dynamic tasks</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/Creating-Custom-Tasks"><code>absent</code> Creating Custom Tasks</a></li>
</ul>

<p><strong>Running Tasks</strong></p>

<ul>
<li><a href="https://github.com/rosaliafx/Rosalia/wiki/Rosalia.exe-command-line-reference">Rosalia.exe command line</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Passing-Properties-to-Workflow">Passing Properties to Workflow</a></li>
<li>
<a class="internal present" href="/rosaliafx/Rosalia/wiki/Continuous-Integration">Continuous Integration</a>

<ul>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/Travis-CI">Travis CI</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/Team-City"><code>absent</code> Team City</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/AppVeyor"><code>absent</code> AppVeyor</a></li>
</ul>
</li>
</ul>

<p><strong>API</strong></p>

<ul>
<li>
<a class="internal present" href="/rosaliafx/Rosalia/wiki/File-System">File System</a>

<ul>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/IFile">IFile</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/IDirectory">IDirectory</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/FileList">FileList</a></li>
</ul>
</li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/TaskContext"><code>absent</code> TaskContext</a></li>
</ul>

<p><strong>Tasklib</strong></p>

<ul>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/AssemblyInfo">AssemblyInfo</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/Compression"><code>absent</code> Compression</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/Git"><code>absent</code> Git</a></li>
<li><a class="internal present" href="/rosaliafx/Rosalia/wiki/MsBuild">MsBuild</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/NuGet"><code>absent</code> NuGet</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/Standard"><code>absent</code> Standard</a></li>
<li><a class="internal absent" href="/rosaliafx/Rosalia/wiki/Svn"><code>absent</code> Svn</a></li>
</ul>        

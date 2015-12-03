# Rosalia #

[![Join the chat at https://gitter.im/rosaliafx/Rosalia](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/rosaliafx/Rosalia?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Rosalia is a concurrency-aware build automation tool that utilize monadic query-comprehension syntax for writing tasks in concise and strongly-typed manner using C#.

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
![](https://img.shields.io/nuget/vpre/Rosalia.svg)
[![Build status](https://ci.appveyor.com/api/projects/status/wpj45p2yw44lkkjd/branch/master?svg=true)](https://ci.appveyor.com/project/rosaliafx/rosalia/branch/master)

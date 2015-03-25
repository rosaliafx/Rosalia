# Rosalia #

Rosalia is a concurrency-aware build automation tool that utilize monadic query-comprehension syntax to allow writing tasks in concise and strongly-typed manner using C#.

* use the same language for both main codebase and build script
* get debugging, refactoring and testing tools to work for build "scripts" out of the box
* utilize any third-party .NET library for build task purposes

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

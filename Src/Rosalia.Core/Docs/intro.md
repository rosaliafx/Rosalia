# Introduction #

Rosalia is an open source automation framework that allows defining tasks and workflows in pure C#.

## Quick start ##

For quick start just do 2 steps:

* create a class library project in your solution;
* install Rosalia NuGet package:

<pre>
	<code class='bash'>Package-Install "Rosalia"</code>
</pre>

You get two sample classes in your project:

    public class Context
    {
        public string Message { get; set; }
    }

	public class MainWorkflow : Workflow<Context>
    {
        public override ITask<Context> RootTask
        {
            get
            {
                return Sequence(
                    //// Task 1
                    Task((result, c) => result.AddInfo("Hello...")),

                    //// Task 2
                    Task((result, c) => c.Data.Message = "...world"),

                    //// Task 3
                    Task((result, c) => result.AddInfo(c.Data.Message))
                );
            }
        }
    }

Now you can run this sample workflow:

1. Right-click on your project in Solution Explorer;
2. Select `Debug -> Start New Instance`
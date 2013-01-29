# Introduction #

Rosalia is an open source automation framework. Its main goal is to allow writing tasks definition in C#. So, with Rosalia you 

* do not use any foreign DSL (no XML like in MSBuild or NAnt);
* do not use any foreign language/platform (like Ruby for rake);
* define tasks and workflows in C#;

## Quick start ##

For quick start just do 2 steps:

* create a class library project in your solution;
* install Rosalia NuGet package:

<pre>
	<code class='bash'>Package-Install "Rosalia"</code>
</pre>

You will get 2 sample classes in your project:

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

## How it works ##

[TBD]

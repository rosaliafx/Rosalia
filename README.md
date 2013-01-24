# Rosalia #

Rosalia is an automation tool written in C#. It could be used for

- build automation;
- install automation;
- ...any other kind of automation

## Features ##

- uses MSBuild and native csproj/sln files for direct compiling -- no custom compiler calls;
- tasks are defined in pure C#. Debug, refactoring, unit testing and all the features of IDE are available;
- you can use standard task or define your own as normal C# classes;
- any external .NET library can be used in task body;
- use NuGet for quick start and tasks importing;

## Quick Start ##

1. Create a Class Library project in your solution.
2. Add [Rosalia NuGet package](https://nuget.org/packages/Rosalia) to this project:

  **PM> Install-Package Rosalia**

That is it. Now your Class Library could be ran (Debug->Start New Instance) and a sample workflow was created for you.

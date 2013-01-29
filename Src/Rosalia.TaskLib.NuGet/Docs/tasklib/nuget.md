# NuGet Tasks #

## GenerateNuGetSpecTask ##

Generates NuGet package specification by user-provided data (package id, dependencies etc) and saves it to a file.

    new GenerateNuGetSpecTask<MyContext>(c => new SpecInput()
		.Id("MyPackageId")
		/* more options here */);

### Examples ###

[TBD]

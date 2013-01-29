# NuGet Tasks #

## GenerateNuGetSpecTask ##

Generates NuGet package specification by user-provided data (package id, dependencies etc) and saves it to a file.

    new GenerateNuGetSpecTask<MyContext>(c => new SpecInput()
		.Id("MyPackageId")
		/* more options here */);

<!-- comment -->

<table>
	<tr>
		<td>1</td>
		<td>2</td>
	</tr>
	<tr>
		<td>3</td>
		<td>4</td>
	</tr>
</table>

### Examples ###


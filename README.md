# Simple .NET assembly changer

This program converts the original .NET assembly, replacing all addition operations (`OpCodes.Add`) with subtraction
operations (`OpCodes.Sub`), using the [Mono.Cecil](https://github.com/jbevain/cecil) library. The resulting assembly is
saved along the path.

## Usage

Run from project root directory

```shell
$ dotnet run --project ./AssemblyChanges/AssemblyChanges/AssemblyChanges.csproj -- pathToAssembly pathToNewDirectory
```

or from build directory with assembly

```shell
$ dotnet AssemblyChanges.dll -- pathToAssembly pathToNewDirectory
```

NOTE: when passing a file as the second argument, and not a directory, then the directory in which the file is located
will be selected.

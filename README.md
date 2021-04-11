# Simple .NET assembly changer

This program converts the original .NET assembly, replacing all addition operations (`OpCodes.Add`) with subtraction
operations (`OpCodes.Sub`), using the [Mono.Cecil](https://github.com/jbevain/cecil) library. The resulting assembly is
saved to the new path.

## Solutions

- [AssemblyChanges](https://github.com/PaGr0m/dotnet-assembly-changes/tree/main/AssemblyChanges/AssemblyChanges) - The
  main logic for working with CIL (Common Intermediate Language)
- [ConsoleApplication](https://github.com/PaGr0m/dotnet-assembly-changes/tree/main/AssemblyChanges/ConsoleApplication) -
  Simple console application with outputting the result of methods
- [TestApplication](https://github.com/PaGr0m/dotnet-assembly-changes/tree/main/AssemblyChanges/TestApplication) -
  Application for testing basic logic using NUnit

## Usage

Run from project root directory:

```shell
$ dotnet run --project ./AssemblyChanges/AssemblyChanges/AssemblyChanges.csproj -- pathToAssembly pathToNewDirectory
```

Run from build directory with assembly:

```shell
$ dotnet AssemblyChanges.dll -- pathToAssembly pathToNewDirectory
```

NOTE: when passing a file as the second argument (not a directory), then the resulting directory will be selected.

using System;
using System.IO;
using CommandLine;

namespace AssemblyChanges
{
    public class InternshipTask
    {
        private class Options
        {
            [Value(
                0,
                Required = true,
                MetaName = "Assembly file.",
                HelpText = "Absolute path to C# assembly."
            )]
            public string AssemblyFile { get; set; }

            [Value(
                1,
                Required = true,
                MetaName = "Resulting file.",
                HelpText = "Absolute path to the directory where the new assembly will be saved.")
            ]
            public string ResultingFile { get; set; }
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(
                    _ => Console.WriteLine(
                        "You must pass the path to the .NET assembly and the path to the resulting file!"
                    )
                ).WithParsed(
                    options => RunProgram(options.AssemblyFile, options.ResultingFile)
                );
        }

        private static void RunProgram(string assemblyFile, string resultingFile)
        {
            Utils.Validation(assemblyFile, ref resultingFile);
            Utils.CopyAssembly(assemblyFile, resultingFile);

            var copyAssemblyFile = Path.Combine(resultingFile, Path.GetFileName(assemblyFile));
            AssemblyChanger.ChangeInstructionsAddToSubtract(copyAssemblyFile);
        }
    }
}
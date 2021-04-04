using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace AssemblyChanges
{
    class Program
    {
        private const string Path =
            "/home/pagrom/University/internships/dotnet-assembly-changes/AssemblyChanges/ConsoleApp1/bin/Debug/net5.0/ConsoleApp1.dll";

        private const string Path2 =
            "/home/pagrom/University/internships/dotnet-assembly-changes/Changes.dll";

        private static readonly MethodInfo DecimalSubtractMethodInfo = typeof(decimal).GetMethod(
            "Subtract", new[] {typeof(decimal), typeof(decimal)}
        );

        static void Main(string[] args)
        {
            PrintTypes(Path, Path2);
        }

        // TODO: call System.Decimal System.Decimal::op_Addition(System.Decimal,System.Decimal)
        // TODO: результирующий файл не как DLL
        public static void PrintTypes(string fileName, string fileName2)
        {
            // var decimalSubtractMethodRef = module.ImportReference(methodInfo);
            // var newInstruction = processor.Create(OpCodes.Call, decimalSubtractMethodRef);

            var subOp = Instruction.Create(OpCodes.Sub);

            // var newModule = ModuleDefinition.CreateModule(fileName2); // TODO: create module

            var module = ModuleDefinition.ReadModule(fileName);
            var decimalSubtractMethodRef = module.ImportReference(DecimalSubtractMethodInfo);

            // - TODO: ModuleDefinition.Types now only returns top level (not nested) types.
            //  If you want to iterate over all the types defined in an assembly,
            //  you can use the method ModuleDefinition.GetTypes().
            foreach (var type in module.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    var processor = method.Body.GetILProcessor();

                    // Collect instructions to replace
                    var simpleInstructions = new List<Instruction>();
                    var decimalInstructions = new List<Instruction>();
                    foreach (var instruction in processor.Body.Instructions)
                    {
                        if (instruction.OpCode == OpCodes.Add)
                        {
                            simpleInstructions.Add(instruction);
                        }

                        if (instruction.OpCode == OpCodes.Call &&
                            instruction.Operand.ToString() ==
                            "System.Decimal System.Decimal::op_Addition(System.Decimal,System.Decimal)")
                        {
                            decimalInstructions.Add(instruction);
                        }
                    }

                    simpleInstructions.ForEach(it => processor.Replace(it, subOp));
                    decimalInstructions.ForEach(it =>
                        processor.Replace(it, processor.Create(OpCodes.Call, decimalSubtractMethodRef)));
                }
            }

            module.Write(fileName + "1");
        }
    }
}
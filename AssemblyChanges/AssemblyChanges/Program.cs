using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// TODO:
//  1. Сделать поддержку не только DLL, еще и EXE и сделать проверку на валидность.
//  2. Подумать над тем как модуль тянуть. Как asm.MainModule или как сейчас.
//  3. Создавать новый module или старый как-то правильно сохранить надо?
//  4. C# docs?
//  5. Понять, можно ли обойти в конце проверку на строку.
//  6. Добавить тесты.
//  7. Еще почитать о "<Module>"
//  8. Refactoring
namespace AssemblyChanges
{
    class Program
    {
        private static readonly MethodInfo DecimalSubtractMethodInfo = typeof(decimal).GetMethod(
            "Subtract", new[] {typeof(decimal), typeof(decimal)}
        );

        private static readonly Instruction SubtractInstruction = Instruction.Create(OpCodes.Sub);

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException(
                    "You must pass the path to the .NET assembly and the path to the resulting file."
                );
            }

            RebuildAssembly(args[0], args[1]);
        }

        // TODO: результирующий файл не как DLL
        public static void RebuildAssembly(string assemblyFile, string resultingFile)
        {
            // TODO: AssemblyDefinition or ModuleDefinition
            using var module = ModuleDefinition.ReadModule(
                assemblyFile
                // , new ReaderParameters {ReadWrite = true}
            );

            var decimalSubtractMethodRef = module.ImportReference(DecimalSubtractMethodInfo);

            /// <mark>
            /// ModuleDefinition.Types now only returns top level (not nested) types.
            /// If you want to iterate over all the types defined in an assembly,
            /// you can use the method ModuleDefinition.GetTypes().
            /// </mark>
            foreach (var type in module.GetTypes())
            {
                if (type.Name == "<Module>")
                {
                    continue;
                }

                foreach (var method in type.GetMethods())
                {
                    ReplaceInstructions(method, decimalSubtractMethodRef);
                }
            }

            // TODO: fix видимо необходимо подвязать зависимости
            module.Write(resultingFile);
        }

        private static void ReplaceInstructions(
            MethodDefinition methodDefinition,
            MethodReference decimalSubtractMethodRef
        )
        {
            var processor = methodDefinition.Body.GetILProcessor();

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

            simpleInstructions.ForEach(it => processor.Replace(it, SubtractInstruction));
            decimalInstructions.ForEach(it =>
                processor.Replace(it, processor.Create(OpCodes.Call, decimalSubtractMethodRef))
            );
        }
    }
}
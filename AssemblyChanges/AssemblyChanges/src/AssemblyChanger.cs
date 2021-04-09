using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace AssemblyChanges
{
    public static class AssemblyChanger
    {
        private static readonly MethodInfo DecimalSubtractMethodInfo = typeof(decimal).GetMethod(
            "Subtract", new[] {typeof(decimal), typeof(decimal)}
        );

        private static readonly MethodInfo DecimalAddMethodInfo = typeof(decimal).GetMethod(
            "op_Addition", new[] {typeof(decimal), typeof(decimal)}
        );

        private static readonly Instruction SubtractInstruction = Instruction.Create(OpCodes.Sub);

        public static void ChangeInstructionsAddToSubtract(string assemblyFile)
        {
            using var module = ModuleDefinition.ReadModule(
                assemblyFile,
                new ReaderParameters {ReadWrite = true}
            );
            var decimalSubtractMethodRef = module.ImportReference(DecimalSubtractMethodInfo);
            var decimalAddMethodRef = module.ImportReference(DecimalAddMethodInfo);

            /*
             * ModuleDefinition.Types now only returns top level (not nested) types.
             * If you want to iterate over all the types defined in an assembly,
             * you can use the method ModuleDefinition.GetTypes().
             */
            foreach (var type in module.GetTypes())
            {
                if (type.Name == "<Module>") continue;

                /* 
                 * TypeDefinition.Constructors is merged inside TypeDefinition.Methods.
                 * It was a Cecil thing, and it was breaking the order in which methods are defined in the type.
                 */
                foreach (var method in type.GetMethods())
                {
                    ReplaceInstructions(method, decimalSubtractMethodRef, decimalAddMethodRef);
                }
            }

            module.Write();
        }

        private static void ReplaceInstructions(
            MethodDefinition methodDefinition,
            MethodReference decimalSubtractMethodRef,
            MemberReference decimalAddMethodRef
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

                if (instruction.OpCode == OpCodes.Call)
                {
                    // Check: instruction == System.Decimal::op_Addition(System.Decimal,System.Decimal)
                    if (instruction.Operand is MethodReference methodRef &&
                        string.Equals(
                            methodRef.FullName,
                            decimalAddMethodRef.FullName,
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        decimalInstructions.Add(instruction);
                    }
                }
            }

            simpleInstructions.ForEach(it => processor.Replace(it, SubtractInstruction));
            decimalInstructions.ForEach(it =>
                processor.Replace(it, processor.Create(OpCodes.Call, decimalSubtractMethodRef))
            );
        }
    }
}
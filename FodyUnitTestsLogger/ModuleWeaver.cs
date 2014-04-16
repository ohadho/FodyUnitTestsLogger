using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace FodyUnitTestsLogger
{
    public class ModuleWeaver
    {
        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            MethodsFinder methodsFinder = new MethodsFinder(ModuleDefinition, "TestFixtureAttribute", "Test");
            IEnumerable<MethodDefinition> methodDefinitions = methodsFinder.GetMethods();
            
            foreach (var methodDefinition in methodDefinitions)
            {
                InjectCodeToMethod(methodDefinition);
            }
        }

        private void InjectCodeToMethod(MethodDefinition methodDefinition)
        {
            Collection<Instruction> instructions = methodDefinition.Body.Instructions;

            MethodReference getCurrentMethodRef = GetMethodReference(typeof (MethodBase), "GetCurrentMethod", new Type[] {});
            MethodReference writeLineReference = GetMethodReference(typeof (Console), "WriteLine", new Type[] {typeof (object)});

            Instruction instructionGetCurrentMethod = Instruction.Create(OpCodes.Call, getCurrentMethodRef);
            Instruction instructionWriteLine = Instruction.Create(OpCodes.Call, writeLineReference);

            instructions.Insert(0, instructionGetCurrentMethod);
            instructions.Insert(1, instructionWriteLine);
        }

        private MethodReference GetMethodReference(Type type, string methodName,  Type[] arguments)
        {            
            return ModuleDefinition.Import(type.GetMethod(methodName, arguments));
        }
    }
}

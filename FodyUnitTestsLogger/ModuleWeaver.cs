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
            IEnumerable<MethodDefinition> testMethods = GetTestMethods();

            foreach (MethodDefinition methodDefinition in testMethods)
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

        private IEnumerable<MethodDefinition> GetTestMethods()
        {
            var testMethods = new List<MethodDefinition>();

            foreach (TypeDefinition typeDefinition in ModuleDefinition.Types)
            {                
                if (IsTestClass(typeDefinition))
                {
                    testMethods.AddRange(GetTestMethodsFromType(typeDefinition));
                }
            }

            return testMethods;
        }

        private IEnumerable<MethodDefinition> GetTestMethodsFromType(TypeDefinition typeDefinition)
        {
            var methods = new List<MethodDefinition>();
            foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
            {
                if (IsTestMethod(methodDefinition))
                {
                    methods.Add(methodDefinition);
                }
            }

            return methods;
        }

        private bool IsTestMethod(MethodDefinition methodDefinition)
        {
            if (methodDefinition.CustomAttributes.Any(attribute => attribute.AttributeType.Name.Contains("Test")))
            {
                return true;
            }

            return false;
        }

        private bool IsTestClass(TypeDefinition typeDefinition)
        {
            IEnumerable<CustomAttribute> attributes = typeDefinition.CustomAttributes.Where(attribute => attribute.AttributeType.FullName.Contains("TestFixtureAttribute"));
            if (attributes.Any())
            {
                return true;
            }

            return false;
        }
    }
}

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
            List<MethodDefinition> testMethods = GetTestMethods();

            foreach (MethodDefinition methodDefinition in testMethods)
            {
                Collection<Instruction> instructions = methodDefinition.Body.Instructions;
                
                MethodReference getCurrentMethodRef = ModuleDefinition.Import(typeof (MethodBase).GetMethod("GetCurrentMethod", new Type []{}));
                MethodReference writeLineReference = ModuleDefinition.Import(typeof (Console).GetMethod("WriteLine", new Type[] {typeof (object)}));

                Instruction instructionGetCurrentMethod = Instruction.Create(OpCodes.Call, getCurrentMethodRef);                
                Instruction instructionWriteLine = Instruction.Create(OpCodes.Call, writeLineReference);
                instructions.Insert(0, instructionGetCurrentMethod);
                instructions.Insert(1, instructionWriteLine);
            }
        }
        
        private List<MethodDefinition> GetTestMethods()
        {
            var res = new List<MethodDefinition>();

            foreach (TypeDefinition typeDefinition in ModuleDefinition.Types)
            {                
                if (IsTestClass(typeDefinition))
                {
                    foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                    {
                        if (IsTestMethod(methodDefinition))
                        {
                            res.Add(methodDefinition);
                        }
                    }                    
                }
            }

            return res;
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

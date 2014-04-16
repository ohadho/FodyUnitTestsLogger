using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace FodyUnitTestsLogger
{
    public class MethodsFinder
    {
        private ModuleDefinition _moduleDefinition;
        private readonly string _typeAttribute;
        private readonly string _methodAttribute;

        public MethodsFinder(ModuleDefinition moduleDefinition, string typeAttribute, string methodAttribute)
        {
            _moduleDefinition = moduleDefinition;
            _typeAttribute = typeAttribute;
            _methodAttribute = methodAttribute;
        }

        public IEnumerable<MethodDefinition> GetMethods()
        {
            List<MethodDefinition> methods = new List<MethodDefinition>();
            foreach (var typeDefinition in _moduleDefinition.Types)
            {
                if (TypeHasAttribute(typeDefinition))
                {
                    methods.AddRange(GetMethodsWithAttribute(typeDefinition));
                }
            }

            return methods;
        }

        private IEnumerable<MethodDefinition> GetMethodsWithAttribute(TypeDefinition typeDefinition)
        {
            IEnumerable<MethodDefinition> methods = typeDefinition.Methods.Where(methodDefinition => MethodHasAttribute(methodDefinition));
            return methods;
        }

        private bool MethodHasAttribute(MethodDefinition methodDefinition)
        {
            if (methodDefinition.CustomAttributes.Where(
                    attribute => attribute.AttributeType.FullName.Contains(_methodAttribute)).Any())
            {
                return true;
            }

            return false;
        }

        private bool TypeHasAttribute(TypeDefinition typeDefinition)
        {            
            if (typeDefinition.CustomAttributes.Where(attribute => attribute.AttributeType.FullName.Contains(_typeAttribute)).Any())
            {
                return true;
            }

            return false;
        }
    }
}
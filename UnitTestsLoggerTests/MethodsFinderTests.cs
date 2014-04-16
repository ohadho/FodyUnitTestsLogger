using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess;
using FodyUnitTestsLogger;
using Mono.Cecil;
using NUnit.Framework;

namespace UnitTestsLoggerTests
{
    [TestFixture]
    public class MethodsFinderTests
    {
        private ModuleWeaver moduleWeaver;
        
        [Test]
        public void GetMethods_ClassWithTest_GetAllTestMethods()
        {
            string path = typeof(DummyTests).Assembly.Location;
            ModuleDefinition module = ModuleDefinition.ReadModule(path);

            MethodsFinder finder = new MethodsFinder(module, "TestFixture", "Test");
            IEnumerable<MethodDefinition> methodDefinitions = finder.GetMethods();

            Assert.AreEqual(2, methodDefinitions.Count());
        }

        [Test]
        public void GetMethods_ClassWithTest_CorrectMethodsAreReturned()
        {
            string path = typeof(DummyTests).Assembly.Location;
            ModuleDefinition module = ModuleDefinition.ReadModule(path);

            MethodsFinder finder = new MethodsFinder(module, "TestFixture", "Test");
            IEnumerable<MethodDefinition> methodDefinitions = finder.GetMethods();

            Assert.AreEqual(1, methodDefinitions.Where(definition => definition.Name == "AddTest").Count());
            Assert.AreEqual(1, methodDefinitions.Where(definition => definition.Name == "AddTest2").Count());
        }

        [Test]
        public void GetMethods_ClassWithTest_DoesNotReturnMethodWithoutAttribute()
        {
            string path = typeof(DummyTests).Assembly.Location;
            ModuleDefinition module = ModuleDefinition.ReadModule(path);

            MethodsFinder finder = new MethodsFinder(module, "TestFixture", "Test");
            IEnumerable<MethodDefinition> methodDefinitions = finder.GetMethods();

            Assert.AreEqual(0, methodDefinitions.Where(definition => definition.Name == "NotTestMethod").Count());            
        }
    }
}

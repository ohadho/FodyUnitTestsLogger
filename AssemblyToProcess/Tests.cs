using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AssemblyToProcess
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void AddTest()
        {
            //int x = 1;
//            MethodBase currentMethod = MethodInfo.GetCurrentMethod();
//            Console.WriteLine(currentMethod);
        }

        [Test]
        public void AddTest2()
        {            
        }

        public void Foo()
        {
            
        }
    }

    public class NoTests
    {
        
    }
}

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
    public class DummyTests
    {
        [Test]
        public void AddTest()
        {
        }

        [Test]
        public void AddTest2()
        {            
        }

        public void NotTestMethod()
        {            
        }
    }

    public class NoTests
    {
        
    }
}

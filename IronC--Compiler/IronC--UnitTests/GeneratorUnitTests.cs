using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IronC__Generator;

namespace IronC__UnitTests
{
    [TestClass]
    public class GeneratorUnitTests
    {
        [TestMethod]
        public void Test1()
        {
            var g = new CodeGenerator(null);
            g.Generate("");
        }
    }
}

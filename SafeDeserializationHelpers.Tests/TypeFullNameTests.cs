using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zyan.SafeDeserializationHelpers.Tests
{
    [TestClass]
    public class TypeFullNameTests : TestBase
    {
        [TestMethod]
        public void EmptyStringIsNotAValidTypeName()
        {
            Assert_Throws<ArgumentException>(() => TypeFullName.Parse(null));
            Assert_Throws<ArgumentException>(() => TypeFullName.Parse(string.Empty));
            Assert_Throws<ArgumentException>(() => TypeFullName.Parse("    "));
            Assert_Throws<ArgumentException>(() => TypeFullName.Parse("  \t\r\n  "));
        }

        [TestMethod]
        public void SimpleTypeNameDoesntHaveAssemblyAndVersionParts()
        {
            var tn = TypeFullName.Parse("string");
            Assert.AreEqual("string", tn.TypeName);
            Assert.AreEqual("mscorlib", tn.AssemblyName);

            tn = TypeFullName.Parse("System.String");
            Assert.AreEqual("System.String", tn.TypeName);
            Assert.AreEqual("mscorlib", tn.AssemblyName);

            tn = TypeFullName.Parse("System.Management.Automation.PSObject");
            Assert.AreEqual("System.Management.Automation.PSObject", tn.TypeName);
            Assert.AreEqual("mscorlib", tn.AssemblyName);

            tn = TypeFullName.Parse(typeof(Enumerable).FullName);
            Assert.AreEqual("System.Linq.Enumerable", tn.TypeName);
            Assert.AreEqual("mscorlib", tn.AssemblyName);
        }

        [TestMethod]
        public void AssemblyQualifiedTypeNameContainsAssemblyPart()
        {
            var tn = TypeFullName.Parse("System.String, mscorlib");
            Assert.AreEqual("System.String", tn.TypeName);
            Assert.AreEqual("mscorlib", tn.AssemblyName);
        }

        [TestMethod]
        public void AssemblyQualifiedTypeNameContainsAssemblyAndVersionParts()
        {
            var tn = TypeFullName.Parse(typeof(string).AssemblyQualifiedName);
            Assert.AreEqual("System.String", tn.TypeName);
            Assert.AreEqual("mscorlib", tn.AssemblyName);

            tn = TypeFullName.Parse("System.Management.Automation.PSObject, System.Management.Automation, Version=3.0.0.0");
            Assert.AreEqual("System.Management.Automation.PSObject", tn.TypeName);
            Assert.AreEqual("System.Management.Automation", tn.AssemblyName);
        }
    }
}

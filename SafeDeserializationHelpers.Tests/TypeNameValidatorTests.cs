using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SafeDeserializationHelpers.Tests
{
    [TestClass]
    public class TypeNameValidatorTests : TestBase
    {
        [TestMethod]
        public void NullValuesAreIgnored()
        {
            Assert_DoesNotThrow(() =>
            {
                TypeNameValidator.Default.ValidateTypeName(null, null);
                TypeNameValidator.Default.ValidateTypeName("notnull", null);
                TypeNameValidator.Default.ValidateTypeName(null, "notnull");
            });
        }

        [TestMethod]
        public void ValidTypes()
        {
            Assert_DoesNotThrow(() =>
            {
                TypeNameValidator.Default.ValidateTypeName(null, "string");
                TypeNameValidator.Default.ValidateTypeName("mscorlib", "System.String");
                TypeNameValidator.Default.ValidateTypeName("AnotherLibrary", "System.Management.Automation.PSObject");
            });
        }

        [TestMethod]
        public void InvalidTypes()
        {
            Assert_Throws<UnsafeDeserializationException>(() =>
                TypeNameValidator.Default.ValidateTypeName("System.Management.Automation", "System.Management.Automation.PSObject"));

            Assert_Throws<UnsafeDeserializationException>(() =>
                TypeNameValidator.Default.ValidateTypeName("System.Management.Automation, Version=1.2.3.4", "System.Management.Automation.PSObject"));

            Assert_Throws<UnsafeDeserializationException>(() =>
                TypeNameValidator.Default.ValidateTypeName("System.Management.Automation, Version=3.0.0.0", "System.Management.Automation.PSObject"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SafeDeserializationHelpers.Tests
{
    [TestClass]
    public class TestBaseTests : TestBase
    {
        [TestMethod, ExpectedException(typeof(AssertFailedException))]
        public void AssertDoesntThrowFailsIfExceptionWasThrown()
        {
            Assert_DoesNotThrow(() => { throw new SecurityException(); });
        }

        [TestMethod, ExpectedException(typeof(AssertFailedException))]
        public void AssertThrowsFailsIfNoExceptionWasThrown()
        {
            Assert_Throws<SecurityException>(() => { });
        }

        [TestMethod, ExpectedException(typeof(AssertFailedException))]
        public void AssertThrowsFailsIfUnexpectedExceptionWasThrown()
        {
            Assert_Throws<SecurityException>(() => { new ArgumentNullException(); });
        }

        [TestMethod]
        public void AssertAreEqualWorksOnLotsOfDataTypes()
        {
            Assert_DoesNotThrow(() =>
            {
                Assert_AreEqual(1, 1);
                Assert_AreEqual("Hello", "Hello");
                Assert_AreEqual(new DataTable("Test"), new DataTable("Test"));
                Assert_AreEqual(new Func<string, string, Process>(Process.Start), new Func<string, string, Process>(Process.Start));
            });

            Assert_Throws<AssertFailedException>(() =>
            {
                Assert_AreEqual(1, 2);
            });
        }

        [TestMethod]
        public void RoundtripIsExpectedToSucceedOnPrimitives()
        {
            Assert_DoesNotThrow(() =>
            {
                Roundtrip(1, false);
                Roundtrip(1, true);
                Roundtrip("Hello", false);
                Roundtrip("Hello", true);
            });
        }
    }
}

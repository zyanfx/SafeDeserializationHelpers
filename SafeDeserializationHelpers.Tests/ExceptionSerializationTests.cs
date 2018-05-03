using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zyan.SafeDeserializationHelpers.Tests
{
    [TestClass]
    public class ExceptionSerializationTests : TestBase
    {
        [TestMethod]
        public void UnsafeDeserializationExceptionIsSerializedAsSecurityException()
        {
            var original = new UnsafeDeserializationException("Hello");
            var data = Serialize(original, true);
            var deserialized = Deserialize(data, true) as SecurityException;

            Assert.IsNotNull(deserialized);
            Assert.IsInstanceOfType(deserialized, typeof(SecurityException));
            Assert.IsNotInstanceOfType(deserialized, typeof(UnsafeDeserializationException));
            Assert_AreEqual(original.Message, deserialized.Message);
            Assert_AreEqual(original.InnerException, deserialized.InnerException);
            Assert_AreEqual(original.Data, deserialized.Data);
        }
    }
}

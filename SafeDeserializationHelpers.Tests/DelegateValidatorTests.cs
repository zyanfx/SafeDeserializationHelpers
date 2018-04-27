namespace Zyan.SafeDeserializationHelpers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DelegateValidatorTests : TestBase
    {
        [TestMethod]
        public void NullDelegateIsValid()
        {
            Assert_DoesNotThrow(() =>
                DelegateValidator.Default.ValidateDelegate(null));
        }

        [TestMethod]
        public void DelegateIsValidUnlessBlacklisted()
        {
            Assert_DoesNotThrow(() =>
                DelegateValidator.Default.ValidateDelegate(new Action<int>(x => { })));
        }

        [TestMethod]
        public void SystemDiagnosticsDelegatesAreNotValid()
        {
            var del = new Func<string, string, Process>(Process.Start);

            Assert_Throws<UnsafeDeserializationException>(() =>
                DelegateValidator.Default.ValidateDelegate(del));
        }

        [TestMethod]
        public void SystemIODelegatesAreNotValid()
        {
            var del = new Action<string>(File.Delete);

            Assert_Throws<UnsafeDeserializationException>(() =>
                DelegateValidator.Default.ValidateDelegate(del));
        }

        [TestMethod]
        public void MulticastDelegatesAreValidated()
        {
            var del = new Func<string, string, Process>((a, b) => null);
            del = Delegate.Combine(del, del, del) as Func<string, string, Process>;

            Assert_DoesNotThrow(() =>
                DelegateValidator.Default.ValidateDelegate(del));
        }

        [TestMethod]
        public void MulticastDelegatesWithSystemDiagnosticsMethodsAreNotValid()
        {
            var del = new Func<string, string, Process>((a, b) => null);
            var start = new Func<string, string, Process>(Process.Start);
            del = Delegate.Combine(del, del, start, del, del) as Func<string, string, Process>;

            Assert_Throws<UnsafeDeserializationException>(() =>
                DelegateValidator.Default.ValidateDelegate(del));
        }
    }
}

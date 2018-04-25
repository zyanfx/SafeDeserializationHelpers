namespace SafeDeserializationHelpers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DelegateValidatorTests
    {
        [TestMethod]
        public void NullDelegateIsValid()
        {
            // Assert.DoesNotThrow
            new DelegateValidator().ValidateDelegate(null);
        }

        [TestMethod]
        public void DelegateIsValidUnlessBlacklisted()
        {
            new DelegateValidator().ValidateDelegate(new Action<int>(x => { }));
        }

        [TestMethod, ExpectedException(typeof(UnsafeDeserializationException))]
        public void SystemDiagnosticsDelegatesAreNotValid()
        {
            var del = new Func<string, string, Process>(Process.Start);
            new DelegateValidator().ValidateDelegate(del);
        }

        [TestMethod, ExpectedException(typeof(UnsafeDeserializationException))]
        public void SystemIODelegatesAreNotValid()
        {
            var del = new Action<string>(File.Delete);
            new DelegateValidator().ValidateDelegate(del);
        }

        [TestMethod]
        public void MulticastDelegatesAreValidated()
        {
            var del = new Func<string, string, Process>((a, b) => null);
            del = Delegate.Combine(del, del, del) as Func<string, string, Process>;
            new DelegateValidator().ValidateDelegate(del);
        }

        [TestMethod, ExpectedException(typeof(UnsafeDeserializationException))]
        public void MulticastDelegatesWithSystemDiagnosticsMethodsAreNotValid()
        {
            var del = new Func<string, string, Process>((a, b) => null);
            var start = new Func<string, string, Process>(Process.Start);
            del = Delegate.Combine(del, del, start, del, del) as Func<string, string, Process>;
            new DelegateValidator().ValidateDelegate(del);
        }
    }
}

using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SafeDeserializationHelpers.Tests
{
    [TestClass]
    public class TestBase
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

        protected void Roundtrip(object graph, bool useBinder)
        {
            var data = default(byte[]);
            var fmt = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                fmt.Serialize(stream, graph);
                data = stream.ToArray();
            }

            if (useBinder)
            {
                fmt.Binder = new SafeSerializationBinder(fmt.Binder);
            }

            using (var stream = new MemoryStream(data))
            {
                var deserialized = fmt.Deserialize(stream);
                var msg = $"Deserialized data doesn't match when {(useBinder ? string.Empty : "not ")}using binder.";
                Assert_AreEqual(graph, deserialized, msg);
            }
        }

        protected void Assert_AreEqual(object expected, object actual, string msg = "Two values was expected to be equal.")
        {
            if (expected is Delegate del1 && actual is Delegate del2)
            {
                Assert_AreEqual(del1.Target, del2.Target, msg);
                Assert_AreEqual(del1.Method, del2.Method, msg);
                return;
            }

            if (expected is string s1 && actual is string s2)
            {
                // avoid comparing strings as IEnumerables
                Assert.AreEqual(s1, s2, msg);
                return;
            }

            if (expected is IEnumerable enum1 && actual is IEnumerable enum2)
            {
                var en1 = enum1.OfType<object>();
                var en2 = enum2.OfType<object>();

                Assert.AreEqual(en1.Count(), en2.Count(), msg);
                foreach (var item in en1.Zip(en2, (e1, e2) => (e1, e2)))
                {
                    Assert_AreEqual(item.e1, item.e2, msg);
                }

                return;
            }

            if (expected is IDictionary dic1 && actual is IDictionary dic2)
            {
                Assert.AreEqual(dic1.Count, dic2.Count, msg);
                foreach (var item in dic1.OfType<object>().Zip(dic2.OfType<object>(), (e1, e2) => (e1, e2)))
                {
                    Assert_AreEqual(item.e1, item.e2, msg);
                }

                return;
            }

            if (expected is DataSet ds1 && actual is DataSet ds2)
            {
                Assert_AreEqual(ds1.DataSetName, ds2.DataSetName, msg);
                Assert_AreEqual(ds1.Tables, ds2.Tables, msg);
                Assert_AreEqual(ds1.Relations, ds2.Relations, msg);
                return;
            }

            if (expected is DataTable dt1 && actual is DataTable dt2)
            {
                Assert_AreEqual(dt1.TableName, dt2.TableName, msg);
                Assert_AreEqual(dt1.Columns, dt2.Columns, msg);
                Assert_AreEqual(dt1.Rows, dt2.Rows, msg);
                return;
            }

            if (expected is DataColumn dc1 && actual is DataColumn dc2)
            {
                Assert_AreEqual(dc1.ColumnName, dc2.ColumnName, msg);
                Assert_AreEqual(dc1.DataType, dc2.DataType, msg);
                return;
            }

            if (expected is DataRow dr1 && actual is DataRow dr2)
            {
                Assert_AreEqual(dr1.ItemArray, dr2.ItemArray, msg);
                return;
            }

            Assert.AreEqual(expected, actual, msg);
        }

        protected void Assert_DoesNotThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.Fail($"No exception should be thrown, but here is what we caught: {ex.ToString()}");
            }
        }

        protected void Assert_Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T)
            {
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected to catch exception of type {typeof(T).Name}, but here is what we caught: {ex.ToString()}");
            }

            Assert.Fail($"Expected to catch exception of type {typeof(T).Name}, but no exception was thrown.");
        }
    }
}

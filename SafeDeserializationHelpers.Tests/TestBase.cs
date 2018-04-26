using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SafeDeserializationHelpers.Tests
{
    public class TestBase
    {
        protected void Roundtrip(object graph, bool safe)
        {
            // make sure that safe mode doesn't affect the serialization
            var originalData = Serialize(graph, false);
            var safeData = Serialize(graph, true);
            Assert_AreEqual(originalData, safeData);

            // make sure that deserialized graph is the same as the original
            var deserialized = Deserialize(originalData, safe);
            var msg = $"Deserialized data doesn't match when {(safe ? string.Empty : "not ")}using binder.";
            Assert_AreEqual(graph, deserialized, msg);
        }

        private byte[] Serialize(object graph, bool safe)
        {
            var fmt = new BinaryFormatter();
            if (safe)
            {
                fmt = fmt.Safe();
            }

            using (var stream = new MemoryStream())
            {
                fmt.Serialize(stream, graph);
                return stream.ToArray();
            }
        }

        private object Deserialize(byte[] data, bool safe)
        {
            var fmt = new BinaryFormatter();
            if (safe)
            {
                fmt = fmt.Safe();
            }

            using (var stream = new MemoryStream(data))
            {
                return fmt.Deserialize(stream);
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

            if (expected is DictionaryEntry de1 && actual is DictionaryEntry de2)
            {
                Assert_AreEqual(de1.Key, de2.Key, msg);
                Assert_AreEqual(de1.Value, de2.Value, msg);
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

            if (expected is PSObject ps1 && actual is PSObject ps2)
            {
                Assert_AreEqual(ps1.Properties, ps2.Properties);
                return;
            }

            if (expected is IIdentity id1 && actual is IIdentity id2)
            {
                Assert_AreEqual(id1.Name, id2.Name, msg);
                Assert_AreEqual(id1.IsAuthenticated, id2.IsAuthenticated, msg);
                Assert_AreEqual(id1.AuthenticationType, id2.AuthenticationType, msg);
                return;
            }

            if (expected != null && actual != null &&
                expected.GetType().IsValueType && !expected.GetType().IsPrimitive &&
                actual.GetType().IsValueType && !actual.GetType().IsPrimitive)
            {
                var expectedType = expected.GetType();
                var actualType = actual.GetType();
                Assert_AreEqual(expectedType, actualType, msg);

                foreach (var prop in expectedType.GetProperties())
                {
                    var exp = prop.GetValue(expected);
                    var act = prop.GetValue(actual);
                    Assert_AreEqual(exp, act, msg);
                }

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

        protected void Assert_Throws<T>(Action action, string msg = null) where T : Exception
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
                Assert.Fail(msg ?? $"Expected to catch exception of type {typeof(T).Name}, but here is what we caught: {ex.ToString()}");
            }

            Assert.Fail(msg ?? $"Expected to catch exception of type {typeof(T).Name}, but no exception was thrown.");
        }
    }
}

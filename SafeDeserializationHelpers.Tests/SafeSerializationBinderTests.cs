using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SafeDeserializationHelpers.Tests
{
    [TestClass]
    public class SafeSerializationBinderTests
    {
        private void Roundtrip(object graph, bool useBinder)
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
                AssertAreEqual(graph, deserialized, msg);
            }
        }

        private void AssertAreEqual(object expected, object actual, string msg)
        {
            if (expected is Delegate del1 && actual is Delegate del2)
            {
                AssertAreEqual(del1.Target, del2.Target, msg);
                AssertAreEqual(del1.Method, del2.Method, msg);
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
                Assert.AreEqual(enum1.OfType<object>().Count(), enum2.OfType<object>().Count(), msg);
                foreach (var item in enum1.OfType<object>().Zip(enum2.OfType<object>(), (e1, e2) => (e1, e2)))
                {
                    AssertAreEqual(item.e1, item.e2, msg);
                }

                return;
            }

            if (expected is IDictionary dic1 && actual is IDictionary dic2)
            {
                Assert.AreEqual(dic1.Count, dic2.Count, msg);
                foreach (var item in dic1.OfType<object>().Zip(dic2.OfType<object>(), (e1, e2) => (e1, e2)))
                {
                    AssertAreEqual(item.e1, item.e2, msg);
                }

                return;
            }

            if (expected is DataTable dt1 && actual is DataTable dt2)
            {
                AssertAreEqual(dt1.TableName, dt2.TableName, msg);
                AssertAreEqual(dt1.Columns, dt2.Columns, msg);
                AssertAreEqual(dt1.Rows, dt2.Rows, msg);
                return;
            }

            if (expected is DataColumn dc1 && actual is DataColumn dc2)
            {
                AssertAreEqual(dc1.ColumnName, dc2.ColumnName, msg);
                AssertAreEqual(dc1.DataType, dc2.DataType, msg);
                return;
            }

            if (expected is DataRow dr1 && actual is DataRow dr2)
            {
                AssertAreEqual(dr1.ItemArray, dr2.ItemArray, msg);
                return;
            }

            Assert.AreEqual(expected, actual, msg);
        }

        [Serializable]
        public class PublicSerializable
        {
            public int X { get; set; }
            public override int GetHashCode() => X.GetHashCode();
            public override bool Equals(object obj)
            {
                if (obj is PublicSerializable p) return p.X == X;
                return false;
            }
        }

        [Serializable]
        private class PrivateSerializable
        {
            public string Y { get; set; }
            public static void SampleMethod(int a, string b, DateTime c) { }
            public override int GetHashCode() => Y.GetHashCode();
            public override bool Equals(object obj)
            {
                if (obj is PrivateSerializable p) return p.Y == Y;
                return false;
            }
        }

        [TestMethod]
        public void SafeSerializationBinderDoesntBreakNormalClasses()
        {
            // prepare sample data: delegates
            var func = new Func<object, bool>(new PublicSerializable().Equals);
            var action = new Action<int, string, DateTime>(PrivateSerializable.SampleMethod);

            // data tables
            var dt = new DataTable("TestTable");
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("Name", typeof(string));
            var row = dt.NewRow();
            row["ID"] = 123;
            row["Name"] = "432";
            dt.Rows.Add(row);

            // scalar values, collections and dictionaries
            var samples = new object[]
            {
                1, "Test", func, action, dt,
                new List<string> { "abc", "def" },
                new Dictionary<int, char> { { 1, 'a' }, { 2, 'b'} },
                new Hashtable { { "Hello", "World" } },
                new List<Delegate> { func, action, action, func },
                new PublicSerializable { X = 123 },
                new PrivateSerializable { Y = "Hello" }
            };

            // make sure that the round-trip doesn't damage any of them
            foreach (var sample in samples)
            {
                Roundtrip(sample, false);
                Roundtrip(sample, true);
            }
        }

        [TestMethod]
        public void OrdinaryBinaryFormatterDoesntBreakOnProcessStartDelegate()
        {
            Roundtrip(new Func<string, string, Process>(Process.Start), false);
        }

        [TestMethod, ExpectedException(typeof(UnsafeDeserializationException))]
        public void SafeSerializationBinderBreaksOnProcessStartDelegate()
        {
            Roundtrip(new Func<string, string, Process>(Process.Start), true);
        }
    }
}

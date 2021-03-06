﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zyan.SafeDeserializationHelpers.Tests
{
    [TestClass]
    public class SafeSerializationBinderTests : TestBase
    {
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

            var rdt = new DataTable("AnotherTestTable");
            rdt.Columns.Add("Value", typeof(string));
            row = rdt.NewRow();
            row["Value"] = "Hello";
            rdt.Rows.Add(row);

            // data set in XML format
            var ds = new DataSet("XmlTestData");
            ds.Tables.Add(dt);
            ds.RemotingFormat = SerializationFormat.Xml;

            // data set in binary format
            var rds = new DataSet("BinaryTestData");
            rds.RemotingFormat = SerializationFormat.Binary;
            rds.Tables.Add(rdt);

            // identity
            var id = WindowsIdentity.GetAnonymous();

            // scalar values, collections and dictionaries
            var samples = new object[]
            {
                1, "Test", func, action, dt, ds, rds, id,
                new List<string> { "abc", "def" },
                new Dictionary<int, char> { { 1, 'a' }, { 2, 'b'} },
                new Hashtable { { "Hello", "World" } },
                new List<Delegate> { func, action, action, func },
                new PublicSerializable { X = 123 },
                new PrivateSerializable { Y = "Hello" },
                new DataSet[] { ds, null, null, rds, ds },
                new List<DataSet> { null, rds, ds },
                new DataTable[] { dt, rdt, null, rdt, dt },
                new List<DataTable> { rdt, null, dt },
                new IIdentity[] { null, id, null },
                new Dictionary<string, DataSet> { { ds.DataSetName, ds }, { rds.DataSetName, rds } },
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
            Assert_DoesNotThrow(() =>
                Roundtrip(new Func<string, string, Process>(Process.Start), false));
        }

        [TestMethod]
        public void SafeSerializationBinderBreaksOnProcessStartDelegate1()
        {
            Assert_Throws<UnsafeDeserializationException>(() =>
                Roundtrip(new Func<string, string, Process>(Process.Start), true));
        }

        [TestMethod]
        public void SafeSerializationBinderBreaksOnProcessStartDelegate2()
        {
            // add Process.Start somewhere to the chain of the delegates
            var d = new Func<string, string, Process>((s1, s2) => null);
            var e = new Func<string, string, Process>(Process.Start);
            var f = Delegate.Combine(d, d, e, d, d);

            Assert_Throws<UnsafeDeserializationException>(() => Roundtrip(f, true));
        }

        [TestMethod]
        public void OrdinaryBinaryFormatterDoesntBreakOnFileDeleteDelegate()
        {
            Assert_DoesNotThrow(() => Roundtrip(new Action<string>(File.Delete), false));
        }

        [TestMethod]
        public void SafeSerializationBinderBreaksOnFileDeleteDelegate()
        {
            Assert_Throws<UnsafeDeserializationException>(() =>
                Roundtrip(new Action<string>(File.Delete), true));
        }

        [TestMethod]
        public void OrdinaryBinaryFormatterDoesntBreakOnPSObjectType()
        {
            var psobject = new PSObject();
            Assert_DoesNotThrow(() => Roundtrip(psobject, false));

            var array = new object[] { 1, "Hello!", psobject, "Goodbye!" };
            Assert_DoesNotThrow(() => Roundtrip(array, false));

            var hash = new Hashtable { { "a", "b" }, { "c", array }, { "d", 123 } };
            Assert_DoesNotThrow(() => Roundtrip(hash, false));

            var list = new List<Hashtable> { null, hash, null, hash };
            Assert_DoesNotThrow(() => Roundtrip(list, false));

            var ex = new Exception("foo", new NullReferenceException("bar"));
            ex.Data["quux"] = list;
            Assert_DoesNotThrow(() => Roundtrip(ex, false));
        }

        [TestMethod]
        public void SafeSerializationBinderBreaksOnPSObjectType()
        {
            var psobject = new PSObject();
            Assert_Throws<UnsafeDeserializationException>(() => Roundtrip(psobject, true));

            var array = new object[] { 1, "Hello!", psobject, "Goodbye!" };
            Assert_Throws<UnsafeDeserializationException>(() => Roundtrip(array, true));

            var hash = new Hashtable { { "a", "b" }, { "c", array }, { "d", 123 } };
            Assert_Throws<UnsafeDeserializationException>(() => Roundtrip(hash, true));

            var list = new List<Hashtable> { null, hash, null, hash };
            Assert_Throws<UnsafeDeserializationException>(() => Roundtrip(list, true));

            var ex = new Exception("foo", new NullReferenceException("bar"));
            ex.Data["quux"] = list;
            Assert_Throws<UnsafeDeserializationException>(() => Roundtrip(ex, true));
        }
    }
}

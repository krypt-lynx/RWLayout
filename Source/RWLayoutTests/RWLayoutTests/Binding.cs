using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayoutTests
{
    [TestClass]
    public class Binding
    {
        class TargetClass
        {
            public string Field;
            public string Property { get => Field; set => Field = value; }

            public string StaticField;
            public string StaticProperty { get => StaticField; set => StaticField = value; }
        }

        struct TargetStruct
        {
            public string Field;
            public string Property { get => Field; set => Field = value; }

            static public string StaticField;
            static public string StaticProperty { get => StaticField; set => StaticField = value; }
        }

        class Binded
        {
            public Bindable<string> BindableProp = new Bindable<string>();
        }

        [TestMethod]
        public void ClassInstanceProperty()
        {
            var target = new TargetClass();
            target.Field = "foo";

            var binded = new Binded();
            binded.BindableProp.Bind(target, "Property");

            Assert.AreEqual("foo", binded.BindableProp.Value);
            target.Field = "bar";
            Assert.AreEqual("bar", binded.BindableProp.Value);
            binded.BindableProp.Value = "baz";
            Assert.AreEqual("baz", target.Field);
        }

        [TestMethod]
        public void ClassInstanceField()
        {
            var target = new TargetClass();
            target.Property = "foo";

            var binded = new Binded();
            binded.BindableProp.Bind(target, "Field");

            Assert.AreEqual("foo", binded.BindableProp.Value);
            target.Property = "bar";
            Assert.AreEqual("bar", binded.BindableProp.Value);
            binded.BindableProp.Value = "baz";
            Assert.AreEqual("baz", target.Property);
        }

        [TestMethod]
        public void ClassStaticProperty()
        {
            var target = new TargetClass();
            target.StaticField = "foo";

            var binded = new Binded();
            binded.BindableProp.Bind(target, "StaticProperty");

            Assert.AreEqual("foo", binded.BindableProp.Value);
            target.StaticField = "bar";
            Assert.AreEqual("bar", binded.BindableProp.Value);
            binded.BindableProp.Value = "baz";
            Assert.AreEqual("baz", target.StaticField);
        }

        [TestMethod]
        public void ClassStaticField()
        {
            var target = new TargetClass();
            target.StaticProperty = "foo";

            var binded = new Binded();
            binded.BindableProp.Bind(target, "StaticField");

            Assert.AreEqual("foo", binded.BindableProp.Value);
            target.StaticProperty = "bar";
            Assert.AreEqual("bar", binded.BindableProp.Value);
            binded.BindableProp.Value = "baz";
            Assert.AreEqual("baz", target.StaticProperty);
        }


        [TestMethod]
        public void StructInstanceProperty_Throws()
        {
            var target = new TargetStruct();
            target.Field = "foo";

            var binded = new Binded();
            Assert.ThrowsException<Exception>(() =>
            {
                binded.BindableProp.Bind(target, "Property");
            });
        }

        [TestMethod]
        public void StructInstanceField_Throws()
        {
            var target = new TargetStruct();
            target.Property = "foo";

            var binded = new Binded();
            Assert.ThrowsException<Exception>(() =>
            {
                binded.BindableProp.Bind(target, "Field");
            });
        }

        [TestMethod]
        public void StructStaticProperty()
        {
            var target = new TargetStruct();
            TargetStruct.StaticField = "foo";

            var binded = new Binded();
            binded.BindableProp.Bind(target, "StaticProperty");

            TargetStruct.StaticProperty = "foo";
            Assert.AreEqual("foo", binded.BindableProp.Value);
            TargetStruct.StaticField = "bar";
            Assert.AreEqual("bar", binded.BindableProp.Value);
            binded.BindableProp.Value = "baz";
            Assert.AreEqual("baz", TargetStruct.StaticField);
        }

        [TestMethod]
        public void StructStaticField()
        {
            var target = new TargetStruct();
            TargetStruct.StaticProperty = "foo";

            var binded = new Binded();
            binded.BindableProp.Bind(target, "StaticField");

            TargetStruct.StaticField = "foo";
            Assert.AreEqual("foo", binded.BindableProp.Value);
            TargetStruct.StaticProperty = "bar";
            Assert.AreEqual("bar", binded.BindableProp.Value);
            binded.BindableProp.Value = "baz";
            Assert.AreEqual("baz", TargetStruct.StaticProperty);
        }

    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2.FastAccess;

namespace RWLayoutTests
{
    [TestClass]
    public class FastAccess_ClassInstance
    {
        class TestClass
        {

            public string GetTestField() => field;
            public void SetTestField(string value) => field = value;
            private string field = "field";

            public string testProperty = "property";
            private string property { set => testProperty = value; get => testProperty; }


            private string RetMethod0()
            {
                return "Method0";
            }
            private string RetMethod1(int arg0)
            {
                return arg0.ToString();
            }
            private string RetMethod2(int arg0, int arg1)
            {
                return (arg0 + arg1).ToString();
            }
            private string RetMethod3(int arg0, int arg1, int arg2)
            {
                return (arg0 + arg1 + arg2).ToString();
            }
            private string RetMethod4(int arg0, int arg1, int arg2, int arg3)
            {
                return (arg0 + arg1 + arg2 + arg3).ToString();
            }
            private string RetMethod5(int arg0, int arg1, int arg2, int arg3, int arg4)
            {
                return (arg0 + arg1 + arg2 + arg3 + arg4).ToString();
            }


            public string testValue = "0";


            private void VoidMethod0()
            {
                testValue = "Method0";
            }
            private void VoidMethod1(int arg0)
            {
                testValue = arg0.ToString();
            }
            private void VoidMethod2(int arg0, int arg1)
            {
                testValue = (arg0 + arg1).ToString();
            }
            private void VoidMethod3(int arg0, int arg1, int arg2)
            {
                testValue = (arg0 + arg1 + arg2).ToString();
            }
            private void VoidMethod4(int arg0, int arg1, int arg2, int arg3)
            {
                testValue = (arg0 + arg1 + arg2 + arg3).ToString();
            }
            private void VoidMethod5(int arg0, int arg1, int arg2, int arg3, int arg4)
            {
                testValue = (arg0 + arg1 + arg2 + arg3 + arg4).ToString();
            }
        }

        [TestMethod]
        public void GetField()
        {
            var test = new TestClass();
            var get = Dynamic.ObjectGetField<TestClass, string>("field");

            Assert.AreEqual("field", get(test));
            test.SetTestField("new");
            Assert.AreEqual("new", get(test));
        }

        [TestMethod]
        public void SetField()
        {
            var test = new TestClass();
            var set = Dynamic.ObjectSetField<TestClass, string>("field");

            set(test, "new");
            Assert.AreEqual("new", test.GetTestField());
        }

        [TestMethod]
        public void GetProperty()
        {
            var test = new TestClass();
            var get = Dynamic.ObjectGetProperty<TestClass, string>("property");

            Assert.AreEqual("property", get(test));
            test.testProperty = "new";
            Assert.AreEqual("new", get(test));
        }

        [TestMethod]
        public void SetProperty()
        {
            var test = new TestClass();
            var set = Dynamic.ObjectSetProperty<TestClass, string>("property");

            set(test, "new");
            Assert.AreEqual("new", test.testProperty);
        }

        [TestMethod]
        public void CallRetMethod()
        {
            var test = new TestClass();
            var call0 = Dynamic.InstanceRetMethod<TestClass, string>("RetMethod0");
            var call1 = Dynamic.InstanceRetMethod<TestClass, int, string>("RetMethod1");
            var call2 = Dynamic.InstanceRetMethod<TestClass, int, int, string>("RetMethod2");
            var call3 = Dynamic.InstanceRetMethod<TestClass, int, int, int, string>("RetMethod3");
            var call4 = Dynamic.InstanceRetMethod<TestClass, int, int, int, int, string>("RetMethod4");
            var call5 = Dynamic.InstanceRetMethod<TestClass, int, int, int, int, int, string>("RetMethod5");

            Assert.AreEqual("Method0", call0(test));
            Assert.AreEqual("1", call1(test, 1));
            Assert.AreEqual("3", call2(test, 1, 2));
            Assert.AreEqual("6", call3(test, 1, 2, 3));
            Assert.AreEqual("10", call4(test, 1, 2, 3, 4));
            Assert.AreEqual("15", call5(test, 1, 2, 3, 4, 5));
        }

        [TestMethod]
        public void CallVoidMethod()
        {
            var test = new TestClass();
            var call0 = Dynamic.InstanceVoidMethod<TestClass>("VoidMethod0");
            var call1 = Dynamic.InstanceVoidMethod<TestClass, int>("VoidMethod1");
            var call2 = Dynamic.InstanceVoidMethod<TestClass, int, int>("VoidMethod2");
            var call3 = Dynamic.InstanceVoidMethod<TestClass, int, int, int>("VoidMethod3");
            var call4 = Dynamic.InstanceVoidMethod<TestClass, int, int, int, int>("VoidMethod4");
            var call5 = Dynamic.InstanceVoidMethod<TestClass, int, int, int, int, int>("VoidMethod5");

            call0(test);
            Assert.AreEqual("Method0", test.testValue);
            call1(test, 1);
            Assert.AreEqual("1", test.testValue);
            call2(test, 1, 2);
            Assert.AreEqual("3", test.testValue);
            call3(test, 1, 2, 3);
            Assert.AreEqual("6", test.testValue);
            call4(test, 1, 2, 3, 4);
            Assert.AreEqual("10", test.testValue);
            call5(test, 1, 2, 3, 4, 5);
            Assert.AreEqual("15", test.testValue);
        }

    }
}

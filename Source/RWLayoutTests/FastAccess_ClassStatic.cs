using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2.FastAccess;

namespace RWLayoutTests.FastAccess
{
    [TestClass]
    public class FastAccess_ClassStatic
    {
        class TestClass
        {
            public static string GetTestField() => field;
            public static void SetTestField(string value) => field = value;
            private static string field = "field";

            public static string testProperty = "property";
            private static string property { set => testProperty = value; get => testProperty; }


            private static string RetMethod0()
            {
                return "Method0";
            }
            private static string RetMethod1(int arg0)
            {
                return arg0.ToString();
            }
            private static string RetMethod2(int arg0, int arg1)
            {
                return (arg0 + arg1).ToString();
            }
            private static string RetMethod3(int arg0, int arg1, int arg2)
            {
                return (arg0 + arg1 + arg2).ToString();
            }
            private static string RetMethod4(int arg0, int arg1, int arg2, int arg3)
            {
                return (arg0 + arg1 + arg2 + arg3).ToString();
            }
            private static string RetMethod5(int arg0, int arg1, int arg2, int arg3, int arg4)
            {
                return (arg0 + arg1 + arg2 + arg3 + arg4).ToString();
            }

            public static string testValue = "0";

            private static void VoidMethod0()
            {
                testValue = "Method0";
            }
            private static void VoidMethod1(int arg0)
            {
                testValue = arg0.ToString();
            }
            private static void VoidMethod2(int arg0, int arg1)
            {
                testValue = (arg0 + arg1).ToString();
            }
            private static void VoidMethod3(int arg0, int arg1, int arg2)
            {
                testValue = (arg0 + arg1 + arg2).ToString();
            }
            private static void VoidMethod4(int arg0, int arg1, int arg2, int arg3)
            {
                testValue = (arg0 + arg1 + arg2 + arg3).ToString();
            }
            private static void VoidMethod5(int arg0, int arg1, int arg2, int arg3, int arg4)
            {
                testValue = (arg0 + arg1 + arg2 + arg3 + arg4).ToString();
            }
        }

        [TestMethod]
        public void GetField()
        {
            var get = Dynamic.StaticGetField<TestClass, string>("field");

            Assert.AreEqual("field", get());
            TestClass.SetTestField("new");
            Assert.AreEqual("new", get());
        }

        [TestMethod]
        public void SetField()
        {
            var set = Dynamic.StaticSetField<TestClass, string>("field");

            set("new");
            Assert.AreEqual("new", TestClass.GetTestField());
        }

        [TestMethod]
        public void GetProperty()
        {
            var get = Dynamic.StaticGetProperty<TestClass, string>("property");

            Assert.AreEqual("property", get());
            TestClass.testProperty = "new";
            Assert.AreEqual("new", get());
        }

        [TestMethod]
        public void SetProperty()
        {
            var set = Dynamic.StaticSetProperty<TestClass, string>("property");

            set("new");
            Assert.AreEqual("new", TestClass.testProperty);
        }


        [TestMethod]
        public void CallRetMethod()
        {
            var test = new TestClass();
            var call0 = Dynamic.StaticRetMethod<TestClass, string>("RetMethod0");
            var call1 = Dynamic.StaticRetMethod<TestClass, int, string>("RetMethod1");
            var call2 = Dynamic.StaticRetMethod<TestClass, int, int, string>("RetMethod2");
            var call3 = Dynamic.StaticRetMethod<TestClass, int, int, int, string>("RetMethod3");
            var call4 = Dynamic.StaticRetMethod<TestClass, int, int, int, int, string>("RetMethod4");
            var call5 = Dynamic.StaticRetMethod<TestClass, int, int, int, int, int, string>("RetMethod5");

            Assert.AreEqual("Method0", call0());
            Assert.AreEqual("1", call1(1));
            Assert.AreEqual("3", call2(1, 2));
            Assert.AreEqual("6", call3(1, 2, 3));
            Assert.AreEqual("10", call4(1, 2, 3, 4));
            Assert.AreEqual("15", call5(1, 2, 3, 4, 5));
        }

        [TestMethod]
        public void CallVoidMethod()
        {
            var call0 = Dynamic.StaticVoidMethod<TestClass>("VoidMethod0");
            var call1 = Dynamic.StaticVoidMethod<TestClass, int>("VoidMethod1");
            var call2 = Dynamic.StaticVoidMethod<TestClass, int, int>("VoidMethod2");
            var call3 = Dynamic.StaticVoidMethod<TestClass, int, int, int>("VoidMethod3");
            var call4 = Dynamic.StaticVoidMethod<TestClass, int, int, int, int>("VoidMethod4");
            var call5 = Dynamic.StaticVoidMethod<TestClass, int, int, int, int, int>("VoidMethod5");

            call0();
            Assert.AreEqual("Method0", TestClass.testValue);
            call1(1);
            Assert.AreEqual("1", TestClass.testValue);
            call2(1, 2);
            Assert.AreEqual("3", TestClass.testValue);
            call3(1, 2, 3);
            Assert.AreEqual("6", TestClass.testValue);
            call4(1, 2, 3, 4);
            Assert.AreEqual("10", TestClass.testValue);
            call5(1, 2, 3, 4, 5);
            Assert.AreEqual("15", TestClass.testValue);
        }

    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2.FastAccess;

namespace RWLayoutTests
{
    [TestClass]
    public class FastAccess_StructInstance
    {
        struct TestStruct
        {
            static public TestStruct Create()
            {
                var test = new TestStruct();
                test.field = "field";
                test.property = "property";

                return test;
            }

            public string GetTestField() => field;
            public void SetTestField(string value) => field = value;
            private string field;

            public string testProperty;
            public object theObject;
            private string property 
            {
                set
                {
                    testProperty = value;
                }
                get => testProperty; 
            }

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

            static public string testValue = "0";

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
            var test = TestStruct.Create();
            var get = Dynamic.StructGetField<TestStruct, string>("field");

            Assert.AreEqual("field", get(ref test));
            test.SetTestField("new");
            Assert.AreEqual("new", get(ref test));
        }

        [TestMethod]
        public void SetField()
        {
            var test = TestStruct.Create();
            var set = Dynamic.StructSetField<TestStruct, string>("field");

            Assert.AreEqual("field", test.GetTestField());
            set(ref test, "new");
            Assert.AreEqual("new", test.GetTestField());
        }

        [TestMethod]
        public void GetProperty()
        {
            var test = TestStruct.Create();
            var get = Dynamic.StructGetProperty<TestStruct, string>("property");

            Assert.AreEqual("property", get(ref test));
            test.testProperty = "new";
            Assert.AreEqual("new", get(ref test));
        }

        [TestMethod]
        public void SetProperty()
        {
            var test = TestStruct.Create();
            var set = Dynamic.StructSetProperty<TestStruct, string>("property");

            Assert.AreEqual("property", test.testProperty);
            set(ref test, "new");
            Assert.AreEqual("new", test.testProperty);
        }

        [TestMethod]
        public void CallRetMethod()
        {
            throw new Exception("calls methods of the struct copy");

            var test = TestStruct.Create();
            var call0 = Dynamic.InstanceRetMethod<TestStruct, string>("RetMethod0");
            var call1 = Dynamic.InstanceRetMethod<TestStruct, int, string>("RetMethod1");
            var call2 = Dynamic.InstanceRetMethod<TestStruct, int, int, string>("RetMethod2");
            var call3 = Dynamic.InstanceRetMethod<TestStruct, int, int, int, string>("RetMethod3");
            var call4 = Dynamic.InstanceRetMethod<TestStruct, int, int, int, int, string>("RetMethod4");
            var call5 = Dynamic.InstanceRetMethod<TestStruct, int, int, int, int, int, string>("RetMethod5");

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
            throw new Exception("calls methods of the struct copy");
            var test = TestStruct.Create();
            var call0 = Dynamic.InstanceVoidMethod<TestStruct>("VoidMethod0");
            var call1 = Dynamic.InstanceVoidMethod<TestStruct, int>("VoidMethod1");
            var call2 = Dynamic.InstanceVoidMethod<TestStruct, int, int>("VoidMethod2");
            var call3 = Dynamic.InstanceVoidMethod<TestStruct, int, int, int>("VoidMethod3");
            var call4 = Dynamic.InstanceVoidMethod<TestStruct, int, int, int, int>("VoidMethod4");
            var call5 = Dynamic.InstanceVoidMethod<TestStruct, int, int, int, int, int>("VoidMethod5");

            call0(test);
            Assert.AreEqual("Method0", TestStruct.testValue);
            call1(test, 1);
            Assert.AreEqual("1", TestStruct.testValue);
            call2(test, 1, 2);
            Assert.AreEqual("3", TestStruct.testValue);
            call3(test, 1, 2, 3);
            Assert.AreEqual("6", TestStruct.testValue);
            call4(test, 1, 2, 3, 4);
            Assert.AreEqual("10", TestStruct.testValue);
            call5(test, 1, 2, 3, 4, 5);
            Assert.AreEqual("15", TestStruct.testValue);
        }

    }
}

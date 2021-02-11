using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2.FastAccess;

namespace RWLayoutTests.FastAccess
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
                test.testValue = 0;

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
                testValue = -1;
                return testValue.ToString();
            }
            private string RetMethod1(int arg0)
            {
                testValue = testValue + arg0;
                return testValue.ToString();
            }
            private string RetMethod2(int arg0, int arg1)
            {
                testValue = testValue + arg0 + arg1;
                return testValue.ToString();
            }
            private string RetMethod3(int arg0, int arg1, int arg2)
            {
                testValue = testValue + arg0 + arg1 + arg2;
                return testValue.ToString();
            }
            private string RetMethod4(int arg0, int arg1, int arg2, int arg3)
            {
                testValue = testValue + arg0 + arg1 + arg2 + arg3;
                return testValue.ToString();
            }
            private string RetMethod5(int arg0, int arg1, int arg2, int arg3, int arg4)
            {
                testValue = testValue + arg0 + arg1 + arg2 + arg3 + arg4;
                return testValue.ToString();
            }

            public int testValue;

            private void VoidMethod0()
            {
                testValue = 1;
            }
            private void VoidMethod1(int arg0)
            {
                testValue = arg0;
            }
            private void VoidMethod2(int arg0, int arg1)
            {
                testValue = (arg0 + arg1);
            }
            private void VoidMethod3(int arg0, int arg1, int arg2)
            {
                testValue = (arg0 + arg1 + arg2);
            }
            private void VoidMethod4(int arg0, int arg1, int arg2, int arg3)
            {
                testValue = (arg0 + arg1 + arg2 + arg3);
            }
            private void VoidMethod5(int arg0, int arg1, int arg2, int arg3, int arg4)
            {
                testValue = (arg0 + arg1 + arg2 + arg3 + arg4);
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
            // struct is copied, by ref call is needed

            var test = TestStruct.Create();
            var call0 = Dynamic.StructRetMethod<TestStruct, string>("RetMethod0");
            var call1 = Dynamic.StructRetMethod<TestStruct, int, string>("RetMethod1");
            var call2 = Dynamic.StructRetMethod<TestStruct, int, int, string>("RetMethod2");
            var call3 = Dynamic.StructRetMethod<TestStruct, int, int, int, string>("RetMethod3");
            var call4 = Dynamic.StructRetMethod<TestStruct, int, int, int, int, string>("RetMethod4");
            var call5 = Dynamic.StructRetMethod<TestStruct, int, int, int, int, int, string>("RetMethod5");

            test.testValue = 10;
            Assert.AreEqual("-1", call0(ref test));
            Assert.AreEqual(-1, test.testValue);

            test.testValue = 20;
            Assert.AreEqual("21", call1(ref test, 1));
            Assert.AreEqual(21, test.testValue);

            test.testValue = 30;
            Assert.AreEqual("33", call2(ref test, 1, 2));
            Assert.AreEqual(33, test.testValue);

            test.testValue = 40;
            Assert.AreEqual("46", call3(ref test, 1, 2, 3));
            Assert.AreEqual(46, test.testValue);

            test.testValue = 50;
            Assert.AreEqual("60", call4(ref test, 1, 2, 3, 4));
            Assert.AreEqual(60, test.testValue);

            test.testValue = 60;
            Assert.AreEqual("75", call5(ref test, 1, 2, 3, 4, 5));
            Assert.AreEqual(75, test.testValue);
        }

        [TestMethod]
        public void CallVoidMethod()
        {
            // struct is copied, by ref call is needed

            var test = TestStruct.Create();
            var call0 = Dynamic.StructVoidMethod<TestStruct>("VoidMethod0");
            var call1 = Dynamic.StructVoidMethod<TestStruct, int>("VoidMethod1");
            var call2 = Dynamic.StructVoidMethod<TestStruct, int, int>("VoidMethod2");
            var call3 = Dynamic.StructVoidMethod<TestStruct, int, int, int>("VoidMethod3");
            var call4 = Dynamic.StructVoidMethod<TestStruct, int, int, int, int>("VoidMethod4");
            var call5 = Dynamic.StructVoidMethod<TestStruct, int, int, int, int, int>("VoidMethod5");

            call0(ref test);
            Assert.AreEqual(1, test.testValue);
            call1(ref test, 1);
            Assert.AreEqual(1, test.testValue);
            call2(ref test, 1, 2);
            Assert.AreEqual(3, test.testValue);
            call3(ref test, 1, 2, 3);
            Assert.AreEqual(6, test.testValue);
            call4(ref test, 1, 2, 3, 4);
            Assert.AreEqual(10, test.testValue);
            call5(ref test, 1, 2, 3, 4, 5);
            Assert.AreEqual(15, test.testValue);
        }

    }
}

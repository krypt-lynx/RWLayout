using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2.FastAccess;

namespace RWLayoutTests.FastAccess
{
    [TestClass]
    public class FastAccess_Ctor
    {
        class TestClass
        {
            public int test = 0;

            private TestClass()
            {
                test = 10;
            }
            private TestClass(int a)
            {
                test = a;
            }
            private TestClass(int a, int b)
            {
                test = a + b;
            }
            private TestClass(int a, int b, int c)
            {
                test = a + b + c;
            }
            private TestClass(int a, int b, int c, int d)
            {
                test = a + b + c + d;
            }
            private TestClass(int a, int b, int c, int d, int e)
            {
                test = a + b + c + d + e;
            }
            private TestClass(string a, int b, ref int c, ref string d)
            {
                test = int.Parse(a) + b + c + int.Parse(d);
                c = int.Parse(a) + b;
                d = (int.Parse(a) - b).ToString();
            }
        }

        struct TestStruct

        {
            public int test;

            private TestStruct(int a)
            {
                test = a;
            }
            private TestStruct(int a, int b)
            {
                test = a + b;
            }
            private TestStruct(int a, int b, int c)
            {
                test = a + b + c;
            }
            private TestStruct(int a, int b, int c, int d)
            {
                test = a + b + c + d;
            }
            private TestStruct(int a, int b, int c, int d, int e)
            {
                test = a + b + c + d + e;
            }
            private TestStruct(int a, int b, ref int c)
            {
                test = a + b + c;
                c = a + b;
            }
        }

        private delegate TestClass TestClassRefCtor(string a, int b, ref int c, ref string d);
        private delegate TestClass TestClassRefCtor_0(object a, int b, ref int c, ref string d);
        private delegate TestClass TestClassRefCtor_1(string a, object b, ref int c, ref string d);
        private delegate TestClass TestClassRefCtor_2(string a, int b, ref object c, ref string d);
        private delegate TestClass TestClassRefCtor_3(string a, int b, ref int c, ref object d);
        private delegate TestClass TestClassRefCtor_4(object a, object b, ref object c, ref object d);

        [TestMethod]
        public void ArgumentCasting()
        {
            var callRef = Dynamic.ConstructorCallerFromDelegate<TestClassRefCtor>();

            var constructor = typeof(TestClass).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                typeof(TestClassRefCtor).GetMethod("Invoke").GetParameters().Select(x => x.ParameterType).ToArray(), null);


            var callRef_0 = (TestClassRefCtor_0)Dynamic.ConstructorCaller(typeof(TestClassRefCtor_0), constructor);
            var callRef_1 = (TestClassRefCtor_1)Dynamic.ConstructorCaller(typeof(TestClassRefCtor_1), constructor);
            var callRef_2 = (TestClassRefCtor_2)Dynamic.ConstructorCaller(typeof(TestClassRefCtor_2), constructor);
            var callRef_3 = (TestClassRefCtor_3)Dynamic.ConstructorCaller(typeof(TestClassRefCtor_3), constructor);
            var callRef_4 = (TestClassRefCtor_4)Dynamic.ConstructorCaller(typeof(TestClassRefCtor_4), constructor);

            int c = 3;
            string d = "4"; 
            var testVar = callRef("1", 2, ref c, ref d);
            Assert.AreEqual(testVar.test, 10);
            Assert.AreEqual(3, c);
            Assert.AreEqual("-1", d);

            c = 3;
            d = "4";
            testVar = callRef_0("1", 2, ref c, ref d);
            Assert.AreEqual(testVar.test, 10);
            Assert.AreEqual(3, c);
            Assert.AreEqual("-1", d);

            c = 3;
            d = "4";
            testVar = callRef_1("1", 2, ref c, ref d);
            Assert.AreEqual(testVar.test, 10);
            Assert.AreEqual(3, c);
            Assert.AreEqual("-1", d);


            object ctest = c;
            object dtest = c;

            c = 3;
            d = "4";
            ctest = c;
            //dtest = d;
            testVar = callRef_2("1", 2, ref ctest, ref d);
            c = (int)ctest;
            //d = (string)dtest;
            Assert.AreEqual(testVar.test, 10);
            Assert.AreEqual(3, c);
            Assert.AreEqual("-1", d);

            c = 3;
            d = "4";
            //ctest = c;
            dtest = d;
            testVar = callRef_3("1", 2, ref c, ref dtest);
            //c = (int)ctest;
            d = (string)dtest;
            Assert.AreEqual(testVar.test, 10);
            Assert.AreEqual(3, c);
            Assert.AreEqual("-1", d);

            c = 3;
            d = "4";
            ctest = c;
            dtest = d;
            testVar = callRef_4("1", 2, ref ctest, ref dtest);
            c = (int)ctest;
            d = (string)dtest;
            Assert.AreEqual(testVar.test, 10);
            Assert.AreEqual(3, c);
            Assert.AreEqual("-1", d);
        }

        [TestMethod]
        public void ClassConstructor()
        {
            var call0 = Dynamic.ConstructorCaller<TestClass>();
            var call1 = Dynamic.ConstructorCaller<TestClass, int>();
            var call2 = Dynamic.ConstructorCaller<TestClass, int, int>();
            var call3 = Dynamic.ConstructorCaller<TestClass, int, int, int>();
            var call4 = Dynamic.ConstructorCaller<TestClass, int, int, int, int>();
            var call5 = Dynamic.ConstructorCaller<TestClass, int, int, int, int, int>();


            Assert.AreEqual(10, call0().test);
            Assert.AreEqual(1, call1(1).test);
            Assert.AreEqual(3, call2(1, 2).test);
            Assert.AreEqual(6, call3(1, 2, 3).test);
            Assert.AreEqual(10, call4(1, 2, 3, 4).test);
            Assert.AreEqual(15, call5(1, 2, 3, 4, 5).test);
        }

        [TestMethod]
        public void StructConstructor()
        {
            var call0 = Dynamic.ConstructorCaller<TestStruct>();
            var call1 = Dynamic.ConstructorCaller<TestStruct, int>();
            var call2 = Dynamic.ConstructorCaller<TestStruct, int, int>();
            var call3 = Dynamic.ConstructorCaller<TestStruct, int, int, int>();
            var call4 = Dynamic.ConstructorCaller<TestStruct, int, int, int, int>();
            var call5 = Dynamic.ConstructorCaller<TestStruct, int, int, int, int, int>();

            Assert.AreEqual(0, call0().test);
            Assert.AreEqual(1, call1(1).test);
            Assert.AreEqual(3, call2(1, 2).test);
            Assert.AreEqual(6, call3(1, 2, 3).test);
            Assert.AreEqual(10, call4(1, 2, 3, 4).test);
            Assert.AreEqual(15, call5(1, 2, 3, 4, 5).test);
        }
    }
}

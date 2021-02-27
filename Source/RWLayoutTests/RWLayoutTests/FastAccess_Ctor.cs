using System;
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
            private TestClass(int a, int b, ref int c)
            {
                test = a + b + c;
                c = a + b;
            }
        }

        class TestStruct
        {
            public int test = 0;

            private TestStruct()
            {
                test = 10;
            }
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

        private delegate TestClass TestClassRefCtor(int a, int b, ref int c);

        [TestMethod]
        public void ClassConstructor()
        {
            var call0 = Dynamic.CreateConstructorCaller<TestClass>();
            var call1 = Dynamic.CreateConstructorCaller<TestClass, int>();
            var call2 = Dynamic.CreateConstructorCaller<TestClass, int, int>();
            var call3 = Dynamic.CreateConstructorCaller<TestClass, int, int, int>();
            var call4 = Dynamic.CreateConstructorCaller<TestClass, int, int, int, int>();
            var call5 = Dynamic.CreateConstructorCaller<TestClass, int, int, int, int, int>();


            Assert.AreEqual(10, call0().test);
            Assert.AreEqual(1, call1(1).test);
            Assert.AreEqual(3, call2(1, 2).test);
            Assert.AreEqual(6, call3(1, 2, 3).test);
            Assert.AreEqual(10, call4(1, 2, 3, 4).test);
            Assert.AreEqual(15, call5(1, 2, 3, 4, 5).test);

            var callRef = Dynamic.CreateConstructorCaller_RenameMe<TestClassRefCtor>();

            int c = 1;
            var testVar = callRef(2, 3, ref c);
            Assert.AreEqual(testVar.test, 6);
            Assert.AreEqual(5, c);
        }

        [TestMethod]
        public void StructConstructor()
        {
            var call0 = Dynamic.CreateConstructorCaller<TestStruct>();
            var call1 = Dynamic.CreateConstructorCaller<TestStruct, int>();
            var call2 = Dynamic.CreateConstructorCaller<TestStruct, int, int>();
            var call3 = Dynamic.CreateConstructorCaller<TestStruct, int, int, int>();
            var call4 = Dynamic.CreateConstructorCaller<TestStruct, int, int, int, int>();
            var call5 = Dynamic.CreateConstructorCaller<TestStruct, int, int, int, int, int>();

            Assert.AreEqual(10, call0().test);
            Assert.AreEqual(1, call1(1).test);
            Assert.AreEqual(3, call2(1, 2).test);
            Assert.AreEqual(6, call3(1, 2, 3).test);
            Assert.AreEqual(10, call4(1, 2, 3, 4).test);
            Assert.AreEqual(15, call5(1, 2, 3, 4, 5).test);
        }
    }
}

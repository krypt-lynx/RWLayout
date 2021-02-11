using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayoutTests
{
    [TestClass]
    public class PerfTest
    {
        class TargetClass
        {
            public int Field;
            public int Property { get; set; }

        }

        const int repeats = 5000000;

        [TestMethod]
        public void _0RepeatFieldDirectAccess()
        {
            var test = new TargetClass();
            int tmp = 0;
                      
            for (int i = 0; i < repeats; i++)
            {
                test.Field = i;
                tmp = test.Field;
            }
        }

        [TestMethod]
        public void _1RepeatFieldReflection()
        {
            var test = new TargetClass();
            int tmp = 0;

            var get = Reflection.CreateGetFieldValueDelegate<int>(typeof(TargetClass).GetField("Field"));
            var set = Reflection.CreateSetFieldValueDelegate<int>(typeof(TargetClass).GetField("Field"));

            for (int i = 0; i < repeats; i++)
            {
                set(test, i);
                tmp = get(test);
            }
        }

        [TestMethod]
        public void _2RepeatFieldILEmit()
        {
            var test = new TargetClass();
            int tmp = 0;

            var get = ILEmit.CreateGetFieldValueDelegate<int>(typeof(TargetClass).GetField("Field"));
            var set = ILEmit.CreateSetFieldValueDelegate<int>(typeof(TargetClass).GetField("Field"));

            for (int i = 0; i < repeats; i++)
            {
                set(test, i);
                tmp = get(test);
            }
        }



        [TestMethod]
        public void _0RepeatPropertyDirectAccess()
        {
            var test = new TargetClass();
            int tmp = 0;

            for (int i = 0; i < repeats; i++)
            {
                test.Property = i;
                tmp = test.Property;
            }
        }

        [TestMethod]
        public void _1RepeatPropertyReflection()
        {
            var test = new TargetClass();
            int tmp = 0;

            var get = Reflection.CreateGetPropertyValueDelegate<int>(typeof(TargetClass).GetProperty("Property"));
            var set = Reflection.CreateSetPropertyValueDelegate<int>(typeof(TargetClass).GetProperty("Property"));

            for (int i = 0; i < repeats; i++)
            {
                set(test, i);
                tmp = get(test);
            }
        }

        [TestMethod]
        public void _2RepeatPropertyILEmit()
        {
            var test = new TargetClass();
            int tmp = 0;

            var get = ILEmit.CreateGetPropertyValueDelegate<int>(typeof(TargetClass).GetProperty("Property"));
            var set = ILEmit.CreateSetPropertyValueDelegate<int>(typeof(TargetClass).GetProperty("Property"));

            for (int i = 0; i < repeats; i++)
            {
                set(test, i);
                tmp = get(test);
            }
        }
    }


    static class Reflection
    {
        public static Func<object, T> CreateGetPropertyValueDelegate<T>(PropertyInfo prop)
        {
            return (x) => (T)prop.GetValue(x);
        }

        public static Func<object, T> CreateGetFieldValueDelegate<T>(FieldInfo field)
        {       
            return (x) => (T)field.GetValue(x);
        }


        public static Action<object, T> CreateSetPropertyValueDelegate<T>(PropertyInfo prop)
        {
            return (o, x) => prop.SetValue(o, x);
        }

        public static Action<object, T> CreateSetFieldValueDelegate<T>(FieldInfo field)
        {
            return (o, x) => field.SetValue(o, x);
        }
    }


    static class ILEmit
    {
        public static Func<object, T> CreateGetPropertyValueDelegate<T>(PropertyInfo prop)
        {
            DynamicMethod getter = new DynamicMethod($"get_{prop.DeclaringType.Name}_{prop.Name}", typeof(T), new Type[] { typeof(object) }, true);
            MethodInfo method = prop.GetGetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            if (method.IsStatic)
            {
                gen.Emit(OpCodes.Call, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, prop.DeclaringType);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }

            return (Func<object, T>)getter.CreateDelegate(typeof(Func<object, T>));
        }

        public static Func<object, T> CreateGetFieldValueDelegate<T>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(T), new Type[] { typeof(object) }, true);
            ILGenerator gen = getter.GetILGenerator();


            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, field);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, field.DeclaringType);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ret);
            }

            return (Func<object, T>)getter.CreateDelegate(typeof(Func<object, T>));
        }

        public static Action<object, T> CreateSetPropertyValueDelegate<T>(PropertyInfo prop)
        {
            DynamicMethod getter = new DynamicMethod($"set_{prop.DeclaringType.Name}_{prop.Name}", null, new Type[] { typeof(object), typeof(T) }, true);
            MethodInfo method = prop.GetSetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            if (method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Call, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, prop.DeclaringType);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }

            return (Action<object, T>)getter.CreateDelegate(typeof(Action<object, T>));
        }

        public static Action<object, T> CreateSetFieldValueDelegate<T>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(object), typeof(T) }, true);
            ILGenerator gen = getter.GetILGenerator();

            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stsfld, field);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, field.DeclaringType);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, field);
                gen.Emit(OpCodes.Ret);
            }

            return (Action<object, T>)getter.CreateDelegate(typeof(Action<object, T>));
        }
    }

}

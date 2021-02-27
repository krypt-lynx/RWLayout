using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2.FastAccess;
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
        public PerfTest()
        {

        }

        class TargetClass
        {
            public int Field;
            public int Property { get; set; }
        }

        const int repeats = 10000000;

        [TestMethod]
        public void Field_0_RepeatDirectAccess()
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
        public void Field_1_RepeatReflection()
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
        public void Field_2_RepeatILEmit()
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
        public void Property_0_RepeatDirectAccess()
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
        public void Property_1_RepeatReflection()
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
        public void Property_2_RepeatILEmit()
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

        [TestMethod]
        public void Property_3_RepeatRWLFastAccessILEmit()
        {
            var test = new TargetClass();
            int tmp = 0;

            var get = Dynamic.InstanceGetProperty<TargetClass, int>("Property");
            var set = Dynamic.InstanceSetProperty<TargetClass, int>("Property");

            for (int i = 0; i < repeats; i++)
            {
                set(test, i);
                tmp = get(test);
                if (i != tmp)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void Property_4_RepeatHarmony()
        {
            var test = new TargetClass();
            int tmp = 0;

            var get = Harmony.CreateGetPropertyValueDelegate<TargetClass, int>(typeof(TargetClass).GetProperty("Property"));
            var set = Harmony.CreateSetPropertyValueDelegate<TargetClass, int>(typeof(TargetClass).GetProperty("Property"));

            for (int i = 0; i < repeats; i++)
            {
                set(test, i);
                tmp = get(test);
                if (i != tmp)
                {
                    Assert.Fail();
                }
            }
        }


        [TestMethod]
        public void Ctor_0_RepeatDirectAccess()
        {
            for (int i = 0; i < repeats; i++)
            {
                new TargetClass();
            }
        }

        [TestMethod]
        public void Ctor_1_RepeatReflection()
        {
            var ctor = typeof(TargetClass).GetConstructor(Type.EmptyTypes);

            for (int i = 0; i < repeats; i++)
            {
                ctor.Invoke(new object[0]);
            }
        }

        public static class CtorStore<T>
        {
            static Func<T> cache;
            static public T CallCtor()
            {
                if (cache == null)
                {
                    cache = Dynamic.CreateConstructorCaller<T>();
                }
                return cache();
            }
        }
        public static class CtorStore
        {
            static Dictionary<Type, Func<object>> cache = new Dictionary<Type, Func<object>>();
            static public object CallCtor(Type type)
            {
                Func<object> ctor;
                if (!cache.TryGetValue(type, out ctor))
                {
                    ctor = (Func<object>)Dynamic.CreateConstructorCaller(typeof(Func<object>), type.GetConstructor(Type.EmptyTypes));
                    cache[type] = ctor;
                }
                return ctor();
            }
        }

        [TestMethod]
        public void Ctor_2_RepeatILEmit()
        {
            var ctor = Dynamic.CreateConstructorCaller<TargetClass>();
            for (int i = 0; i < repeats; i++)
            {
                ctor();
            }
        }

        [TestMethod]
        public void Ctor_2_1_RepeatILEmitType()
        {
            //var ctor = Dynamic.CreateConstructorCaller<TargetClass>();
            var type = typeof(TargetClass);
            for (int i = 0; i < repeats; i++)
            {
                CtorStore.CallCtor(type);
            }
        }

        [TestMethod]
        public void Ctor_2_2_RepeatILEmitGeneric()
        {
            //var ctor = Dynamic.CreateConstructorCaller<TargetClass>();
            for (int i = 0; i < repeats; i++)
            {
                CtorStore<TargetClass>.CallCtor();
            }
        }

        [TestMethod]
        public void Ctor_3_RepeatActivator()
        {
            var ctor = Dynamic.CreateConstructorCaller<TargetClass>();
            for (int i = 0; i < repeats; i++)
            {
                Activator.CreateInstance<TargetClass>();
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
            DynamicMethod getter = new DynamicMethod($"get_{prop.DeclaringType.Name}_{prop.Name}", typeof(T), new Type[] { typeof(object) }, typeof(PerfTest), true);
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
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(T), new Type[] { typeof(object) }, typeof(PerfTest), true);
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
            DynamicMethod getter = new DynamicMethod($"set_{prop.DeclaringType.Name}_{prop.Name}", null, new Type[] { typeof(object), typeof(T) }, typeof(PerfTest), true);
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
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(object), typeof(T) }, typeof(PerfTest), true);
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


    static class Harmony
    {
        public static Func<TInstance, T> CreateGetPropertyValueDelegate<TInstance, T>(PropertyInfo prop)
        {
            MethodInfo method = prop.GetGetMethod(true);

            return HarmonyLib.AccessTools.MethodDelegate<Func<TInstance, T>>(method);

        }


        public static Action<TInstance, T> CreateSetPropertyValueDelegate<TInstance, T>(PropertyInfo prop)
        {
            MethodInfo method = prop.GetSetMethod(true);

            return HarmonyLib.AccessTools.MethodDelegate<Action<TInstance, T>>(method);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
    public partial class Dynamic
    {
        public static Func<T> CreateConstructorCaller<T>()
        {
           var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
            return (Func<T>)CreateConstructorCaller(typeof(Func<T>), constructor);
        }

        public static Func<TArg0, T> CreateConstructorCaller<T, TArg0>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0) }, null);
            return (Func<TArg0, T>)CreateConstructorCaller(typeof(Func<TArg0, T>), constructor);
        }

        public static Func<TArg0, TArg1, T> CreateConstructorCaller<T, TArg0, TArg1>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1) }, null);
            return (Func<TArg0, TArg1, T>)CreateConstructorCaller(typeof(Func<TArg0, TArg1, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, T> CreateConstructorCaller<T, TArg0, TArg1, TArg2>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2) }, null);
            return (Func<TArg0, TArg1, TArg2, T>)CreateConstructorCaller(typeof(Func<TArg0, TArg1, TArg2, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, T> CreateConstructorCaller<T, TArg0, TArg1, TArg2, TArg3>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3) }, null);
            return (Func<TArg0, TArg1, TArg2, TArg3, T>)CreateConstructorCaller(typeof(Func<TArg0, TArg1, TArg2, TArg3, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, T> CreateConstructorCaller<T, TArg0, TArg1, TArg2, TArg3, TArg4>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) }, null);
            return (Func<TArg0, TArg1, TArg2, TArg3, TArg4, T>)CreateConstructorCaller(typeof(Func<TArg0, TArg1, TArg2, TArg3, TArg4, T>), constructor);
        }

        public static TDelegate CreateConstructorCaller_RenameMe<TDelegate>() where TDelegate : Delegate
        {
            var methodInfo = typeof(TDelegate).GetMethod("Invoke");
            if (methodInfo.ReturnType == typeof(void))
            {
                throw new ArgumentException("TDelegate is a void delegate");
            }

            var instanceType = methodInfo.ReturnType;
            var constructor = instanceType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, methodInfo.GetParameters().Select(x => x.ParameterType).ToArray(), null);

            return (TDelegate)CreateConstructorCaller(typeof(TDelegate), constructor);
        }

        public static Delegate CreateConstructorCaller(Type delegateType, ConstructorInfo constructor)
        {
            return CreateConstructorCaller(delegateType, constructor, constructor.DeclaringType, constructor.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Delegate CreateConstructorCaller(Type delegateType, ConstructorInfo constructor, Type instanceType, params Type[] args)
        {
            DynamicMethod wrapper = new DynamicMethod($"ctor_{constructor.DeclaringType.Name}_{constructor.Name}", instanceType, args, typeof(Dynamic), true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (instanceType.IsValueType && args.Length == 0)
            {
                gen.Emit(OpCodes.Ldloca_S, (byte)0);
                gen.Emit(OpCodes.Initobj, instanceType);
                gen.Emit(OpCodes.Ldloc_0);
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    gen.EmitLdarg((byte)i);
                }
                gen.Emit(OpCodes.Newobj, constructor);
            }
            gen.Emit(OpCodes.Ret);
            var test = wrapper.CreateDelegate(delegateType);
            return wrapper.CreateDelegate(delegateType);
        }
    }
}

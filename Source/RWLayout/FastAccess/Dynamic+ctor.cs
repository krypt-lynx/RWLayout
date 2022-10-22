using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{


    public partial class Dynamic
    {
        public static Func<T> ConstructorCaller<T>()
        {
            if (typeof(T).IsValueType)
            {
                return (Func<T>)ConstructorCaller(typeof(Func<T>), null, typeof(T));
            }
            else
            {
                var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
                if (constructor == null)
                {
                    throw new MemberNotFoundException(".ctor", typeof(T));
                }
                return (Func<T>)ConstructorCaller(typeof(Func<T>), constructor);
            }
        }

        public static Func<TArg0, T> ConstructorCaller<T, TArg0>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0) }, null);
            if (constructor == null)
            {
                throw new MemberNotFoundException(".ctor", typeof(T));
            }
            return (Func<TArg0, T>)ConstructorCaller(typeof(Func<TArg0, T>), constructor);
        }

        public static Func<TArg0, TArg1, T> ConstructorCaller<T, TArg0, TArg1>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1) }, null);
            if (constructor == null)
            {
                throw new MemberNotFoundException(".ctor", typeof(T));
            }
            return (Func<TArg0, TArg1, T>)ConstructorCaller(typeof(Func<TArg0, TArg1, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, T> ConstructorCaller<T, TArg0, TArg1, TArg2>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2) }, null);
            if (constructor == null)
            {
                throw new MemberNotFoundException(".ctor", typeof(T));
            }
            return (Func<TArg0, TArg1, TArg2, T>)ConstructorCaller(typeof(Func<TArg0, TArg1, TArg2, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, T> ConstructorCaller<T, TArg0, TArg1, TArg2, TArg3>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3) }, null);
            if (constructor == null)
            {
                throw new MemberNotFoundException(".ctor", typeof(T));
            }
            return (Func<TArg0, TArg1, TArg2, TArg3, T>)ConstructorCaller(typeof(Func<TArg0, TArg1, TArg2, TArg3, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, T> ConstructorCaller<T, TArg0, TArg1, TArg2, TArg3, TArg4>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) }, null);
            if (constructor == null)
            {
                throw new MemberNotFoundException(".ctor", typeof(T));
            }
            return (Func<TArg0, TArg1, TArg2, TArg3, TArg4, T>)ConstructorCaller(typeof(Func<TArg0, TArg1, TArg2, TArg3, TArg4, T>), constructor);
        }


        public static TDelegate ConstructorCallerFromDelegate<TDelegate>() where TDelegate : Delegate
        {
            var methodInfo = typeof(TDelegate).GetMethod("Invoke");
            if (methodInfo.ReturnType == typeof(void))
            {
                throw new ArgumentException("TDelegate is a void delegate");
            }

            var instanceType = methodInfo.ReturnType;
            var constructor = instanceType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, methodInfo.GetParameters().Select(x => x.ParameterType).ToArray(), null);
            if (constructor == null)
            {
                throw new MemberNotFoundException(".ctor", typeof(TDelegate)); // todo: proper type for error message
            }
            return (TDelegate)ConstructorCaller(typeof(TDelegate), constructor);
        }

        public static TDelegate ConstructorCallerFromDelegate<TDelegate>(ConstructorInfo constructor) where TDelegate : Delegate
        {
            return (TDelegate)ConstructorCaller(typeof(TDelegate), constructor);
        }        
        
        public static Delegate ConstructorCaller(Type delegateType, ConstructorInfo constructor)
        {
            return ConstructorCaller(delegateType, constructor, constructor.DeclaringType);
        }

        public static Delegate ConstructorCaller(Type delegateType, Type valueType)
        {
            return ConstructorCaller(delegateType, null, valueType);
        }
   
        public static Delegate ConstructorCaller(Type delegateType, ConstructorInfo constructor, Type instanceType)
        {
            // arguments checks
            if (!typeof(Delegate).IsAssignableFrom(delegateType))
            {
                throw new ArgumentException("delegateType does not represents a delegate", nameof(delegateType));
            }

            if (instanceType == null && !instanceType.IsValueType)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            if (constructor == null && !instanceType.IsValueType)
            {
                throw new ArgumentException("constructor is null, but instanceType is not a value type", nameof(instanceType));
            }

            // read delegate info
            var methodInfo = delegateType.GetMethod("Invoke");
            var delegateArgs = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray() ?? Type.EmptyTypes;
            var retType = methodInfo.ReturnType != typeof(void) ? methodInfo.ReturnType : null;

            // read constructor info
            var ctorArgs = constructor?.GetParameters().Select(x => x.ParameterType).ToArray() ?? Type.EmptyTypes;

            if (delegateArgs.Length != ctorArgs.Length)
            {
                throw new Exception("deletage's arguments count does not match constructor's arguments count");
            }

            //

            var targetType = constructor?.DeclaringType ?? instanceType;


            DynamicMethod wrapper = new DynamicMethod($"ctor_{targetType.Name}_", retType, delegateArgs, typeof(Dynamic), true);
            IILGenerator gen = wrapper.GetILGenerator().AsInterface();

            if (targetType.IsValueType && ctorArgs.Length == 0)
            {
                CreateInitValueType(targetType, gen);

            }
            else
            {
                List<ILArgBuilder> argBuilders = new List<ILArgBuilder>();

                for (int i = 0; i < ctorArgs.Length; i++)
                {
                    argBuilders.Add(ILMethodBuilder.GetArgBuilder(gen, delegateArgs[i], ctorArgs[i]));
                }

                for (int i = 0; i < ctorArgs.Length; i++)
                {
                    argBuilders[i].Prepare((byte)i);
                }

                for (int i = 0; i < ctorArgs.Length; i++)
                {
                    argBuilders[i].PassArg((byte)i);
                }

                gen.Newobj(constructor);

                for (int i = ctorArgs.Length - 1; i >= 0; i--)
                {
                    argBuilders[i].Finalize_((byte)i);
                }
            }
            gen.Ret();
            return wrapper.CreateDelegate(delegateType);
        }

        private static void CreateInitValueType(Type targetType, IILGenerator gen)
        {
            var _var = gen.DeclareLocal(targetType);
            gen
                .Ldloca_S(_var)
                .Initobj(targetType)
                .Ldloc_0();
        }
    }
}

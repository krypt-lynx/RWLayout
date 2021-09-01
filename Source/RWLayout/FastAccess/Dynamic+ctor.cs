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
    class TestILGenerator
    {
        ILGenerator gen;
        public ILGenerator Gen {
            get
            {
                Debug.WriteLine("direct access to ILGenerator");
                return gen;
            }
        }

        public TestILGenerator(ILGenerator gen)
        {
            Debug.WriteLine($"new generator created");
            this.gen = gen;
        }

        internal LocalBuilder DeclareLocal(Type localType)
        {
            Debug.WriteLine($"declare local: {localType?.Name}");
            return gen.DeclareLocal(localType);
        }

        public virtual void Emit(OpCode opcode, string str)
        {
            Debug.WriteLine($"emit: {opcode} {str}");
            gen.Emit(opcode, str);
        }

        public virtual void Emit(OpCode opcode, FieldInfo field)
        {
            Debug.WriteLine($"emit: {opcode} {field}");
            gen.Emit(opcode, field);
        }

        public virtual void Emit(OpCode opcode, Label[] labels)
        {
            Debug.WriteLine($"emit: {opcode} {labels}"); // todo:
            gen.Emit(opcode, labels);
        }

        public virtual void Emit(OpCode opcode, Label label)
        {
            Debug.WriteLine($"emit: {opcode} {label}");
            gen.Emit(opcode, label);
        }

        public virtual void Emit(OpCode opcode, LocalBuilder local)
        {
            Debug.WriteLine($"emit: {opcode} {local}");
            gen.Emit(opcode, local);
        }

        public virtual void Emit(OpCode opcode, float arg)
        {
            Debug.WriteLine($"emit: {opcode} {arg}");
            gen.Emit(opcode, arg);
        }

        public virtual void Emit(OpCode opcode, byte arg)
        {
            Debug.WriteLine($"emit: {opcode} {arg}");
            gen.Emit(opcode, arg);
        }

        public void Emit(OpCode opcode, sbyte arg)
        {
            Debug.WriteLine($"emit: {opcode} {arg}");
            gen.Emit(opcode, arg);
        }

        public virtual void Emit(OpCode opcode, short arg)
        {
            Debug.WriteLine($"emit: {opcode} {arg}");
            gen.Emit(opcode, arg);
        }

        public virtual void Emit(OpCode opcode, double arg)
        {
            Debug.WriteLine($"emit: {opcode} {arg}");
            gen.Emit(opcode, arg);
        }

        public virtual void Emit(OpCode opcode, MethodInfo meth)
        {
            Debug.WriteLine($"emit: {opcode} {meth}");
            gen.Emit(opcode, meth);
        }

        public virtual void Emit(OpCode opcode, int arg)
        {
            Debug.WriteLine($"emit: {opcode} {arg}");
            gen.Emit(opcode, arg);
        }

        public virtual void Emit(OpCode opcode)
        {
            Debug.WriteLine($"emit: {opcode}");
            gen.Emit(opcode);
        }

        public virtual void Emit(OpCode opcode, long arg)
        {
            Debug.WriteLine($"emit: {opcode} {arg}");
            gen.Emit(opcode, arg);
        }

        public virtual void Emit(OpCode opcode, Type cls)
        {
            Debug.WriteLine($"emit: {opcode} {cls}");
            gen.Emit(opcode, cls);
        }

        public virtual void Emit(OpCode opcode, ConstructorInfo con)
        {
            Debug.WriteLine($"emit: {opcode} {con}");
            gen.Emit(opcode, con);
        }
    }

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
                return (Func<T>)ConstructorCaller(typeof(Func<T>), constructor);
            }
        }

        public static Func<TArg0, T> ConstructorCaller<T, TArg0>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0) }, null);
            return (Func<TArg0, T>)ConstructorCaller(typeof(Func<TArg0, T>), constructor);
        }

        public static Func<TArg0, TArg1, T> ConstructorCaller<T, TArg0, TArg1>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1) }, null);
            return (Func<TArg0, TArg1, T>)ConstructorCaller(typeof(Func<TArg0, TArg1, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, T> ConstructorCaller<T, TArg0, TArg1, TArg2>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2) }, null);
            return (Func<TArg0, TArg1, TArg2, T>)ConstructorCaller(typeof(Func<TArg0, TArg1, TArg2, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, T> ConstructorCaller<T, TArg0, TArg1, TArg2, TArg3>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3) }, null);
            return (Func<TArg0, TArg1, TArg2, TArg3, T>)ConstructorCaller(typeof(Func<TArg0, TArg1, TArg2, TArg3, T>), constructor);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, TArg4, T> ConstructorCaller<T, TArg0, TArg1, TArg2, TArg3, TArg4>()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) }, null);
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
            TestILGenerator gen = new TestILGenerator(wrapper.GetILGenerator());

            if (targetType.IsValueType && ctorArgs.Length == 0)
            {
                var _var = gen.DeclareLocal(targetType);
                gen.Emit(OpCodes.Ldloca_S, _var);
                gen.Emit(OpCodes.Initobj, targetType);
                gen.Emit(OpCodes.Ldloc_0);
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

                gen.Emit(OpCodes.Newobj, constructor);

                for (int i = ctorArgs.Length - 1; i >= 0; i--)
                {
                    argBuilders[i].Finalize_((byte)i);
                }
            }
            gen.Emit(OpCodes.Ret);
            return wrapper.CreateDelegate(delegateType);
        }
      
    }
}

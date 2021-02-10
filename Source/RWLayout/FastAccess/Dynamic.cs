using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
    static class ILGeneratorExtensions
    {
        public static void EmitLdarg(this ILGenerator gen, byte argIndex)
        {
            switch (argIndex)
            {
                case 0:
                    gen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    gen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    gen.Emit(OpCodes.Ldarg_S, argIndex);
                    break;
            }
        }
    }

    public partial class Dynamic
    {
        public static Delegate CreateMethodCaller(Type delegateType, MethodInfo method, Type ret, params Type[] args)
        {
            DynamicMethod wrapper = new DynamicMethod($"method_{method.DeclaringType.Name}_{method.Name}", ret, args, true);
            ILGenerator gen = wrapper.GetILGenerator();

            for (int i = 0; i < args.Length; i++)
            {
                gen.EmitLdarg((byte)i);
            }
            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);

            return wrapper.CreateDelegate(delegateType);
        }
    }

    public class FastAccess2
    {


        // static func

        public static Func<TArg0, TResult> CreateStaticRetMethodWrapper<TArg0, TResult>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", typeof(TResult), new Type[] { typeof(TArg0) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Call, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is not static");
            }

            return (Func<TArg0, TResult>)wrapper.CreateDelegate(typeof(Func<TArg0, TResult>));
        }

        // instance func

        public static Func<TInstance, TResult> CreateInstanceRetMethodWrapper<TInstance, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return CreateInstanceRetMethodWrapper<TInstance, TResult>(method);
        }

        public static Func<TInstance, TArg0, TResult> CreateInstanceRetMethodWrapper<TInstance, TArg0, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return CreateInstanceRetMethodWrapper<TInstance, TArg0, TResult>(method);
        }


        public static Func<TInstance, TResult> CreateInstanceRetMethodWrapper<TInstance, TResult>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", typeof(TResult), new Type[] { typeof(TInstance) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (!method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is static");
            }

            return (Func<TInstance, TResult>)wrapper.CreateDelegate(typeof(Func<TInstance, TResult>));
        }

        public static Func<TInstance, TArg0, TResult> CreateInstanceRetMethodWrapper<TInstance, TArg0, TResult>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", typeof(TResult), new Type[] { typeof(TInstance), typeof(TArg0) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (!method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is static");
            }

            return (Func<TInstance, TArg0, TResult>)wrapper.CreateDelegate(typeof(Func<TInstance, TArg0, TResult>));
        }

        // static action

        public static Action<TArg0, TArg1> CreateStaticVoidMethodWrapper<TArg0, TArg1>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", null, new Type[] { typeof(TArg0), typeof(TArg1) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Call, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is not static");
            }

            return (Action<TArg0, TArg1>)wrapper.CreateDelegate(typeof(Action<TArg0, TArg1>));
        }

        public static Action<TArg0, TArg1, TArg2, TArg3> CreateStaticRetMethodWrapper<TArg0, TArg1, TArg2, TArg3>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", null, new Type[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_3);
                gen.Emit(OpCodes.Call, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is not static");
            }

            return (Action<TArg0, TArg1, TArg2, TArg3>)wrapper.CreateDelegate(typeof(Action<TArg0, TArg1, TArg2, TArg3>));
        }

        // instance action

        public static Action<TInstance> CreateInstanceVoidMethodWrapper<TInstance>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return CreateInstanceVoidMethodWrapper<TInstance>(method);
        }

        public static Action<TInstance> CreateInstanceVoidMethodWrapper<TInstance>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", null, new Type[] { typeof(TInstance) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (!method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is static");
            }

            return (Action<TInstance>)wrapper.CreateDelegate(typeof(Action<TInstance>));
        }

        public static Action<TInstance, TArg0> CreateInstanceVoidMethodWrapper<TInstance, TArg0>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", null, new Type[] { typeof(TInstance), typeof(TArg0) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (!method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is static");
            }

            return (Action<TInstance, TArg0>)wrapper.CreateDelegate(typeof(Action<TInstance, TArg0>));
        }

        public static Action<TInstance, TArg0, TArg1> CreateInstanceVoidMethodWrapper<TInstance, TArg0, TArg1>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", null, new Type[] { typeof(TInstance), typeof(TArg0), typeof(TArg1) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (!method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is static");
            }

            return (Action<TInstance, TArg0, TArg1>)wrapper.CreateDelegate(typeof(Action<TInstance, TArg0, TArg1>));
        }

        // static field
        public static Func<TField> CreateGetStaticFieldWrapper<TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(TField), new Type[] { }, true);
            ILGenerator gen = getter.GetILGenerator();

            if (!field.IsStatic)
            {
                throw new InvalidOperationException($"method {field} is static");
            }
            else
            { 
                gen.Emit(OpCodes.Ldsfld, field);
                gen.Emit(OpCodes.Ret);
            }

            return (Func<TField>)getter.CreateDelegate(typeof(Func<TField>));
        }
        // instance field
    }
}

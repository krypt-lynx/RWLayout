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

        public static void EmitLdarg(this TestILGenerator gen, byte argIndex)
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

    public delegate TValue Getter<in TInstance, out TValue>(TInstance instance) where TInstance : class;
    public delegate void Setter<in TInstance, in TValue>(TInstance instance, TValue value) where TInstance : class;

    public delegate TValue ByRefGetter<TInstance, out TValue>(ref TInstance instance) where TInstance : struct;
    public delegate void ByRefSetter<TInstance, in TValue>(ref TInstance instance, TValue value) where TInstance : struct;


    public partial class Dynamic
    {
        public static TDelegate CreateMethodCaller<TDelegate>(MethodInfo method) where TDelegate : Delegate
        {
            return (TDelegate)CreateMethodCaller(typeof(TDelegate), method);
        }

        public static Delegate CreateMethodCaller(Type delegateType, MethodInfo method)
        {
            return CreateMethodCaller(
                delegateType,
                method,
                method.ReturnType != typeof(void) ? method.ReturnType : null,
                method.IsStatic ? null : (method.DeclaringType.IsValueType ? method.DeclaringType.MakeByRefType() : method.DeclaringType),
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Delegate CreateMethodCaller(Type delegateType, MethodInfo method, Type return_, Type this_, params Type[] args)
        {
            var arguments = Enumerable.Empty<Type>();
            if (this_ != null)
            {
                arguments = arguments.Append(this_);
            }
            arguments = arguments.Concat(args);

            DynamicMethod wrapper = new DynamicMethod($"method_{method.DeclaringType.Name}_{method.Name}", return_, arguments.ToArray(), typeof(Dynamic), true);
            ILGenerator gen = wrapper.GetILGenerator();

            int argIndex = 0;
            if (this_ != null)
            {
                if (this_.IsValueType)
                {
                    // Struct;
                    gen.Emit(OpCodes.Ldarga_S, (byte)argIndex++);
                }
                else 
                {
                    // Class; ref Struct;
                    gen.EmitLdarg((byte)argIndex++);
                }
            }


            for (int i = 0; i < args.Length; i++)
            {
                gen.EmitLdarg((byte)argIndex++);
            }
            
            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);

            return wrapper.CreateDelegate(delegateType);
        }
    }
}

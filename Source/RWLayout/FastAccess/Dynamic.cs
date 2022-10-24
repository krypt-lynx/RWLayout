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
        public static IILGenerator Ldarg(this IILGenerator gen, byte argIndex)
        {
            switch (argIndex)
            {
                case 0:
                    gen.Ldarg_0();
                    break;
                case 1:
                    gen.Ldarg_1();
                    break;
                case 2:
                    gen.Ldarg_2();
                    break;
                case 3:
                    gen.Ldarg_3();
                    break;
                default:
                    gen.Ldarg_S(argIndex);
                    break;
            }

            return gen;
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
            // arguments checks
            if (!typeof(Delegate).IsAssignableFrom(delegateType))
            {
                throw new ArgumentException("delegateType does not represents a delegate", nameof(delegateType));
            }

            var methodInfo = delegateType.GetMethod("Invoke");
            var delegateArgs = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray() ?? Type.EmptyTypes;
            var retType = methodInfo.ReturnType != typeof(void) ? methodInfo.ReturnType : null;

            Type[] methodArgs;
            
            var args = method.GetParameters().Select(x => x.ParameterType);
            if (method.IsStatic)
            {
                methodArgs = args.ToArray();
            }
            else
            {
                Type declType;
                if (method.DeclaringType.IsValueType)
                {
                    declType = method.DeclaringType.MakeByRefType();
                } 
                else
                {
                    declType = method.DeclaringType;
                }
                methodArgs = Enumerable.Concat(declType.Yield(), args).ToArray();
            }

            if (delegateArgs.Length != methodArgs.Length)
            {
                throw new Exception("deletage's arguments count does not match constructor's arguments count");
            }

            var targetType = method.DeclaringType;

            DynamicMethod wrapper = new DynamicMethod($"method_{targetType.Name}_{method.Name}_", retType, delegateArgs, targetType, true);
            IILGenerator gen = wrapper.GetILGenerator().AsInterface();

            List<ILArgBuilder> argBuilders = new List<ILArgBuilder>();

            for (int i = 0; i < methodArgs.Length; i++)
            {
                argBuilders.Add(ILMethodBuilder.GetArgBuilder(gen, delegateArgs[i], methodArgs[i]));
            }

            for (int i = 0; i < methodArgs.Length; i++)
            {
                argBuilders[i].Prepare((byte)i);
            }

            for (int i = 0; i < methodArgs.Length; i++)
            {
                argBuilders[i].PassArg((byte)i);
            }

            if (method.IsVirtual)
            {
                gen.Callvirt(method);
            } 
            else
            {
                gen.Call(method);
            }

            for (int i = methodArgs.Length - 1; i >= 0; i--)
            {
                argBuilders[i].Finalize_((byte)i);
            }

            gen.Ret();

            return wrapper.CreateDelegate(delegateType);
        }
    }
}

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

            // read delegate info
            var methodInfo = delegateType.GetMethod("Invoke");
            var delegateArgs = methodInfo?.GetParameters().Select(x => x.ParameterType).ToArray() ?? Type.EmptyTypes;
            var retType = methodInfo.ReturnType != typeof(void) ? methodInfo.ReturnType : null;

            // read method info
            var methodArgs = method.GetParameters().Select(x => x.ParameterType).ToArray();
            var argsCount = methodArgs.Length;

            //

            Type this_ = method.IsStatic ? null : (method.DeclaringType.IsValueType ? method.DeclaringType.MakeByRefType() : method.DeclaringType);

            var arguments = Enumerable.Empty<Type>();
            if (this_ != null)
            {
                arguments = arguments.Append(this_);
                argsCount++;
            }
            arguments = arguments.Concat(methodArgs);

            if (delegateArgs.Length != argsCount)
            {
                throw new Exception("deletage's arguments count does not match method's arguments count");
            }

            DynamicMethod wrapper = new DynamicMethod($"method_{method.DeclaringType.Name}_{method.Name}", retType, arguments.ToArray(), method.DeclaringType, true);
            IILGenerator gen = wrapper.GetILGenerator().AsInterface();



            int argIndex = 0;
            if (this_ != null)
            {
                if (this_.IsValueType)
                {
                    // Struct;
                    gen.Ldarga_S((byte)argIndex++);
                }
                else 
                {
                    // Class; ref Struct;
                    gen.Ldarg((byte)argIndex++);
                }
            }


            for (int i = 0; i < methodArgs.Length; i++)
            {
                gen.Ldarg((byte)argIndex++);
            }
            
            gen
                .Call(method)
                .Ret();

            return wrapper.CreateDelegate(delegateType);
        }
    }
}

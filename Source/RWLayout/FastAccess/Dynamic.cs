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

        public static void EmitLdarga(this ILGenerator gen, byte argIndex)
        {
            gen.Emit(OpCodes.Ldarga_S, argIndex);
        }
    }

    public partial class Dynamic
    {
        public static Delegate CreateMethodCaller(Type delegateType, MethodInfo method, Type return_, Type this_, params Type[] args)
        {
            var arguments = Enumerable.Empty<Type>();
            if (this_ != null)
            {
                arguments = arguments.Append(this_);
            }
            arguments = arguments.Concat(args);

            DynamicMethod wrapper = new DynamicMethod($"method_{method.DeclaringType.Name}_{method.Name}", return_, arguments.ToArray(), true);
            ILGenerator gen = wrapper.GetILGenerator();

            int argIndex = 0;
            if (this_ != null)
            {
                if (this_.IsValueType)
                {
                    gen.EmitLdarga((byte)argIndex++);
                }
                else 
                {
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

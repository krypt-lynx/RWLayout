using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
    static class ILMethodBuilder
    {
        static Dictionary<Func<Type, Type, bool>, Type> ilBuilders = new Dictionary<Func<Type, Type, bool>, Type>
                {
                    { PassthroughILBuilder.WillHandle, typeof(PassthroughILBuilder) },
                    { CastILBuilder.WillHandle, typeof(CastILBuilder) },
                    { UnboxILBuilder.WillHandle, typeof(UnboxILBuilder)},
                    { ByRefClassILBuilder.WillHandle, typeof(ByRefClassILBuilder) },
                    { ByRefValueTypeILBuilder.WillHandle, typeof(ByRefValueTypeILBuilder) },
                };

        public static ILArgBuilder GetArgBuilder(IILGenerator gen, Type delegateArg, Type targetArg) 
        {
            foreach (var builder in ilBuilders)
            {
                if (builder.Key(delegateArg, targetArg))
                {
                    return (ILArgBuilder)Activator.CreateInstance(builder.Value, gen, delegateArg, targetArg);
                }
            }

            throw new NotSupportedException($"argument converstion from {delegateArg} to {targetArg} is not supported");
        }
    }
}

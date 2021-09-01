using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{

    static class ILGeneratorEmit
    {
        public static IILGenerator Ldfld(this IILGenerator gen, FieldInfo field) { gen.Emit(OpCodes.Ldfld, field); return gen; }
        public static IILGenerator Ldsfld(this IILGenerator gen, FieldInfo field) { gen.Emit(OpCodes.Ldsfld, field); return gen; }
        
        public static IILGenerator Ldstr(this IILGenerator gen, string str) { gen.Emit(OpCodes.Ldstr, str); return gen; }

        public static IILGenerator Ldc_I4_0(this IILGenerator gen) { gen.Emit(OpCodes.Ldc_I4_0); return gen; }
        public static IILGenerator Ldc_I4_1(this IILGenerator gen) { gen.Emit(OpCodes.Ldc_I4_1); return gen; }
        public static IILGenerator Ldc_I4_2(this IILGenerator gen) { gen.Emit(OpCodes.Ldc_I4_2); return gen; }
        public static IILGenerator Ldc_I4_3(this IILGenerator gen) { gen.Emit(OpCodes.Ldc_I4_3); return gen; }
        public static IILGenerator Ldc_I4_4(this IILGenerator gen) { gen.Emit(OpCodes.Ldc_I4_4); return gen; }
        public static IILGenerator Ldc_I4_5(this IILGenerator gen) { gen.Emit(OpCodes.Ldc_I4_5); return gen; }



        public static IILGenerator Ldarga_S(this IILGenerator gen, byte index) { gen.Emit(OpCodes.Ldarga_S, index); return gen; }
        

        public static IILGenerator Ldarg_0(this IILGenerator gen) { gen.Emit(OpCodes.Ldarg_0); return gen; }
        public static IILGenerator Ldarg_1(this IILGenerator gen) { gen.Emit(OpCodes.Ldarg_1); return gen; }
        public static IILGenerator Ldarg_2(this IILGenerator gen) { gen.Emit(OpCodes.Ldarg_2); return gen; }
        public static IILGenerator Ldarg_3(this IILGenerator gen) { gen.Emit(OpCodes.Ldarg_3); return gen; }
        public static IILGenerator Ldarg_S(this IILGenerator gen, byte index) { gen.Emit(OpCodes.Ldarg_S, index); return gen; }


        public static IILGenerator Ldloc_0(this IILGenerator gen) { gen.Emit(OpCodes.Ldloc_0); return gen; }
        public static IILGenerator Ldloc_1(this IILGenerator gen) { gen.Emit(OpCodes.Ldloc_1); return gen; }
        public static IILGenerator Ldloc_2(this IILGenerator gen) { gen.Emit(OpCodes.Ldloc_2); return gen; }
        public static IILGenerator Ldloc_3(this IILGenerator gen) { gen.Emit(OpCodes.Ldloc_3); return gen; }
        public static IILGenerator Ldloc_S(this IILGenerator gen, LocalBuilder local) { gen.Emit(OpCodes.Ldloc_S, local); return gen; }

        public static IILGenerator Ldloca_S(this IILGenerator gen, LocalBuilder local) { gen.Emit(OpCodes.Ldloca_S, local); return gen; }

        public static IILGenerator Stfld(this IILGenerator gen, FieldInfo field) { gen.Emit(OpCodes.Stfld, field); return gen; }
        public static IILGenerator Stsfld(this IILGenerator gen, FieldInfo field) { gen.Emit(OpCodes.Stsfld, field); return gen; }

        public static IILGenerator Stloc_0(this IILGenerator gen) { gen.Emit(OpCodes.Stloc_0); return gen; }
        public static IILGenerator Stloc_1(this IILGenerator gen) { gen.Emit(OpCodes.Stloc_1); return gen; }
        public static IILGenerator Stloc_2(this IILGenerator gen) { gen.Emit(OpCodes.Stloc_2); return gen; }
        public static IILGenerator Stloc_S(this IILGenerator gen, LocalBuilder local) { gen.Emit(OpCodes.Stloc_S, local); return gen; }

        public static IILGenerator Br(this IILGenerator gen, Label label) { gen.Emit(OpCodes.Br, label); return gen; }
        public static IILGenerator Brtrue_S(this IILGenerator gen, Label label) { gen.Emit(OpCodes.Brtrue_S, label); return gen; }


        public static IILGenerator Newarr(this IILGenerator gen, Type type) { gen.Emit(OpCodes.Newarr, type); return gen; }

        public static IILGenerator Dup(this IILGenerator gen) { gen.Emit(OpCodes.Dup); return gen; }

        public static IILGenerator Stelem_Ref(this IILGenerator gen) { gen.Emit(OpCodes.Stelem_Ref); return gen; }

        public static IILGenerator Call(this IILGenerator gen, MethodInfo method) { gen.Emit(OpCodes.Call, method); return gen; }
        public static IILGenerator Callvirt(this IILGenerator gen, MethodInfo method) { gen.Emit(OpCodes.Callvirt, method); return gen; }


        public static IILGenerator Ret(this IILGenerator gen) { gen.Emit(OpCodes.Ret); return gen; }

        public static IILGenerator Initobj(this IILGenerator gen, Type targetType) { gen.Emit(OpCodes.Initobj, targetType); return gen; }
        public static IILGenerator Newobj(this IILGenerator gen, ConstructorInfo ctor) { gen.Emit(OpCodes.Newobj, ctor); return gen; }

    }
}

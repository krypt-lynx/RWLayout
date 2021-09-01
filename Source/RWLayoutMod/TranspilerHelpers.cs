using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayoutMod
{
    static class IL
    {
        public static CodeInstruction Ldfld(FieldInfo field) => new CodeInstruction(OpCodes.Ldfld, field);
        public static CodeInstruction Ldstr(String str) => new CodeInstruction(OpCodes.Ldstr, str);

        public static CodeInstruction Ldc_I4_0() => new CodeInstruction(OpCodes.Ldc_I4_0);
        public static CodeInstruction Ldc_I4_1() => new CodeInstruction(OpCodes.Ldc_I4_1);
        public static CodeInstruction Ldc_I4_2() => new CodeInstruction(OpCodes.Ldc_I4_2);
        public static CodeInstruction Ldc_I4_3() => new CodeInstruction(OpCodes.Ldc_I4_3);
        public static CodeInstruction Ldc_I4_4() => new CodeInstruction(OpCodes.Ldc_I4_4);
        public static CodeInstruction Ldc_I4_5() => new CodeInstruction(OpCodes.Ldc_I4_5);


        public static CodeInstruction Ldarg_0() => new CodeInstruction(OpCodes.Ldarg_0);
        public static CodeInstruction Ldarg_1() => new CodeInstruction(OpCodes.Ldarg_1);
        public static CodeInstruction Ldarg_2() => new CodeInstruction(OpCodes.Ldarg_2);
        public static CodeInstruction Ldloc_S(LocalBuilder local) => new CodeInstruction(OpCodes.Ldloc_S, local);

        public static CodeInstruction Stloc_0() => new CodeInstruction(OpCodes.Stloc_0);
        public static CodeInstruction Stloc_1() => new CodeInstruction(OpCodes.Stloc_1);
        public static CodeInstruction Stloc_2() => new CodeInstruction(OpCodes.Stloc_2);
        public static CodeInstruction Stloc_S(LocalBuilder local) => new CodeInstruction(OpCodes.Stloc_S, local);

        public static CodeInstruction Br(Label label) => new CodeInstruction(OpCodes.Br, label);
        public static CodeInstruction Brtrue_S(Label label) => new CodeInstruction(OpCodes.Brtrue_S, label);


        public static CodeInstruction Newarr(Type type) => new CodeInstruction(OpCodes.Newarr, type);

        public static CodeInstruction Dup() => new CodeInstruction(OpCodes.Dup);

        public static CodeInstruction Stelem_Ref() => new CodeInstruction(OpCodes.Stelem_Ref);

        public static CodeInstruction Call(MethodInfo method) => new CodeInstruction(OpCodes.Call, method);
        public static CodeInstruction Callvirt(MethodInfo method) => new CodeInstruction(OpCodes.Callvirt, method);
    }

}

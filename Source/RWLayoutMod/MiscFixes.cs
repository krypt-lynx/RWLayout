using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RWLayout.alpha2;
using RWLayout.alpha2.FastAccess;
using UnityEngine;
using Verse;

namespace RWLayoutMod
{

    static class MiscFixes
    {
        public static void PatchLogMessage(Harmony harmony)
        {
            harmony.Patch(AccessTools.Constructor(typeof(LogMessage), new Type[] { typeof(string) }),
                prefix: new HarmonyMethod(typeof(MiscFixes), nameof(MiscFixes.LogMessage_ctor_string_prefix)));
            harmony.Patch(AccessTools.Constructor(typeof(LogMessage), new Type[] { typeof(LogMessageType), typeof(string), typeof(string) }),
                prefix: new HarmonyMethod(typeof(MiscFixes), nameof(MiscFixes.LogMessage_ctor_LogMessageType_string_string_prefix)));
        }

        internal static bool LogMessage_ctor_string_prefix(ref string text)
        {
            text = text ?? "null";
            return true;
        }

        internal static bool LogMessage_ctor_LogMessageType_string_string_prefix(ref string text)
        {
            text = text ?? "null";
            return true;
        }

        public static void PatchDoResizeControl(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(WindowResizer), nameof(WindowResizer.DoResizeControl)),
                postfix: new HarmonyMethod(typeof(MiscFixes), nameof(MiscFixes.DoResizeControl_postfix)));
        }

        static Getter<WindowResizer, bool> get_WindowResizer_isResizing = Dynamic.InstanceGetField<WindowResizer, bool>("isResizing");
        static Setter<WindowResizer, bool> set_WindowResizer_isResizing = Dynamic.InstanceSetField<WindowResizer, bool>("isResizing");
        internal static void DoResizeControl_postfix(WindowResizer __instance)
        {
            bool isResizing = get_WindowResizer_isResizing(__instance);
            if (isResizing)
            {
                if (!Input.GetMouseButton(0))
                {
                    set_WindowResizer_isResizing(__instance, false);
                }
            }
        }

        public static void TryPatchLoadPatches(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(ModContentPack), "LoadPatches"),
                transpiler: new HarmonyMethod(typeof(MiscFixes), nameof(MiscFixes.LoadPatches_transpiler)));
        }

        private static string GetILCodeReference()
        {
            string reference;
            try
            {
                var name = Assembly.GetExecutingAssembly().GetName().Name;

                using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(name + ".IL_LoadPatches.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    reference = reader.ReadToEnd();
                }
            }
            catch
            {
                reference = null;
            }

            return reference;
        }

        internal static IEnumerable<CodeInstruction> LoadPatches_transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var reference = GetILCodeReference();
            var current = ILSerializer.SerializeInstructions(instructions);

            if (current != reference)
            {
                Log.Warning("ModContentPack.LoadPatches does not matches the reference. Skipping transpiler");
                return instructions;
            }

            try
            {
                return ModifyInstructions(instructions, generator);
            } 
            catch (Exception e)
            {
                LogHelper.LogException("Exception duting code modification", e);
                return instructions;
            }
        }

        private static IEnumerable<CodeInstruction> ModifyInstructions(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstrs = new List<CodeInstruction>();

            var assetLocal = generator.DeclareLocal(typeof(LoadableXmlAsset));
            var docLocal = generator.DeclareLocal(typeof(System.Xml.XmlDocument));
            var elseLabel = generator.DefineLabel();
            var continueLabel = generator.DefineLabel();

            bool patchOnXmlDocAccess = true;

            var e = instructions.GetEnumerator();
            while (e.MoveNext())
            {
                var inst = e.Current;

                if (patchOnXmlDocAccess && inst.ToString() == "ldfld System.Xml.XmlDocument Verse.LoadableXmlAsset::xmlDoc")
                {
                    patchOnXmlDocAccess = false;

                    newInstrs.AddRange(new CodeInstruction[] {
                        // asset = ..     
                        IL.Stloc_S(assetLocal), 

                        // doc = asset.xmlDoc
                        IL.Ldloc_S(assetLocal),
                        inst, //IL.Ldfld(typeof(LoadableXmlAsset).GetField(nameof(LoadableXmlAsset.xmlDoc))),       
                        IL.Stloc_S(docLocal),

                        // if (doc == null) goto elseLabel
                        IL.Ldloc_S(docLocal),
                        IL.Brtrue_S(elseLabel), 

                        // Log.Error($"<..>")
                        IL.Ldc_I4_5(),
                        IL.Newarr(typeof(string)),
                        IL.Dup(),
                        IL.Ldc_I4_0(),
                        IL.Ldstr("[RWL] Game failed to load malformed xml patch file\nXML patch: "),
                        IL.Stelem_Ref(),
                        IL.Dup(),
                        IL.Ldc_I4_1(),
                        IL.Ldloc_S(assetLocal),
                        IL.Callvirt(AccessTools.Method(typeof(LoadableXmlAsset), "get_FullFilePath")),
                        IL.Stelem_Ref(),
                        IL.Dup(),
                        IL.Ldc_I4_2(),
                        IL.Ldstr("\nThe file belongs to mod: "),
                        IL.Stelem_Ref(),
                        IL.Dup(),
                        IL.Ldc_I4_3(),
                        IL.Ldarg_0(),
                        IL.Call(AccessTools.Method(typeof(ModContentPack), "get_PackageIdPlayerFacing")),
                        IL.Stelem_Ref(),
                        IL.Dup(),
                        IL.Ldc_I4_4(),
                        IL.Ldstr("\nPlease, report the issue to mod author\nUnder normal circumstances it will cause game crash and mod list reset due to game bug, but it was patched by RWLayout\n"),
                        IL.Stelem_Ref(),
                        IL.Call(AccessTools.Method(typeof(string), nameof(String.Concat), new Type[] { typeof(string[]) })),
                        IL.Call(AccessTools.Method(typeof(Log), nameof(Log.Error), new Type[] { typeof(string) })),

                        // goto continueLabel 
                        IL.Br(continueLabel),

                        // .. = doc.DocumentElement
                        IL.Ldloc_S(docLocal).WithLabels(elseLabel), // label: elseLabel
                    });
                }
                else if (inst.ToString() == "endfinally NULL [Label9, EX_EndException]")
                {
                    newInstrs.Add(inst);
                    e.MoveNext();
                    inst = e.Current;
                    newInstrs.Add(inst.WithLabels(continueLabel));
                }
                else
                {
                    newInstrs.Add(inst);
                }
            }

            return newInstrs;
        }
    }

}

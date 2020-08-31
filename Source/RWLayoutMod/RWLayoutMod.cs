using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RWLayout.alpha2;
using HarmonyLib;
using UnityEngine;
using Verse;
using System.IO;

namespace RWLayoutMod
{
    public class RWLayoutMod : Mod
    {
        public static string packageIdOfMine = null;
        public static string commitInfo = null;

        public RWLayoutMod(ModContentPack content) : base(content)
        {
            packageIdOfMine = content.PackageId;

            var name = Assembly.GetExecutingAssembly().GetName().Name;

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(name + ".git.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    commitInfo = reader.ReadToEnd()?.TrimEndNewlines();
                }
            } catch
            {
                commitInfo = null;
            }



            Harmony harmony = new Harmony(packageIdOfMine);

            harmony.Patch(AccessTools.Method(typeof(DebugWindowsOpener), "DevToolStarterOnGUI"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), "DevToolStarterOnGUI_postfix"));
            harmony.Patch(AccessTools.Method(typeof(DebugWindowsOpener), "DrawButtons"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), "DrawButtons_postfix"));


            harmony.Patch(AccessTools.Method(typeof(WindowResizer), "DoResizeControl"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), "DoResizeControl_prefix"));

            // bugfixes should not be there but in separate mod, but those are annoying doring development and I'm lazy to create a new one
            // bugfixes:
            // missing null checks in Log class
            harmony.Patch(AccessTools.Constructor(typeof(LogMessage), new Type[] { typeof(string) }),
                prefix: new HarmonyMethod(typeof(HarmonyLogPatches), "LogMessage_ctor_string_prefix"));
            harmony.Patch(AccessTools.Constructor(typeof(LogMessage), new Type[] { typeof(LogMessageType), typeof(string), typeof(string) }),
                prefix: new HarmonyMethod(typeof(HarmonyLogPatches), "LogMessage_ctor_LogMessageType_string_string_prefix"));
            // window resizing issues
            harmony.Patch(AccessTools.Method(typeof(WindowResizer), "DoResizeControl"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), "DoResizeControl_postfix"));

            //harmony.Patch(AccessTools.Method(typeof(GenUI), "Rounded", new Type[] { typeof(Rect) }),
            //    prefix: new HarmonyMethod(typeof(HarmonyPatches), "Rounded_prefix"));
        }


        public override string SettingsCategory()
        {
            return "RWLayout";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            var options = new Listing_Standard();
            options.maxOneColumn = true;

            options.Begin(inRect);


            if (options.ButtonText("break everything"))
            {
                Log.Message(null);
            }

            options.End();
        }


        static class HarmonyLogPatches
        {
            static bool LogMessage_ctor_string_prefix(ref string text)
            {
                text = text ?? "null";
                return true;
            }

            static bool LogMessage_ctor_LogMessageType_string_string_prefix(ref string text)
            {
                text = text ?? "null";
                return true;
            }
        }

        [StaticConstructorOnStartup]
        static class HarmonyPatches
        {

            static bool Rounded_prefix(Rect r, ref Rect __result)
            {
                __result = Rect.MinMaxRect(r.xMin, r.yMin, r.xMax, r.yMax);

                return false;
            }

            static bool DoResizeControl_prefix(WindowResizer __instance, ref Rect __result, Rect winRect)
            {
                if (__instance is CWindowResizer resizer)
                {                    
                    __result = resizer.override_DoResizeControl(winRect);
                    return false;
                } 

                return true;
            }

            static void DoResizeControl_postfix(WindowResizer __instance)
            {
                var field = typeof(WindowResizer).GetField("isResizing", BindingFlags.NonPublic | BindingFlags.Instance);

                bool isResizing = (bool)field.GetValue(__instance);
                if (isResizing)
                {
                    if (!Input.GetMouseButton(0))
                    {
                        field.SetValue(__instance, false);
                    }
                }
            }


            static void DevToolStarterOnGUI_postfix()
            {
                var windows = (List<Window>)typeof(WindowStack).GetField("windows", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Find.WindowStack);
                var devWindow = windows.FirstOrDefault(x => x.ID == -1593759361);

                if (devWindow != null)
                {
                    var oldRect = devWindow.windowRect;
                    devWindow.windowRect = Rect.MinMaxRect(oldRect.xMin, oldRect.yMin, oldRect.xMax + 28, oldRect.yMax);
                }
            }


            static WidgetRow widgetRow(DebugWindowsOpener obj)
            {
                return (WidgetRow)typeof(DebugWindowsOpener).GetField("widgetRow", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
            }

            public static readonly Texture2D OpenInspectSettings = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/ToggleTweak", true);

            static void DrawButtons_postfix(DebugWindowsOpener __instance)
            {                
                if (widgetRow(__instance).ButtonIcon(OpenInspectSettings, "Open RWLayout tests list."))
                {
                    Find.WindowStack.Add(new TestsListWindow());
                }
            }
        }
    }
}

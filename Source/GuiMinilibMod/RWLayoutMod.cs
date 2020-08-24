using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RWLayout_moddiff;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace RWLayoutMod
{
    public class RWLayoutMod : Mod
    {
        public static string packageIdOfMine = null;

        public RWLayoutMod(ModContentPack content) : base(content)
        {
            packageIdOfMine = content.PackageId;

            Harmony harmony = new Harmony(packageIdOfMine);

            harmony.Patch(AccessTools.Method(typeof(DebugWindowsOpener), "DevToolStarterOnGUI"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), "DevToolStarterOnGUI_postfix"));
            harmony.Patch(AccessTools.Method(typeof(DebugWindowsOpener), "DrawButtons"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), "DrawButtons_postfix"));


            harmony.Patch(AccessTools.Method(typeof(WindowResizer), "DoResizeControl"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), "DoResizeControl_prefix"));

            //harmony.Patch(AccessTools.Method(typeof(GenUI), "Rounded", new Type[] { typeof(Rect) }),
            //    prefix: new HarmonyMethod(typeof(HarmonyPatches), "Rounded_prefix"));
        }

        static class HarmonyPatches
        {
            static bool Rounded_prefix(Rect r, ref Rect __result)
            {
                __result = Rect.MinMaxRect(r.xMin, r.yMin, r.xMax, r.yMax);

                return false;
            }

            static bool DoResizeControl_prefix(WindowResizer __instance, ref Rect __result, Rect winRect)
            {
                if (typeof(CWindowResizer).IsAssignableFrom(__instance.GetType()))
                {
                    
                    __result = ((CWindowResizer)__instance).override_DoResizeControl(winRect);
                    return false;
                }

                return true;
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

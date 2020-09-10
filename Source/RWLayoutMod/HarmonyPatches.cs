using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod
{

    static class MiscFixes
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

        static bool Rounded_prefix(Rect r, ref Rect __result)
        {
            __result = r.GUIRounded();

            return false;
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
    }

    static class RWLayoutResizerPatches
    {
        static bool DoResizeControl_prefix(WindowResizer __instance, ref Rect __result, Rect winRect)
        {
            if (__instance is CWindowResizer resizer)
            {
                __result = resizer.override_DoResizeControl(winRect);
                return false;
            }

            return true;
        }

    }

    [StaticConstructorOnStartup]
    static class DevToolsPatches
    {
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

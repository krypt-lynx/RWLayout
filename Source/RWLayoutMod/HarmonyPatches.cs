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

        static FieldInfo WindowResizer_isResizing = typeof(WindowResizer).GetField("isResizing", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        internal static void DoResizeControl_postfix(WindowResizer __instance)
        {
            bool isResizing = (bool)WindowResizer_isResizing.GetValue(__instance);
            if (isResizing)
            {
                if (!Input.GetMouseButton(0))
                {
                    WindowResizer_isResizing.SetValue(__instance, false);
                }
            }
        }
    }
}

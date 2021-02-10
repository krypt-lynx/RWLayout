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

        static Func<WindowResizer, bool> get_WindowResizer_isResizing = RWLayout.alpha2.FastAccess.Dynamic.InstanceGetField<WindowResizer, bool>("isResizing");
        static Action<WindowResizer, bool> set_WindowResizer_isResizing = RWLayout.alpha2.FastAccess.Dynamic.InstanceSetField<WindowResizer, bool>("isResizing");
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
    }
}

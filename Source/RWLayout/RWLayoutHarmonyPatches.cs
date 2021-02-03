﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public static class WindowManager
    {
        internal static HashSet<Type> forceOnGUIFor_WindowTypes = new HashSet<Type>();

    }

    public static class RWLayoutHarmonyPatches
    {

        [Obsolete("patches are invoked by lib itself")]
        public static void PatchOnce()
        {

        }

        internal static void Patch(Harmony harmony)
        {
            // CWindow resize injection
            harmony.Patch(AccessTools.Method(typeof(WindowResizer), nameof(WindowResizer.DoResizeControl)),
                prefix: new HarmonyMethod(typeof(RWLayoutResizerPatches), nameof(RWLayoutResizerPatches.DoResizeControl_prefix)));

            // WindowStack.Add redirect to OnGUI method
            harmony.Patch(AccessTools.Method(typeof(UIRoot), nameof(UIRoot.UIRootOnGUI)),
                prefix: new HarmonyMethod(typeof(WindowStackAddPatches), nameof(WindowStackAddPatches.UIRoot_UIRootOnGUI_prefix)),
                postfix: new HarmonyMethod(typeof(WindowStackAddPatches), nameof(WindowStackAddPatches.UIRoot_UIRootOnGUI_postfix)));
            harmony.Patch(AccessTools.Method(typeof(WindowStack), nameof(WindowStack.Add)),
                prefix: new HarmonyMethod(typeof(WindowStackAddPatches), nameof(WindowStackAddPatches.WindowStack_Add_prefix)));

        }
    }

    static class WindowStackAddPatches
    {
        static int inOnGuiCounter = 0;
        static Queue<Window> windowsQueue = new Queue<Window>();

        internal static bool WindowStack_Add_prefix(Window window)
        {
            if (inOnGuiCounter == 0 && NeedToInvoke(window))
            {
                //Log.Message($"window {window.GetType().Name} queued");
                windowsQueue.Enqueue(window);
                return false;
            }
            else
            {
                //Log.Message($"window {window.GetType().Name} added");
                return true;
            }
        }

        private static bool NeedToInvoke(Window window)
        {
            if (window == null)
            {
                return false;
            }

            if (window is IWindow wnd)
            {
                return wnd.ForceOnGUI;
            }
            else
            {
                return WindowManager.forceOnGUIFor_WindowTypes.Contains(window.GetType());
            }
        }

        internal static void UIRoot_UIRootOnGUI_prefix()
        {
            inOnGuiCounter++;
            while (windowsQueue.Count > 0)
            {
                Find.WindowStack.Add(windowsQueue.Dequeue());
            }
        }

        internal static void UIRoot_UIRootOnGUI_postfix()
        {
            inOnGuiCounter--;
        }
    }

    static class RWLayoutResizerPatches
    {
        internal static bool DoResizeControl_prefix(WindowResizer __instance, ref Rect __result, Rect winRect)
        {
            if (__instance is CWindowResizer resizer)
            {
                __result = resizer.override_DoResizeControl(winRect);
                return false;
            }

            return true;
        }

    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWLayout.alpha2;
using UnityEngine;

namespace RWLayoutMod.NativeWindow
{
    public static class MixedUseExamplePatches
    {
        const float ExtendBy = 100;

        static CGuiRoot host;

        public static void Prepare()
        {
            host = new CGuiRoot();

            var label = host.AddElement(new CLabel
            {
                Title = "This is CLabel, it patched in using Harmony and follows constraints",
                WordWrap = true,
                TextAlignment = TextAnchor.MiddleCenter,
            });

            host.Embed(label);
        }

        static void InitialSize_postfix(ref Vector2 __result)
        {
            __result.y += ExtendBy;
        }

        static void DoWindowContents_prefix(ref Rect inRect)
        {
            host.InRect = Rect.MinMaxRect(inRect.xMin, inRect.yMax - ExtendBy, inRect.xMax, inRect.yMax);
            host.UpdateLayoutIfNeeded();
            host.DoElementContent();

            inRect.yMax -= ExtendBy;
        }

        //   public override void DoWindowContents(Rect inRect)
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod.NativeWindow
{



    class WindowTest_NativeWindow : Window
    {
        const float windowWidth = 300;
        const float windowHeight = 100;

        public override Vector2 InitialSize => new Vector2(windowWidth + Margin * 2, windowHeight + Margin * 2);

        public WindowTest_NativeWindow()
        {
            draggable = true;
            doCloseX = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var textAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;            
            Widgets.Label(inRect, "This is Widgets.Label, it rendered using native methods in native window");
            Text.Anchor = textAnchor;
        }
    }
}

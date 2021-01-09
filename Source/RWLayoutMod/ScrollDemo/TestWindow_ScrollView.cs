using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod.ScrollDemo
{
    class TestWindow_ScrollView : CWindow
    {
        public override void ConstructGui()
        {
            doCloseX = true;
            draggable = true;

            MakeResizable(true, true, ClStrength.Strong);

            base.ConstructGui();
            
            var scroll1 = Gui.AddElement(new CScrollView());
            var test1 = scroll1.Content.AddElement(new CButtonText
            {
                Title = "Test",
                Action = (_) => Log.Message("Click!"),
            });
            
            var scroll2 = Gui.AddElement(new CScrollView());
            var test2 = scroll2.Content.AddElement(new CFrame
            {
                Insets = new EdgeInsets(2, 8, 2, 8)
            });

            scroll1.Content.Embed(test1, new EdgeInsets(10));
            scroll1.Content.ConstrainSize(400, 400);

            scroll2.Content.Embed(test2);
            scroll2.AddConstraint(scroll2.Content.width >= 100);
            scroll2.AddConstraint(scroll2.Content.width ^ scroll2.InnerSizeGuide.width, ClStrength.Medium);
            scroll2.AddConstraint(scroll2.Content.height ^ 400);


            Gui.StackLeft(scroll1, 10, (scroll2, scroll1.width));
            //Gui.StackLeft(true, true, ClStrength.Strong, scroll2);

            InnerSize = new Vector2(300, 300);
        }

        public override void DoWindowContents(Rect inRect)
        {
            //Log.Message(string.Join("\n", Gui.AllConstraintsDebug().Select(x => x.ToString())));
            base.DoWindowContents(inRect);
        }
    }
}

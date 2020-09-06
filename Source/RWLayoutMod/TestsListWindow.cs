using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod
{
    class TestsListWindow : CWindow
    {
        (string, Func<Window>)[] tests =
        {
            ("List View", () => new ListViewDemo.TestWindow_ListView()), 
            ("Window Resize", () => new ResizeDemo.TestWindow_WindowResize()),
            ("Add/Remove Elements", () => new BrickDemo.TestWindow_Brick()),
            ("Show/Hide Elements", () => new VisibilityDemo.TestWindow_Visibility()),
            ("Scroll View", () => new ScrollDemo.TestWindow_ScrollView()),
            ("Mixed Use", () => new NativeWindow.WindowTest_NativeWindow()),
        };


        public override void ConstructGui()
        {
            doCloseX = true;
            draggable = true;


            var titleLabel = Gui.AddElement(new CLabel
            {
                Font = GameFont.Medium,
                Title = "RWLayout tests"
            });

            var buttonsPanel = new CElement();
            buttonsPanel.Name = "buttonsPanel";
            var buttonsColumn1 = buttonsPanel.AddElement(new CElement());
            var buttonsColumn2 = buttonsPanel.AddElement(new CElement());

            var buttons = tests.Select(x => buttonsColumn1.AddElement(new CButton
            {
                Title = x.Item1,
                Action = (_) => Find.WindowStack.Add(x.Item2())
            }))
                .Select(x => (object)(x, 30)).ToArray();


            var debugCheck = buttonsColumn2.AddElement(new CCheckBox
            {
                Title = "Layout Debug",
                Checked = CElement.DebugDraw,
                Changed = (x) => CElement.DebugDraw = x.Checked,
            });

            var versionInfo = Gui.AddElement(new CLabel
            {
                Title = $"RWLayout version: {RWLayoutMod.commitInfo}",
                //Multiline = true,
                Color = new Color(1, 1, 1, 0.5f),
                Font = GameFont.Tiny,
                TextAlignment = TextAnchor.UpperRight,
            });

            buttonsPanel.StackLeft(false, true, ClStrength.Strong,
                (buttonsColumn1, 220), 20, (buttonsColumn2, 150));
            buttonsPanel.AddConstraint(buttonsColumn1.top ^ buttonsPanel.top, ClStrength.Strong);
            buttonsPanel.AddConstraint(buttonsColumn2.top ^ buttonsPanel.top, ClStrength.Strong);
            buttonsColumn1.StackTop(true, true, ClStrength.Strong, buttons);
            buttonsColumn2.StackTop(true, true, ClStrength.Strong,
                (debugCheck, 30));

            buttonsColumn1.AddConstraint(buttonsColumn1.bottom <= buttonsPanel.bottom, ClStrength.Medium);
            buttonsColumn2.AddConstraint(buttonsColumn2.bottom <= buttonsPanel.bottom, ClStrength.Medium);

            Gui.AddElement(buttonsPanel);

            Gui.StackTop(true, true, ClStrength.Strong,
                (titleLabel, 42), buttonsPanel, 20, (versionInfo, versionInfo.intrinsicHeight));

            //Gui.AddConstraint(Gui.height ^ 230 - MarginsSize().x);
        }

        //bool once = false;
        public override void DoWindowContents(Rect inRect)
        {
            //if (!once)
            //{
            //    once = true;
            //    Log.Message(Gui.Solver.ToString());
            //    Log.Message(string.Join("\n", Gui.Solver.AllConstraints().Select(x => x.ToString())));
            //}
            base.DoWindowContents(inRect);
        }
    }
}

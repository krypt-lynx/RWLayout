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
            ("List View", () => new ListingStandardDemo.TestWindow_ListingStandard()),
            ("Window Resize", () => new ResizeDemo.TestWindow_WindowResize()),
            ("Add/Remove Elements", () => new BrickDemo.TestWindow_Brick()),
            ("Show/Hide Elements", () => new VisibilityDemo.TestWindow_Visibility()),
        };

        public override void ConstructGui()
        {
           // EditWindow_CurveEditor

            doCloseX = true;
            draggable = true;

            Gui.Name = "guiroot";

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
                Multiline = true,
                Color = new Color(1, 1, 1, 0.5f),
                Font = GameFont.Tiny,
                TextAlignment = TextAnchor.UpperRight,
            });

            
            buttonsPanel.StackLeft(true, true, ClStrength.Strong,
                (buttonsColumn1, 220), 20, (buttonsColumn2, 150));
            buttonsColumn1.StackTop(true, false, ClStrength.Strong, buttons);
            buttonsColumn2.StackTop(true, false, ClStrength.Strong,
                (debugCheck, 30));

            Gui.AddElement(buttonsPanel);

            Gui.StackTop(true, true, ClStrength.Strong,
                (titleLabel, 42), buttonsPanel, (versionInfo, versionInfo.intrinsicHeight));

            Gui.AddConstraint(Gui.height ^ 230 - MarginsSize().x);
        }

        public override void DoWindowContents(Rect inRect)
        {
            base.DoWindowContents(inRect);
        }
    }
}

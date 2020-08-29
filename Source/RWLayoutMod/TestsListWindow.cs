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
        public override void ConstructGui()
        {

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


            var btn1 = buttonsColumn1.AddElement(new CButton
            {
                Title = "List_Standart",
                Action = (_) =>
                {
                    Find.WindowStack.Add(new ListingStandardDemo.TestWindow_ListingStandard());
                }
            });
            var btn2 = buttonsColumn1.AddElement(new CButton
            {
                Title = "Window Resize",
                Action = (_) =>
                {
                    Find.WindowStack.Add(new ResizeDemo.TestWindow_WindowResize());
                }
            });
            var btn3 = buttonsColumn1.AddElement(new CButton
            {
                Title = "Add/Remove Elements",
                Action = (_) =>
                {
                    Find.WindowStack.Add(new BrickDemo.TestWindow_Brick());
                }
            });
            var btn4 = buttonsColumn1.AddElement(new CButton
            {
                Title = "Show/Hide Elements",
                Action = (_) =>
                {
                    Find.WindowStack.Add(new VisibilityDemo.TestWindow_Visibility());
                }
            });

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
            buttonsColumn1.StackTop(true, false, ClStrength.Strong,
                (btn1, 30), (btn2, 30), (btn3, 30), (btn4, 30));
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

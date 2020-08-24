﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary_moddiff;
using RWLayout_moddiff;
using Verse;

namespace RWLayoutMod
{
    class TestsListWindow : CWindow
    {
        public override void ConstructGui()
        {
            doCloseX = true;

            //gui = new CGuiRoot();
            var titleLabel = Gui.AddElement(new CLabel
            {
                Font = GameFont.Medium,
                Title = "RWLayout tests"
            });

            var buttonsPanel = Gui.AddElement(new CElement());
            var buttonsColumn1 = buttonsPanel.AddElement(new CElement());
            var buttonsColumn2 = buttonsPanel.AddElement(new CElement());


            var btn1 = buttonsColumn1.AddElement(new CButton
            {
                Title = "List_Standart",
                Action = (_) =>
                {
                    Find.WindowStack.Add(new TestWindow_ListingStandard());
                }
            });
            var btn2 = buttonsColumn1.AddElement(new CButton
            {
                Title = "Window Resize",
                Action = (_) =>
                {
                    Find.WindowStack.Add(new TestWindow_WindowResize());
                }
            });
            var btn3 = buttonsColumn1.AddElement(new CButton
            {
                Title = "Add/Remove Elements Demo",
                Action = (_) =>
                {
                    Find.WindowStack.Add(new BrickDemo.TestWindow_Brick());
                }
            });

            var debugCheck = buttonsColumn2.AddElement(new CCheckBox
            {
                Title = "Layout Debug",
                Checked = CElement.DebugDraw,
                Changed = (x) => CElement.DebugDraw = x.Checked,
            });

            Gui.StackTop(true, true, ClStrength.Strong, 
                (titleLabel, 42), buttonsPanel);

            
            buttonsPanel.StackLeft(true, true, ClStrength.Strong,
                (buttonsColumn1, 220), 20, (buttonsColumn2, 150));
            buttonsColumn1.StackTop(true, false, ClStrength.Strong,
                /*(btn1, 30),*/ (btn2, 30), (btn3, 30));
            buttonsColumn2.StackTop(true, false, ClStrength.Strong,
                (debugCheck, 30));

            
            Solver.AddConstraints(Gui.height ^ 300);
        }
    }
}

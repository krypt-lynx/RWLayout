using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary_moddiff;
using RWLayout_moddiff;
using UnityEngine;
using Verse;

namespace RWLayoutMod
{
    class TestWindow_WindowResize : CWindow
    {
        ClLinearEquation widthLock;
        ClLinearEquation heightLock;

        public override void ConstructGui()
        {
            // set window properties
            doCloseX = true;
            resizeable = true;
            draggable = true;

            // set initial window size
            InnerSize = new Vector2(400, 200);

            // limit window size
            Solver.AddConstraints(Gui.width >= 250, Gui.width <= 800);
            Solver.AddConstraints(Gui.height >= 100, Gui.height <= 400);
            // you can use it with any anchor:
            // make window always square:
            //      Solver.AddConstraints(Gui.width ^ Gui.height);
            // make window always fitting label:
            //      Solver.AddConstraints(Gui.width >= label.width);


            // bind windows size to resize input
            Solver.AddConstraint(ClStrength.Weak, Gui.width ^ this.guideWidth);
            Solver.AddConstraint(ClStrength.Weak, Gui.height ^ this.guideHeight);

            // if your window is square you can use this:
            //      Solver.AddConstraint(ClStrength.Weak, Gui.width ^ (this.guideWidth + this.guideHeight) / 2);

            // "Resize me" label
            var label = Gui.AddElement(new CLabel
            {
                Multiline = false,
                Title = "Resize me",
                Font = GameFont.Medium
            });

            // place it in senter of window
            Solver.AddConstraint(Gui.centerX ^ label.centerX);
            Solver.AddConstraint(Gui.centerY ^ label.centerY);

            // set size of label to its minimal fitting size
            Solver.AddConstraint(label.width ^ label.intrinsicWidth);
            Solver.AddConstraint(label.height ^ label.intrinsicHeight);

            // show/hide window title switch
            var titleCkeck = Gui.AddElement(new CCheckBox
            {
                Title = "show title",
                Checked = false,
                Changed = (sender) =>
                {
                    optionalTitle = sender.Checked ? "window resize test" : null;
                }

            });
            // set location and size
            Solver.AddConstraints(Gui.left ^ titleCkeck.left, Gui.bottom ^ titleCkeck.bottom, titleCkeck.width ^ 100, titleCkeck.height ^ 24);

            // reset window size button
            var resetBtn = Gui.AddElement(new CButton
            {
                Title = "reset size",
                Action = (sender) =>
                {
                    InnerSize = new Vector2(400, 200);
                }
            });
            // set location and size
            Solver.AddConstraints(Gui.right ^ resetBtn.right, Gui.bottom ^ resetBtn.bottom, resetBtn.width ^ 100, resetBtn.height ^ 24);

            // lock window width switch
            var lockWidth = Gui.AddElement(new CCheckBox
            {
                Title = "lock width",
                Checked = false,
                Changed = (sender) =>
                {
                    if (sender.Checked)
                    {
                        widthLock = new ClLinearEquation(Gui.width, this.InnerSize.x, ClStrength.Strong);
                        Solver.AddConstraint(widthLock);
                        Solver.Solve();
                    }                    
                    else
                    {
                        Solver.RemoveConstraint(widthLock);
                        Solver.Solve();
                    }
                }
            });
            // set location and size
            Solver.AddConstraints(Gui.left ^ lockWidth.left, Gui.top ^ lockWidth.top, lockWidth.width ^ 100, lockWidth.height ^ 24);

            // lock window height switch
            var lockHeight = Gui.AddElement(new CCheckBox
            {
                Title = "lock height",
                Checked = false,
                Changed = (sender) =>
                {
                    if (sender.Checked)
                    {
                        heightLock = new ClLinearEquation(Gui.height, this.InnerSize.y, ClStrength.Strong);
                        Solver.AddConstraint(heightLock);
                        Solver.Solve();
                    }
                    else
                    {
                        Solver.RemoveConstraint(heightLock);
                        Solver.Solve();
                    }
                }
            });
            // set location and size
            Solver.AddConstraints(Gui.right ^ lockHeight.right, Gui.top ^ lockHeight.top, lockHeight.width ^ 100, lockHeight.height ^ 24);            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod.ResizeDemo
{
    class TestWindow_WindowResize : CWindow
    {
        ClLinearConstraint widthLock;
        ClLinearConstraint heightLock;

        public override void ConstructGui()
        {
            // set window properties
            doCloseX = true;
            MakeResizable(true, true);
            draggable = true;

            // set initial window size
            InnerSize = new Vector2(400, 200);

            // limit window size
            Gui.AddConstraints(Gui.width >= 250, Gui.width <= 800);
            Gui.AddConstraints(Gui.height >= 100, Gui.height <= 400);
            // you can use it with any anchor:
            // make window always square:
            //      AddConstraints(Gui.width ^ Gui.height);
            // make window always fitting label:
            //      AddConstraints(Gui.width >= label.width);


            // bind windows size to resize input


            // if your window is square you can use this:
            //      AddConstraint(ClStrength.Weak, Gui.width ^ (this.guideWidth + this.guideHeight) / 2);
            
            // "Resize me" label
            var label = Gui.AddElement(new CLabel
            {
                //Multiline = false,
                Title = "Resize me",
                Font = GameFont.Medium
            });

            // place it in senter of window
            Gui.AddConstraint(Gui.centerX ^ label.centerX);
            Gui.AddConstraint(Gui.centerY ^ label.centerY);

            // set size of label to its minimal fitting size
            Gui.AddConstraint(label.width ^ label.intrinsicWidth);
            Gui.AddConstraint(label.height ^ label.intrinsicHeight);
            
            // show/hide window title switch
            var titleCkeck = Gui.AddElement(new CCheckboxLabeled
            {
                Title = "show title",
                Checked = false,
                Changed = (_, value) =>
                {
                    optionalTitle = value ? "window resize test" : null;
                }

            });
            // set location and size
            Gui.AddConstraints(
                Gui.left ^ titleCkeck.left,
                Gui.bottom ^ titleCkeck.bottom,
                titleCkeck.width ^ 100,
                titleCkeck.height ^ 24);
            
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
            Gui.AddConstraints(
                Gui.right ^ resetBtn.right, 
                Gui.bottom ^ resetBtn.bottom, 
                resetBtn.width ^ 100,
                resetBtn.height ^ 24);
            
            // lock window width switch
            var lockWidth = Gui.AddElement(new CCheckboxLabeled
            {
                Title = "lock width",
                Checked = false,
                Changed = (_, v) =>
                {
                    if (v)
                    {
                        widthLock = Gui.width ^ this.InnerSize.x;
                        Gui.AddConstraint(widthLock);
                        //Solver.Solve(); // TODO: Regression
                    }
                    else
                    {
                        Gui.RemoveConstraint(widthLock);
                        //Solver.Solve(); // TODO: Regression
                    }
                }
            });
            // set location and size
            Gui.AddConstraints(
                Gui.left ^ lockWidth.left,
                Gui.top ^ lockWidth.top,
                lockWidth.width ^ 100, 
                lockWidth.height ^ 24);

            // lock window height switch
            var lockHeight = Gui.AddElement(new CCheckboxLabeled
            {
                Title = "lock height",
                Checked = false,
                Changed = (_, v) =>
                {
                    if (v)
                    {
                        heightLock = Gui.height ^ this.InnerSize.y;
                        Gui.AddConstraint(heightLock);
                        // Solver.Solve(); // TODO: Regression
                    }
                    else
                    {
                        Gui.RemoveConstraint(heightLock);
                        // Solver.Solve(); // TODO: Regression
                    }
                }
            });
            // set location and size
            Gui.AddConstraints(
                Gui.right ^ lockHeight.right, 
                Gui.top ^ lockHeight.top,
                lockHeight.width ^ 100,
                lockHeight.height ^ 24);     
            
        }
    }
}

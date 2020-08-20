﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using GuiMinilib;
using UnityEngine;
using Verse;

namespace GuiMinilibMod
{
    class TestWindow_WindowResize : CWindow
    {
        ClLinearEquation widthLock;
        ClLinearEquation heightLock;

        public override void ConstructGui()
        {
            doCloseX = true;
            resizeable = true;
            draggable = true;


            InnerSize = new Vector2(400, 200);

            //  Gui.ConstrainSize(500, 300);
            Gui.solver.AddConstraint(Gui.width, (a) => a >= 200, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.height, (a) => a >= 100, ClStrength.Strong);

            Gui.solver.AddConstraint(Gui.width, (a) => a <= 800, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.height, (a) => a <= 400, ClStrength.Strong);

            Gui.solver.AddConstraint(Gui.width, this.width, (a, b) => a == b, ClStrength.Weak);
            Gui.solver.AddConstraint(Gui.height, this.height, (a, b) => a == b, ClStrength.Weak);


            //Gui.solver.AddConstraint(this.width, (a) => a == 400, ClStrength.Medium);
            //Gui.solver.AddConstraint(this.height, (a) => a == 200, ClStrength.Medium);

            var label = Gui.AddElement(new CLabel
            {
                Multiline = false,
                Title = "Resize me",
                Font = GameFont.Medium
            });

            Gui.solver.AddConstraint(Gui.centerX, label.centerX, (a, b) => a == b, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.centerY, label.centerY, (a, b) => a == b, ClStrength.Strong);

            Gui.solver.AddConstraint(label.width, label.intrinsicWidth, (a, b) => a == b, ClStrength.Strong);
            Gui.solver.AddConstraint(label.height, label.intrinsicHeight, (a, b) => a == b, ClStrength.Strong);


            var titleCkeck = Gui.AddElement(new CCheckBox
            {
                Title = "show title",
                Checked = false,
                Changed = (sender) =>
                {
                    optionalTitle = sender.Checked ? "window resize test" : null;
                }
            });
            Gui.solver.AddConstraint(Gui.left, titleCkeck.left, (a, b) => a == b, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.bottom, titleCkeck.bottom, (a, b) => a == b, ClStrength.Strong);
            titleCkeck.ConstrainSize(100, 24);

            var resetBtn = Gui.AddElement(new CButton
            {
                Title = "reset size",
                Action = (sender) =>
                {
                    InnerSize = new Vector2(400, 200);
                }
            });
            Gui.solver.AddConstraint(Gui.right, resetBtn.right, (a, b) => a == b, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.bottom, resetBtn.bottom, (a, b) => a == b, ClStrength.Strong);
            resetBtn.ConstrainSize(100, 24);

            var lockWidth = Gui.AddElement(new CCheckBox
            {
                Title = "lock width",
                Checked = false,
                Changed = (sender) =>
                {
                    if (sender.Checked)
                    {
                        widthLock = new ClLinearEquation(Gui.width, this.InnerSize.x, ClStrength.Strong);
                        Gui.solver.AddConstraint(widthLock);
                        Gui.solver.Solve();
                    }                    
                    else
                    {
                        Gui.solver.RemoveConstraint(widthLock);
                        Gui.solver.Solve();
                    }
                }
            });
            Gui.solver.AddConstraint(Gui.left, lockWidth.left, (a, b) => a == b, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.top, lockWidth.top, (a, b) => a == b, ClStrength.Strong);
            lockWidth.ConstrainSize(100, 24);

            var lockHeight = Gui.AddElement(new CCheckBox
            {
                Title = "lock height",
                Checked = false,
                Changed = (sender) =>
                {
                    if (sender.Checked)
                    {
                        heightLock = new ClLinearEquation(Gui.height, this.InnerSize.y, ClStrength.Strong);
                        Gui.solver.AddConstraint(heightLock);
                        Gui.solver.Solve();
                    }
                    else
                    {
                        Gui.solver.RemoveConstraint(heightLock);
                        Gui.solver.Solve();
                    }
                }
            });
            Gui.solver.AddConstraint(Gui.right, lockHeight.right, (a, b) => a == b, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.top, lockHeight.top, (a, b) => a == b, ClStrength.Strong);
            lockHeight.ConstrainSize(100, 24);


        }
        //   CLabel label;
        public override void DoWindowContents(Rect inRect)
        {
            //Log.Message($"label bounds: {label}");
            base.DoWindowContents(inRect);
        }
    }
}

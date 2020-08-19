using System;
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
        public override void ConstructGui()
        {
            doCloseX = true;
            resizeable = true;
            //  Gui.ConstrainSize(500, 300);
            Gui.solver.AddConstraint(Gui.width, (a) => a >= 300, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.height, (a) => a >= 200, ClStrength.Strong);

            Gui.solver.AddConstraint(Gui.width, (a) => a <= 500, ClStrength.Strong);
            Gui.solver.AddConstraint(Gui.height, (a) => a <= 300, ClStrength.Strong);

            Gui.solver.AddConstraint(Gui.width, this.width, (a, b) => a == b, ClStrength.Weak);
            Gui.solver.AddConstraint(Gui.height, this.height, (a, b) => a == b, ClStrength.Weak);


            var label = Gui.AddElement(new CLabel
            {
                Multiline = false,
                Title = "Resize me",
                Font = GameFont.Medium
            });

            Gui.solver.AddConstraint(Gui.centerX, label.centerX, (a, b) => a == b);
            Gui.solver.AddConstraint(Gui.centerY, label.centerY, (a, b) => a == b);

            Gui.solver.AddConstraint(label.width, label.intrinsicWidth, (a, b) => a == b);
            Gui.solver.AddConstraint(label.height, label.intrinsicHeight, (a, b) => a == b);
        }
     //   CLabel label;
        public override void DoWindowContents(Rect inRect)
        {
            //Log.Message($"label bounds: {label}");
            base.DoWindowContents(inRect);
        }
    }
}

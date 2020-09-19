using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWLayout.alpha2;
using UnityEngine;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Frame 
    /// </summary>
    /// <remarks>For the gods sake, do not use it for that 1px wide white boxes around every element currently infesting mod GUIs. It looks terrible, and if you think otherwise I question your taste.</remarks>
    public class CFrame : CElement
    {
        /// <summary>
        /// Thickness of sides
        /// </summary>
        public EdgeInsets Insets = EdgeInsets.One;
        /// <summary>
        /// Frame Color
        /// </summary>
        public Color Color = new Color(1, 1, 1, 0.3f);

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(Insets.left + Insets.right, Insets.top + Insets.bottom);
        }

        public override void DoContent()
        {
            base.DoContent();

            GuiTools.PushColor(Color);
            GuiTools.Box(BoundsRounded, Insets);
            GuiTools.PopColor();
        }

    }
}

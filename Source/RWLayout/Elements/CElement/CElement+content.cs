// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public partial class CElement
    {
        public Rect Bounds { get; protected set; }
        public Rect BoundsRounded { get; protected set; }
        protected Vector2 drawingOffset;

        public virtual Vector2 tryFit(Vector2 size) { return Vector2.zero; }

        public void DoElementContent()
        {
            if (!Hidden)
            {
                DoContent();

                foreach (var element in Elements)
                {
                    element.DoElementContent();
                }
            }
        }

        public virtual Vector2 toViewCoordinates(CElement source, Vector2 point)
        {
            return point; // todo: implement
        }

        public static bool DebugDraw = false;

        public virtual void DoContent()
        {
            if (DebugDraw)
            {
                GuiTools.UsingColor(new Color(1, 0, 0, 0.2f), () => GuiTools.Box(Bounds, new EdgeInsets(1, 1, 1, 1)));
                GuiTools.UsingColor(new Color(1, 1, 1, 0.2f), () => GuiTools.UsingFont(GameFont.Tiny, () =>
                {
                    Widgets.Label(Bounds, Bounds.ToString());
                }));
            }
        }
    }
}

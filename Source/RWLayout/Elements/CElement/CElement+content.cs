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
        /// <summary>
        /// Bounds of the view
        /// </summary>
        public Rect Bounds { get; protected set; }
        /// <summary>
        /// Rounded to whole numbers bounds of the view
        /// </summary>
        public Rect BoundsRounded { get; protected set; }
        protected Vector2 drawingOffset; // TODO: implement

        /// <summary>
        /// expected to return best fitting size in given place
        /// </summary>
        /// <param name="size"></param>
        /// <returns>used for intrinsic size calculations</returns>
        public virtual Vector2 tryFit(Vector2 size) { return Vector2.zero; }

        /// <summary>
        /// Renders the view and its children
        /// </summary>
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

        /// <summary>
        /// Translates coordinates to basis used in current view
        /// </summary>
        /// <param name="source">the view original coordinates belongs to</param>
        /// <param name="point">coordinates in original view basis</param>
        /// <returns>coordinates in current view basis</returns>
        /// <remarks>not implemented</remarks>
        public virtual Vector2 toViewCoordinates(CElement source, Vector2 point)
        {
            return point; // todo: implement
        }

        /// <summary>
        /// Controls debug overlay
        /// </summary>
        public static bool DebugDraw = false;

        /// <summary>
        /// Renders the view
        /// </summary>
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

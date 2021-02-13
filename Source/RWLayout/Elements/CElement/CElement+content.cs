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
        /// Frame of the view
        /// </summary>
        /// <remarks>
        /// Defines location of element in parent element.
        /// Use this property for operating with element's childrens
        /// </remarks>
        public Rect Frame { get; protected set; }

        /// <summary>
        /// Bounds of the view
        /// </summary>
        /// <remarks>
        /// The rect of the element in its coorditanes system (with respect to Grouping/Clipping)
        /// Use this property to draw element content
        /// </remarks>
        public Rect Bounds { get; protected set; }

        /// <summary>
        /// Rounded to whole numbers bounds of the view
        /// </summary>
        public Rect BoundsRounded { get; protected set; }
        protected Vector2 drawingOffset; // TODO: implement

        protected Vector2 localOffset = Vector2.zero;

        /// <summary>
        /// Offset of coordinates in relation to parent view
        /// </summary>
        public Vector2 LocalOffset { get => localOffset; }

        protected Vector2 globalOffset = Vector2.zero;

        /// <summary>
        /// Offset of parent coordinaties in relation to global origin
        /// </summary>
        /// <remarks>
        /// Full offset is LocalOffset + GlobalOffset
        /// </remarks>
        public Vector2 GlobalOffset { get => globalOffset; }


        /// <summary>
        /// expected to return best fitting size in given place
        /// </summary>
        /// <param name="size"></param>
        /// <returns>used for intrinsic size calculations</returns>
        public virtual Vector2 tryFit(Vector2 size) { return Vector2.zero; }

        /// <summary>
        /// Renders the view and its children. If you want to have a custom render you need to override this method
        /// </summary>
        public virtual void DoElementContent()
        {
            if (!Hidden)
            {

                var clipping = Clipping;
                var groupping = Groupping && !clipping;
                if (clipping)
                {
                    GUI.BeginClip(Frame);
                }
                if (groupping)
                {
                    GUI.BeginGroup(Frame);
                }

                DoContent();

                foreach (var element in Elements)
                {
                    element.DoElementContent();
                }

                if(groupping)
                {
                    GUI.EndGroup();
                }
                if (clipping)
                {
                    GUI.EndClip();
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
        /// Hint for element
        /// </summary>
        public TipSignal? Tip { get; set; } = null;

        /// <summary>
        /// Controls debug overlay
        /// </summary>
        public static bool DebugDraw { get; set; } = false;

        /// <summary>
        /// Renders the view
        /// </summary>
        public virtual void DoContent()
        {
            if (DebugDraw)
            {
                GuiTools.UsingColor(new Color(1, 0, 0, 0.2f), () => GuiTools.Box(Bounds, new EdgeInsets(1)));                
                TooltipHandler.TipRegion(Bounds, $"{NamePrefix()}:\n" +
                    $"Frame: {{x:{Frame.x} y:{Frame.y} w:{Frame.width} h:{Frame.height}}}\n" +
                    $"Bounds: {{x:{Bounds.x} y:{Bounds.y} w:{Bounds.width} h:{Bounds.height}}}");
            }

            if (Tip != null)
            {
                TooltipHandler.TipRegion(BoundsRounded, Tip.Value);
            }
        }
    }
}

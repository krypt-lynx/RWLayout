using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Basic wrapper class for native gui
    /// </summary>
    public class CWidget : CElement
    {
        /// <summary>
        /// shout provide smallest size fitting content of the widget. Argument is current desized size
        /// </summary>
        public Func<Vector2, Vector2> TryFitContent;

        /// <summary>
        /// Do native content. Works in the same way as DoWindowContent of Verse.Window class. First argument is the CWidget itself. Second is CWidget's bounds rounded to whole screen pixels (*not to integers*)
        /// </summary>
        public Action<CWidget, Rect> DoWidgetContent;

        public override Vector2 tryFit(Vector2 size)
        {
            if (TryFitContent == null)
            {
                return base.tryFit(size);
            }
            else
            {
                return TryFitContent(size);
            }
        }

        public override void DoContent()
        {
            base.DoContent();
            DoWidgetContent?.Invoke(this, BoundsRounded);
        }
    }
}

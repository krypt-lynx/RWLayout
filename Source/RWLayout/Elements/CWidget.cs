// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

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
        public Func<Vector2, Vector2> TryFitContect;

        /// <summary>
        /// Do native content. Works in the same way as DoWindowContent of Verse.Window class. First argument is the CWidget itself. Second is CWidget's bounds rounded to whole pixels (*not to integers*)
        /// </summary>
        public Action<CWidget, Rect> DoWidgetContent;

        public override Vector2 tryFit(Vector2 size)
        {
            if (TryFitContect == null)
            {
                return base.tryFit(size);
            }
            else
            {
                return TryFitContect(size);
            }
        }

        public override void DoContent()
        {
            base.DoContent();
            DoWidgetContent?.Invoke(this, BoundsRounded);
        }
    }
}

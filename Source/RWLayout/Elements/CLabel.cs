// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout
{
    public enum CTextAlignment
    {
        Left = 0, 
        Center,
        Right
    }

    public class CLabel : CElement
    {
        public string Title;
        public GameFont Font = GameFont.Small;
        public Color? Color = null;
        public bool Multiline = false;
        /// <summary>
        /// Not implemented (should be used as modifier for tryFit)
        /// </summary>
        public int LineCount = 0;
        /// <summary>
        /// Text alignment
        /// </summary>
        /// <remarks>
        /// Placeholde implementation. Works only for single line labels
        /// </remarks>
        public CTextAlignment TextAlignment = CTextAlignment.Left;

        public override Vector2 tryFit(Vector2 size)
        {
            Vector2 result;
            if (Multiline)
            {
                var y = GuiTools.UsingFont(Font, () => Text.CalcHeight(Title, size.x));
                result = new Vector2(size.x, y);
            }
            else
            {
                result = GuiTools.UsingFont(Font, () => Text.CalcSize(Title));
            }

            //Log.Message($"fitting size of {NamePrefix()} (\"{Title}\"): {result} for {size}");
            return result;
        }

        public override void DoContent()
        {
            base.DoContent();
            GuiTools.FontPush(Font);
            GuiTools.ColorPush(Color);

            switch (TextAlignment)
                {
                case CTextAlignment.Left:
                    Widgets.Label(bounds, Title);
                    break;
                case CTextAlignment.Center:
                    {
                        var size = tryFit(bounds.size);
                        var left = (bounds.width - size.x) / 2;
                        Widgets.Label(Rect.MinMaxRect(bounds.xMin + left, bounds.yMin, bounds.xMax, bounds.yMax), Title);
                    } break;
                case CTextAlignment.Right:
                    {
                        var size = tryFit(bounds.size);
                        var left = bounds.width - size.x;
                        Widgets.Label(Rect.MinMaxRect(bounds.xMin + left, bounds.yMin, bounds.xMax, bounds.yMax), Title);
                    } break;
                default:
                    Widgets.Label(bounds, Title);
                    break;
                }

            GuiTools.ColorPop();
            GuiTools.FontPop();
        }
    }
}

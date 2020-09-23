using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public static partial class GuiTools
    {
        public static void Box(Rect rect, EdgeInsets insets)
        {
            var scale = Prefs.UIScale;

            float left = rect.xMin;
            float top = rect.yMin;
            float right = rect.xMax;
            float bottom = rect.yMax;

            float width = rect.width;
            float height = rect.height;

            if (insets.Left > width)
            {
                insets.Left = width;
            }
            if (insets.Right > width - insets.Left)
            {
                insets.Right = width - insets.Left;
            }

            if (insets.Top > height)
            {
                insets.Top = height;
            }
            if (insets.Bottom > height - insets.Top)
            {
                insets.Bottom = height - insets.Top;
            }

            //    insets.left = GUIScale(insets.left, scale);
            //    insets.top = GUIScale(insets.top, scale);
            //    insets.right = GUIScale(insets.right, scale);
            //    insets.bottom = GUIScale(insets.bottom, scale);

            GUI.DrawTexture(Rect.MinMaxRect(GUIScale(left, scale), GUIScale(top, scale),
                GUIScale((left + insets.Left), scale), GUIScale(bottom, scale)), BaseContent.WhiteTex);
            GUI.DrawTexture(Rect.MinMaxRect(GUIScale((right - insets.Right), scale), GUIScale(top, scale),
               GUIScale((right - insets.Right + insets.Right), scale), GUIScale(bottom, scale)), BaseContent.WhiteTex);
            GUI.DrawTexture(Rect.MinMaxRect(GUIScale((left + insets.Left), scale), GUIScale((top), scale),
                GUIScale((right - insets.Right), scale), GUIScale((top + insets.Top), scale)), BaseContent.WhiteTex);
            GUI.DrawTexture(Rect.MinMaxRect(GUIScale((left + insets.Left), scale), GUIScale((bottom - insets.Bottom), scale),
                GUIScale((right - insets.Right), scale), GUIScale(bottom, scale)), BaseContent.WhiteTex);
        }

    }
}

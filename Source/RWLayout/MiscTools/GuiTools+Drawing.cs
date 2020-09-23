using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.moddiff
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

            if (insets.left > width)
            {
                insets.left = width;
            }
            if (insets.right > width - insets.left)
            {
                insets.right = width - insets.left;
            }

            if (insets.top > height)
            {
                insets.top = height;
            }
            if (insets.bottom > height - insets.top)
            {
                insets.bottom = height - insets.top;
            }

            //    insets.left = GUIScale(insets.left, scale);
            //    insets.top = GUIScale(insets.top, scale);
            //    insets.right = GUIScale(insets.right, scale);
            //    insets.bottom = GUIScale(insets.bottom, scale);

            GUI.DrawTexture(Rect.MinMaxRect(GUIScale(left, scale), GUIScale(top, scale),
                GUIScale((left + insets.left), scale), GUIScale(bottom, scale)), BaseContent.WhiteTex);
            GUI.DrawTexture(Rect.MinMaxRect(GUIScale((right - insets.right), scale), GUIScale(top, scale),
               GUIScale((right - insets.right + insets.right), scale), GUIScale(bottom, scale)), BaseContent.WhiteTex);
            GUI.DrawTexture(Rect.MinMaxRect(GUIScale((left + insets.left), scale), GUIScale((top), scale),
                GUIScale((right - insets.right), scale), GUIScale((top + insets.top), scale)), BaseContent.WhiteTex);
            GUI.DrawTexture(Rect.MinMaxRect(GUIScale((left + insets.left), scale), GUIScale((bottom - insets.bottom), scale),
                GUIScale((right - insets.right), scale), GUIScale(bottom, scale)), BaseContent.WhiteTex);
        }

    }
}

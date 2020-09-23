using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public static partial class GuiTools
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GUIScale(float value, float scale)
        {
            return ((int)(value * scale)) / scale;
        }

        /// <summary>
        /// Expands rect
        /// </summary>
        /// <param name="rect">rect to expand</param>
        /// <param name="insets">side sizes to expand by</param>
        /// <returns></returns>
        public static Rect ExpandedBy(this Rect rect, EdgeInsets insets)
        {
            return Rect.MinMaxRect(rect.xMin - insets.left, rect.yMin - insets.top, rect.xMax + insets.right, rect.yMax + insets.bottom);
        }

        /// <summary>
        /// Contracts rect
        /// </summary>
        /// <param name="rect">rect to contract</param>
        /// <param name="insets">side sizes to contract by</param>
        /// <returns></returns>
        public static Rect ContractedBy(this Rect rect, EdgeInsets insets)
        {
            return Rect.MinMaxRect(rect.xMin + insets.left, rect.yMin + insets.top, rect.xMax - insets.right, rect.yMax - insets.bottom);
        }

        /// <summary>
        /// Coordinates rounded to be whole screen pixels
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect GUIRounded(this Rect rect)
        {
            var scale = Prefs.UIScale;

            return Rect.MinMaxRect(GUIScale(rect.xMin, scale), GUIScale(rect.yMin, scale),
                GUIScale(rect.xMax, scale), GUIScale(rect.yMax, scale));
        }

        /// <summary>
        /// Rect with size rounded to be whole screen pixels
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect GUIRoundedPreserveOrigin(this Rect rect)
        {
            var scale = Prefs.UIScale;

            return new Rect(rect.xMin, rect.yMin,
                GUIScale(rect.width, scale), GUIScale(rect.height, scale));
        }

        /// <summary>
        /// Calculate rect centered insize another rect
        /// </summary>
        /// <param name="bounds">Reference rect</param>
        /// <param name="insets">Side margins</param>
        /// <param name="size">Size of new rect</param>
        /// <returns></returns>
        public static Rect SizeCenteredIn(Rect bounds, EdgeInsets insets, Vector2 size)
        {
            return new Rect(
                bounds.xMin + insets.left + (bounds.width - size.x - insets.left - insets.right) / 2,
                bounds.yMin + insets.top + (bounds.height - size.y - insets.top - insets.bottom) / 2,
                size.x, size.y);
        }

        /// <summary>
        /// Texture size as Vector2
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Size(this Texture2D texture)
        {
            return new Vector2(texture.width, texture.height);
        }
    }
}

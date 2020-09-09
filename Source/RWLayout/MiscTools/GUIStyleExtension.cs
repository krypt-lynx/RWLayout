using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{

    public static class GUIStyleExtension
    {
        // Unity api is insufficient. Time for hacks.


        /// <summary>
        /// returns fitting size of the wrapped text
        /// </summary>
        /// <param name="style">used style</param>
        /// <param name="content">content with text to measure</param>
        /// <param name="size">size to fit</param>
        /// <returns>fitting size</returns>
        public static Vector2 CalcSize(this GUIStyle style, GUIContent content, Vector2 size)
        {
            if (!style.wordWrap)
            {
                return style.CalcSize(content);
            }
            else
            {
                float lower = 0;
                float upper = 0;

                style.CalcMinMaxWidth(content, out lower, out upper);
                if (Prefs.UIScale != 1.0) // fix of upper size being too small
                {
                    var v2 = style.CalcSize(content);
                    upper = Mathf.Max(upper, v2.x);
                }
                // TODO: invalid CalcMinMaxWidth result
                //       hypothesize: CalcMinMaxWidth uses different font texture to calc size. 
                //       question: Why single character ('G') is wrapped? 
                //       Maybe it is possimle to create custom GUIStyle with custom font witting actual size?

                var x = Mathf.Min(upper, size.x);

                var result = new Vector2(x, style.CalcHeight(content, x));
                return result;
            }
        }


        // slow, not in use
        /// <summary>
        /// returns exact fitting size of the wrapped text
        /// </summary>
        /// <param name="style">used style</param>
        /// <param name="content">content with text to measure</param>
        /// <param name="size">size to fit</param>
        /// <returns>fitting size</returns>
        public static Vector2 CalcSize_exact(this GUIStyle style, GUIContent content, Vector2 size)
        {            
            float lower = 0;
            float upper = 0;
            //float upperMax = 0;

            Text.CurFontStyle.CalcMinMaxWidth(content, out lower, out upper);
            //upperMax = upper;


            if (upper <= size.x || lower == upper)
            {
                return new Vector2(upper, Text.CurFontStyle.CalcHeight(content, upper));
            }

            float desired = Text.CurFontStyle.CalcHeight(content, size.x);
            float middle = 0;
            const float epsilon = 0.01f;
            lower = 0;
            while (Mathf.Abs(upper - lower) > epsilon)
            {
                middle = lower + (upper - lower) / 2;
                float current = Text.CurFontStyle.CalcHeight(content, middle);

                if (current > desired)
                {
                    lower = middle;
                } 
                else
                {
                    upper = middle;
                }
            }
            middle = Mathf.Min(Mathf.Ceil(middle + epsilon), size.x);
            return new Vector2(middle, desired);
        }
    }
}

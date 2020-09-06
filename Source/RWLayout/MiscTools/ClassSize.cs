using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.Alpha1.MiscTools
{
    // Unity api is insufficient. Time for hacks.



    public static class GUIStyleExtension
    {
        public static Vector2 CalcSize(this GUIStyle style, GUIContent content, Vector2 size)
        {
            // VS, please, go fuck yourself. Unity debugger crashes without assignments of out variables.
            float lower = 0;
            float upper = 0;

            Text.CurFontStyle.CalcMinMaxWidth(content, out lower, out upper);

            if (upper <= size.x || lower == upper)
            {
                return new Vector2(lower, Text.CurFontStyle.CalcHeight(content, upper));
            }

            float desired = Text.CurFontStyle.CalcHeight(content, size.x);
            float middle = 0;

            while (Mathf.Abs(upper - lower) > 0.0001)
            {
                middle = lower + (upper - lower) / 2;
                float current = Text.CurFontStyle.CalcHeight(content, middle);

                if (current == desired)
                {
                    upper = middle;
                } 
                else
                {
                    lower = middle;
                }
            }

            return new Vector2(middle, desired);



            throw new NotImplementedException();
        }

        // https://stackoverflow.com/questions/967047/how-to-perform-a-binary-search-on-ilistt
        public static Int32 BinarySearchIndexOf<T>(this IList<T> list, T value, IComparer<T> comparer = null)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            comparer = comparer ?? Comparer<T>.Default;

            Int32 lower = 0;
            Int32 upper = list.Count - 1;

            while (lower <= upper)
            {
                Int32 middle = lower + (upper - lower) / 2;
                Int32 comparisonResult = comparer.Compare(value, list[middle]);
                if (comparisonResult == 0)
                    return middle;
                else if (comparisonResult < 0)
                    upper = middle - 1;
                else
                    lower = middle + 1;
            }

            return ~lower;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RWLayout.alpha2
{
    public struct EdgeInsets
    {
        /// <summary>
        /// Creates edge insets
        /// </summary>
        /// <param name="top">left inset</param>
        /// <param name="right">right inset</param>
        /// <param name="bottom">bottom inset</param>
        /// <param name="left">left inset</param>
        public EdgeInsets(float top, float right, float bottom, float left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }
        
        /// <summary>
        /// Creates edge insets of equal value
        /// </summary>
        /// <param name="margin">value of the insets</param>
        public EdgeInsets(float margin)
        {
            Top = margin;
            Right = margin;
            Bottom = margin;
            Left = margin;
        }

        /// <summary>
        /// Creates insets from unity border representation (format used by GUI.DrawTexture)
        /// </summary>
        /// <param name="unityBorders">unity borders</param>
        public EdgeInsets(Vector4 unityBorders)
        {
            // Oops. I have different parameters order.
            // (left, top, right, bottom) 
            //  x     y    z      w
            Top = unityBorders.y;
            Right = unityBorders.z;
            Bottom = unityBorders.w;
            Left = unityBorders.x;            
        }

        /// <summary>
        /// Zero width insets
        /// </summary>
        public static EdgeInsets Zero = new EdgeInsets(0);

        /// <summary>
        /// 1 px width insets
        /// </summary>
        public static EdgeInsets One = new EdgeInsets(1);

        /// <summary>
        /// top inset
        /// </summary>
        public float Top;
        /// <summary>
        /// right inset
        /// </summary>
        public float Right;
        /// <summary>
        /// bottom inset
        /// </summary>
        public float Bottom;
        /// <summary>
        /// left inset
        /// </summary>
        public float Left;

        /// <summary>
        /// total width of insets
        /// </summary>
        public float Width
        {
            get
            {
                return Left + Right;
            }
        }

        /// <summary>
        /// total height of insets
        /// </summary>
        public float Height
        {
            get
            {
                return Top + Bottom;
            }
        }

        /// <summary>
        /// total size of insets
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }


        public override int GetHashCode()
        {
            int iTop = Top.GetHashCode();
            int iRight = Right.GetHashCode();
            iRight = iRight << 8 | iRight >> 24;
            int iBottom = Bottom.GetHashCode();
            iBottom = iBottom << 16 | iBottom >> 16;
            int iLeft = Left.GetHashCode();
            iLeft = iLeft << 24 | iLeft >> 8;

            return iTop ^ iRight ^ iBottom ^ iLeft;
        }

        public override bool Equals(object obj)
        {
            if (obj is EdgeInsets other)
            {
                return
                    Top == other.Top &&
                    Right == other.Right &&
                    Bottom == other.Bottom &&
                    Left == other.Left;
            }
            else
            {
                return false;
            }
        }

        public Vector4 AsVector4()
        {
            // (left, top, right and bottom)
            return new Vector4(Left, Top, Right, Bottom);
        }

        public override string ToString()
        {
            return ((FormattableString)$"{{{Top}, {Right}, {Bottom}, {Left}}}").ToString(CultureInfo.InvariantCulture);
        }


    }
}

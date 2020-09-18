﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2
{
    public struct EdgeInsets
    {
        public EdgeInsets(float top, float right, float bottom, float left)
        {
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.left = left;
        }
        public EdgeInsets(float margin)
        {
            top = margin;
            right = margin;
            bottom = margin;
            left = margin;
        }
        public static EdgeInsets Zero = new EdgeInsets(0);
        public static EdgeInsets One = new EdgeInsets(1);

        public float top, right, bottom, left;


        public override int GetHashCode()
        {
            int iTop = top.GetHashCode();
            int iRight = right.GetHashCode();
            iRight = iRight << 8 | iRight >> 24;
            int iBottom = bottom.GetHashCode();
            iBottom = iBottom << 16 | iBottom >> 16;
            int iLeft = left.GetHashCode();
            iLeft = iLeft << 24 | iLeft >> 8;

            return iTop ^ iRight ^ iBottom ^ iLeft;
        }

        public override bool Equals(object obj)
        {
            if (obj is EdgeInsets other)
            {
                return
                    top == other.top &&
                    right == other.right &&
                    bottom == other.bottom &&
                    left == other.left;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return ((FormattableString)$"{{{top}, {right}, {bottom}, {left}}}").ToString(CultureInfo.InvariantCulture);
        }
    }
}

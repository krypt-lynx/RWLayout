﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.Alpha1
{
    public static class GuiTools
    {
        static Stack<GameFont> fonts = new Stack<GameFont>();
        public static void FontPush(GameFont font)
        {
            fonts.Push(Text.Font);
            Text.Font = font;
        }
        public static void FontPop()
        {
            Text.Font = fonts.Pop();
        }
        public static void UsingFont(GameFont font, Action action)
        {
            FontPush(font);
            action();
            FontPop();
        }
        public static T UsingFont<T>(GameFont font, Func<T> func)
        {
            FontPush(font);
            T result = func();
            FontPop();
            return result;
        }


        static Stack<Color> colors = new Stack<Color>();
        public static void ColorPush(Color? color)
        {
            colors.Push(GUI.color);
            if (color.HasValue)
            {
                GUI.color = color.Value;
            }
        }
        public static void ColorPop()
        {
            GUI.color = colors.Pop();
        }

        public static void UsingColor(Color? color, Action action)
        {
            ColorPush(color);
            action();
            ColorPop();
        }
        public static T UsingColor<T>(Color? color, Func<T> func)
        {
            ColorPush(color);
            T result = func();
            ColorPop();
            return result;
        }


        static Stack<TextAnchor> textAnchors = new Stack<TextAnchor>();
        public static void TextAnchorPush(TextAnchor anchor)
        {
            textAnchors.Push(Text.Anchor);
            Text.Anchor = anchor;
        }
        public static void TextAnchorPop()
        {
            Text.Anchor = textAnchors.Pop();
        }
        public static void UsingTextAnchor(TextAnchor anchor, Action action)
        {
            TextAnchorPush(anchor);
            action();
            TextAnchorPop();
        }
        public static T UsingTextAnchor<T>(TextAnchor anchor, Func<T> func)
        {
            TextAnchorPush(anchor);
            T result = func();
            TextAnchorPop();
            return result;
        }


        public static void Box(Rect rect, EdgeInsets insets)
        {            
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

            GUI.DrawTexture(new Rect(left, top, (float)insets.left, height), BaseContent.WhiteTex);
            GUI.DrawTexture(new Rect(right - (float)insets.right, top, (float)insets.right, height), BaseContent.WhiteTex);
            GUI.DrawTexture(new Rect(left + (float)insets.left, top, width - (float)insets.left - (float)insets.right, (float)insets.top), BaseContent.WhiteTex);
            GUI.DrawTexture(new Rect(left + (float)insets.left, bottom - (float)insets.bottom, width - (float)insets.left - (float)insets.right, (float)insets.bottom), BaseContent.WhiteTex);
        }

        public static Rect ExpandedBy(this Rect rect, EdgeInsets insets)
        {
            return Rect.MinMaxRect(rect.xMin - insets.left, rect.yMin - insets.top, rect.xMax + insets.right, rect.yMax + insets.bottom);
        }

        // Token: 0x06001CF4 RID: 7412 RVA: 0x000B0312 File Offset: 0x000AE512
        public static Rect ContractedBy(this Rect rect, EdgeInsets insets)
        {
            return Rect.MinMaxRect(rect.xMin + insets.left, rect.yMin + insets.top, rect.xMax - insets.right, rect.yMax - insets.bottom);
        }

        public static Rect Rounded2(this Rect rect)
        {
            return Rect.MinMaxRect((int)rect.xMin, (int)rect.yMin, (int)rect.xMax, (int)rect.yMax);
        }
    }
}
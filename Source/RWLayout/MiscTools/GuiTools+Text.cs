// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

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
        static Stack<GameFont> fonts = new Stack<GameFont>();
        public static void PushFont(GameFont font)
        {
            fonts.Push(Text.Font);
            Text.Font = font;
        }
        public static void PopFont()
        {
            Text.Font = fonts.Pop();
        }
        public static void UsingFont(GameFont font, Action action)
        {
            PushFont(font);
            action();
            PopFont();
        }
        public static T UsingFont<T>(GameFont font, Func<T> func)
        {
            PushFont(font);
            T result = func();
            PopFont();
            return result;
        }

        static Stack<Color> colors = new Stack<Color>();
        public static void PushColor(Color? color)
        {
            colors.Push(GUI.color);
            if (color.HasValue)
            {
                GUI.color = color.Value;
            }
        }
        public static void PopColor()
        {
            GUI.color = colors.Pop();
        }
        public static void UsingColor(Color? color, Action action)
        {
            PushColor(color);
            action();
            PopColor();
        }
        public static T UsingColor<T>(Color? color, Func<T> func)
        {
            PushColor(color);
            T result = func();
            PopColor();
            return result;
        }

        static Stack<TextAnchor> textAnchors = new Stack<TextAnchor>();
        public static void PushTextAnchor(TextAnchor anchor)
        {
            textAnchors.Push(Text.Anchor);
            Text.Anchor = anchor;
        }
        public static void PopTextAnchor()
        {
            Text.Anchor = textAnchors.Pop();
        }
        public static void UsingTextAnchor(TextAnchor anchor, Action action)
        {
            PushTextAnchor(anchor);
            action();
            PopTextAnchor();
        }
        public static T UsingTextAnchor<T>(TextAnchor anchor, Func<T> func)
        {
            PushTextAnchor(anchor);
            T result = func();
            PopTextAnchor();
            return result;
        }

        static Stack<bool> wordWraps = new Stack<bool>();
        internal static void PushWordWrap(bool wordWrap)
        {
            wordWraps.Push(Text.WordWrap);
            Text.WordWrap = wordWrap;
        }
        internal static void PopWordWrap()
        {
            Text.WordWrap = wordWraps.Pop();
        }
        public static void UsingWordWrap(bool wordWrap, Action action)
        {
            PushWordWrap(wordWrap);
            action();
            PopWordWrap();
        }
        public static T UsingWordWrap<T>(bool wordWrap, Func<T> func)
        {
            PushWordWrap(wordWrap);
            T result = func();
            PopWordWrap();
            return result;
        }

    }
}

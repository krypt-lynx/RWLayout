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
        public TextAnchor TextAlignment = TextAnchor.UpperLeft;

        static GUIContent contentForTesting = new GUIContent();
        public override Vector2 tryFit(Vector2 size)
        {
            contentForTesting.text = Title;
            return GuiTools.UsingFont(Font, () => Text.CurFontStyle.CalcSize(contentForTesting));
        }

        public override void DoContent()
        {
            base.DoContent();
            GuiTools.FontPush(Font);
            GuiTools.ColorPush(Color);
            GuiTools.TextAnchorPush(TextAlignment);

            Widgets.Label(bounds, Title);

            GuiTools.TextAnchorPop();
            GuiTools.ColorPop();
            GuiTools.FontPop();
        }
    }
}

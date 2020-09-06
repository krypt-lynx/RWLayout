using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public abstract class CTitledElement : CElement
    {
        public string Title;
        public GameFont Font = GameFont.Small;
        public Color? Color = null;

        public bool WordWrap = false;
        public bool Multiline = false;
        public int LineCount = 0;
        /// <summary>
        /// Text alignment
        /// </summary>
        public TextAnchor TextAlignment = TextAnchor.UpperLeft;

        protected void ApplyGeometryOnly()
        {
            GuiTools.PushFont(Font);
            GuiTools.PushWordWrap(WordWrap);
        }

        protected void RestoreGeometryOnly()
        {
            GuiTools.PopWordWrap();
            GuiTools.PopFont();
        }

        protected void ApplyAll()
        {
            GuiTools.PushFont(Font);
            GuiTools.PushColor(Color);
            GuiTools.PushTextAnchor(TextAlignment);
            GuiTools.PushWordWrap(WordWrap);
        }

        protected void RestoreAll()
        {
            GuiTools.PopWordWrap();
            GuiTools.PopTextAnchor();
            GuiTools.PopColor();
            GuiTools.PopFont();
        }

        static GUIContent contentForTesting = new GUIContent();
        public Vector2 tryFitText(Vector2 size, Vector2 margin)
        {
            ApplyGeometryOnly();
            contentForTesting.text = Title;
            var result = Text.CurFontStyle.CalcSize(contentForTesting);
            RestoreGeometryOnly();
            return result + margin;
        }
    }

}

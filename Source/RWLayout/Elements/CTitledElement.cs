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
        string title = null;
        public string Title
        {
            get => title; 
            set {
                if (title != value)
                {
                    title = value;
                    SetNeedsUpdateLayout();
                }
            }
        }


        GameFont font = GameFont.Small;
        public GameFont Font 
        {
            get => font; 
            set {
                if (font != value)
                {
                    font = value;
                    SetNeedsUpdateLayout();
                }
            }
        }
        public Color? Color = null;

        public bool wordWrap = false;
        public bool WordWrap
        {
            get => wordWrap;
            set
            {
                if (wordWrap != value)
                {
                    wordWrap = value;
                    SetNeedsUpdateLayout();
                }
            }
        }
        //public bool Multiline = false;
        //public int LineCount = 0;
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
            contentForTesting.text = Title ?? "";

            float minX = 0;
            float maxX = 0;
            
            Text.CurFontStyle.CalcMinMaxWidth(contentForTesting, out minX, out maxX);

            float x = Mathf.Max(minX, Mathf.Min(maxX, size.x));
            float y = Text.CurFontStyle.CalcHeight(contentForTesting, x);

            RestoreGeometryOnly();
            return new Vector2(x, y) + margin;
        }
    }

}

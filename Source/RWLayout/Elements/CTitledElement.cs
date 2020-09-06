using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWLayout.Alpha1.MiscTools;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Base class for widgets wrappers contaning cofigurable text in it
    /// </summary>
    public abstract class CTitledElement : CElement
    {
        string title = null;
        /// <summary>
        /// the title of view
        /// </summary>
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
        /// <summary>
        /// font for title
        /// </summary>
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
        /// <summary>
        /// color for title
        /// </summary>
        public Color? Color = null;

        bool wordWrap = false;
        /// <summary>
        /// use word wrap flag
        /// </summary>
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
        /// <summary>
        /// measures text size for tryFit method.
        /// </summary>
        /// <param name="size">desired size</param>
        /// <param name="margin">expected inner margin</param>
        /// <returns>Best size for text to fir in desized size is possible to fit. Required size to fit if is not</returns>
        public Vector2 tryFitText(Vector2 size, Vector2 margin)
        {
            ApplyGeometryOnly();
            contentForTesting.text = Title ?? "";

            //float minX = 0;
            //float maxX = 0;
            
            //Text.CurFontStyle.CalcMinMaxWidth(contentForTesting, out minX, out maxX);

            //float x = Mathf.Max(minX, Mathf.Min(maxX, size.x));
            //float y = Text.CurFontStyle.CalcHeight(contentForTesting, x);
            var result = Text.CurFontStyle.CalcSize(contentForTesting, size - margin);
            RestoreGeometryOnly();
            return result + margin;
        }
    }

}

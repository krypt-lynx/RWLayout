// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using UnityEngine;
using Verse;

namespace RWLayout.Alpha1
{
    public enum CScrollBarMode
    {
        Auto,
        Show,
        Hide
    }

    public class CListingStandart : CElement
    {
        Listing_Standard listing = new Listing_Standard();
        Rect innerRect;
        Vector2 scrollPosition = Vector2.zero;

        List<CListingRow> rows = new List<CListingRow>();

        public CScrollBarMode ShowScrollBar = CScrollBarMode.Auto;

        float contentHeight = 0;

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(bounds.width, contentHeight);
        }

        public bool IsScrollBarVisible()
        {
            return (ShowScrollBar == CScrollBarMode.Show) ||
                (contentHeight > bounds.height);
        }

        public override void PostConstraintsUpdate()
        {
            base.PostConstraintsUpdate();

            float y = 0;
            float w = bounds.width - (IsScrollBarVisible() ? 20 : 0);
            foreach (var row in rows)
            {
                row.Solver.AddConstraints(ClStrength.Weak, row.height ^ 20);
                row.InRect = new Rect(0, y, w, float.NaN);
                y += row.bounds.height;
            }
            contentHeight = y;
            innerRect = new Rect(0, 0, w, contentHeight);
        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            float y = 0;
            foreach (var row in rows)
            {
                row.InRect = new Rect(0, y, bounds.width - (IsScrollBarVisible() ? 20 : 0), float.NaN);
                y += row.bounds.height;
            }
            contentHeight = y;
        }

        public override void DoContent()
        {
            base.DoContent();
            bool showScrollBar = IsScrollBarVisible();

            if (showScrollBar)
            {
                listing.BeginScrollView(bounds, ref scrollPosition, ref innerRect);
            }
            else
            {
                listing.Begin(bounds);
            }

            foreach (var element in rows)
            {
                var rect = listing.GetRect(element.bounds.height);
                element.DoElementContent();
            }

            if (showScrollBar)
            {
                listing.EndScrollView(ref innerRect);
            }
            else
            {
                listing.End();
            }
        }

        /*
            public class Listing_Standard : Listing
            {
                public Listing_Standard();
                public Listing_Standard(GameFont font);

                public override void Begin(Rect rect);
                public void BeginScrollView(Rect rect, ref Vector2 scrollPosition, ref Rect viewRect);
                [Obsolete]
                public Listing_Standard BeginSection(float height);
                public Listing_Standard BeginSection_NewTemp(float height, float sectionBorder = 4, float bottomBorder = 4);
                [Obsolete("Only used for mod compatibility")]
                public bool ButtonDebug(string label);
                public bool ButtonDebug_NewTmp(string label, bool highlight);
                public bool ButtonImage(Texture2D tex, float width, float height);
                public bool ButtonText(string label, string highlightTag = null);
                public bool ButtonTextLabeled(string label, string buttonLabel);
                public void CheckboxLabeled(string label, ref bool checkOn, string tooltip = null);
                public bool CheckboxLabeledSelectable(string label, ref bool selected, ref bool checkOn);
                public override void End();
                public void EndScrollView(ref Rect viewRect);
                public void EndSection(Listing_Standard listing);
                public void IntAdjuster(ref int val, int countChange, int min = 0);
                public void IntEntry(ref int val, ref string editBuffer, int multiplier = 1);
                public void IntRange(ref IntRange range, int min, int max);
                public void IntSetter(ref int val, int target, string label, float width = 42);
                public Rect Label(TaggedString label, float maxHeight = -1, string tooltip = null);
                public Rect Label(string label, float maxHeight = -1, string tooltip = null);
                [Obsolete("Only used for mod compatibility")]
                public void LabelCheckboxDebug(string label, ref bool checkOn);
                public void LabelCheckboxDebug_NewTmp(string label, ref bool checkOn, bool highlight);
                public void LabelDouble(string leftLabel, string rightLabel, string tip = null);
                public void None();
                [Obsolete]
                public bool RadioButton(string label, bool active, float tabIn = 0, string tooltip = null);
                public bool RadioButton_NewTemp(string label, bool active, float tabIn = 0, string tooltip = null, float? tooltipDelay = null);
                public bool SelectableDef(string name, bool selected, Action deleteCallback);
                public float Slider(float val, float min, float max);
                public string TextEntry(string text, int lineCount = 1);
                public string TextEntryLabeled(string label, string text, int lineCount = 1);
                public void TextFieldNumeric<T>(ref T val, ref string buffer, float min = 0, float max = 1E+09F) where T : struct;
                public void TextFieldNumericLabeled<T>(string label, ref T val, ref string buffer, float min = 0, float max = 1E+09F) where T : struct;
            }
         */


        public CElement NewRow()
        {
            var row = new CListingRow();
            rows.Add(row);

            return row;
        }
    }
}

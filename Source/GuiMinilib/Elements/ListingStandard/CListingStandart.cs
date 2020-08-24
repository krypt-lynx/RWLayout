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

namespace RWLayout
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

        /*
        public override void UpdateLayout()
        {
            base.UpdateLayout();
            foreach (var row in rows)
            {
                row.UpdateLayout();
            }
        }*/

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

        public CElement NewRow()
        {
            var row = new CListingRow();
            rows.Add(row);

            return row;
        }
    }
}

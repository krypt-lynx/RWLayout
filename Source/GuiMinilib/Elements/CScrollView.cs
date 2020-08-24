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
    public class CScrollView : CElement
    {
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
            foreach (var row in rows)
            {
                row.Solver.AddConstraints(ClStrength.Weak, row.height ^ 20);
                row.InRect = new Rect(0, y, bounds.width - (IsScrollBarVisible() ? 20 : 0), float.NaN);
                y += row.bounds.height;
            }
            contentHeight = y;
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
            float w = bounds.width - (IsScrollBarVisible() ? 20 : 0);
            foreach (var row in rows)
            {
                row.InRect = new Rect(0, y, w, float.NaN);
                y += row.bounds.height;
            }
            contentHeight = y;
            innerRect = new Rect(0, 0, w, contentHeight);
        }

        public override void DoContent()
        {
            base.DoContent();
            bool showScrollBar = IsScrollBarVisible();

            showScrollBar = true;
            Widgets.BeginScrollView(bounds, ref scrollPosition, innerRect, showScrollBar);

            foreach (var element in rows)
            {
                if ((element.bounds.yMax > scrollPosition.y) && (element.bounds.yMin < (scrollPosition.y + this.bounds.height)))
                {
                    element.DoElementContent();
                }
            }

            Widgets.EndScrollView();
        }

        public CElement NewRow()
        {
            var row = new CListingRow();
            rows.Add(row);

            return row;
        }
    }
}

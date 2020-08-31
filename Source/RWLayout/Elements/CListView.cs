using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public class CListView : CElement
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


        public virtual Vector2 toViewCoordinates(Vector2 point)
        {
            return point; // todo: implement
        }

        public override CElement hitTest(Vector2 point)
        {
            if (userInteractionEnabled && bounds.Contains(point))
            {

                var listPoint = point - this.bounds.position;

                foreach (var row in rows)
                {
                    var element = row.hitTest(listPoint);
                    if (element != null)
                    {
                        return element;
                    }
                }

                return base.hitTest(point);
            } 
            else
            {
                return null;
            }
        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            var skin = GUI.skin.verticalScrollbar;

            float y = 0;
            float w = bounds.width - (IsScrollBarVisible() ? (skin.fixedWidth + skin.margin.left) : 0);
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

        public CListingRow AppendRow(CListingRow row)
        {
            if (row.Owner != null)
            {
                throw new InvalidOperationException($"{row} is already added to {row.Owner}");
            }

            row.Owner = this;
            rows.Add(row);
            // set needs layout
            return row;
        }

        public CListingRow InsertRow(int index, CListingRow row)
        {
            if (row.Owner != null)
            {
                throw new InvalidOperationException($"{row} is already added to {row.Owner}");
            }

            row.Owner = this;
            rows.Insert(index, row);
            // set needs layout
            return row;
        }

        public bool RemoveRow(CListingRow row)
        {
            bool result;
            if (result = rows.Remove(row))
            {
                row.Owner = null;
            }
            // set needs layout
            return result;
        }

        public CListingRow RemoveRowAt(int index)
        {
            var row = rows[index];
            rows.RemoveAt(index);
            row.Owner = null;
            // set needs layout
            return row;
        }

        public void MoveRowTo(CListingRow row, int index)
        {
            rows.Remove(row);
            rows.Insert(index, row);
            // set needs layout
        }

        public int IndexOfRow(CListingRow row)
        {
            return rows.IndexOf(row);
        }
    }
}

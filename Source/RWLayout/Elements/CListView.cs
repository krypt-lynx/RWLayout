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
    /// <summary>
    /// Constraints controlled list view implementation
    /// </summary>
    /// <remarks>
    /// Contrants cannot be set between rows and external layout. This tradeoff was made to avoid performance hit. 
    /// If you really need such constraints - you can use CScrolView with .StackTop method. Maybe I will do something with it later.
    /// </remarks>
    public class CListView : CElement
    {
        Rect innerRect;
        /// <summary>
        /// Scroll Location
        /// </summary>
        public Vector2 ScrollPosition = Vector2.zero;

        List<CListingRow> rows = new List<CListingRow>();

        /// <summary>
        /// Than to show scroll bars.
        /// </summary>
        public CScrollBarMode ShowScrollBar = CScrollBarMode.Auto;

        float contentHeight = 0;

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(Bounds.width, contentHeight);
        }

        /// <summary>
        /// Returnns true if verstical scrollbar is visible
        /// </summary>
        /// <returns></returns>
        public bool IsScrollBarVisible()
        {
            return (ShowScrollBar == CScrollBarMode.Show) ||
                (contentHeight > Bounds.height);
        }

        public override CElement hitTest(Vector2 point)
        {
            if (userInteractionEnabled && Bounds.Contains(point))
            {
                var listPoint = point - this.Bounds.position + this.ScrollPosition;

                var sub = base.hitTest(point);
                if (sub != this)
                {
                    return sub;
                }

                foreach (var row in rows)
                {
                    var element = row.hitTest(listPoint);
                    if (element != null)
                    {
                        return element;
                    }
                }

                return this;
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
            float w = Bounds.width - (IsScrollBarVisible() ? (skin.fixedWidth + skin.margin.left) : 0);
            foreach (var row in rows)
            {
                row.InRect = new Rect(0, y, w, float.NaN);
                row.UpdateLayoutIfNeeded();
                y += row.Bounds.height;
            }
            contentHeight = y;
            innerRect = new Rect(0, 0, w, contentHeight);
        }

        public override void DoContent()
        {
            base.DoContent();
            bool showScrollBar = IsScrollBarVisible();

            showScrollBar = true;
            Widgets.BeginScrollView(Bounds, ref ScrollPosition, innerRect, showScrollBar);

            foreach (var element in rows)
            {
                if ((element.Bounds.yMax > ScrollPosition.y) && (element.Bounds.yMin < (ScrollPosition.y + this.Bounds.height)))
                {
                    element.DoElementContent();
                }
            }

            Widgets.EndScrollView();
        }

        /// <summary>
        /// Append a row to CListView.
        /// </summary>
        /// <param name="row">the row</param>
        /// <returns>the row</returns>
        /// <remarks>Row must not be added in this or other CListView</remarks>
        public CListingRow AppendRow(CListingRow row)
        {
            if (row.Owner != null)
            {
                throw new InvalidOperationException($"{row} is already added to {row.Owner}");
            }

            row.Owner = this;
            rows.Add(row);
            SetNeedsUpdateLayout();
            return row;
        }

        /// <summary>
        /// Insert a row to CListView at index.
        /// </summary>
        /// <param name="index">index to insert the row at</param>
        /// <param name="row">the row</param>
        /// <returns>the row</returns>
        /// <remarks>Row must not be added in this or other CListView</remarks>
        public CListingRow InsertRow(int index, CListingRow row)
        {
            if (row.Owner != null)
            {
                throw new InvalidOperationException($"{row} is already added to {row.Owner}");
            }

            row.Owner = this;
            rows.Insert(index, row);
            SetNeedsUpdateLayout();
            return row;
        }

        /// <summary>
        /// Removes the row from CListView
        /// </summary>
        /// <param name="row">the row</param>
        /// <returns>true if row was successfully removed</returns>
        public bool RemoveRow(CListingRow row)
        {
            bool result;
            if (result = rows.Remove(row))
            {
                row.Owner = null;
            }
            SetNeedsUpdateLayout();
            return result;
        }

        /// <summary>
        /// Removes the row at index from CListView
        /// </summary>
        /// <param name="row">index of row to remove</param>
        /// <returns>the removed row</returns>
        public CListingRow RemoveRowAt(int index)
        {
            var row = rows[index];
            rows.RemoveAt(index);
            row.Owner = null;
            SetNeedsUpdateLayout();
            return row;
        }

        /// <summary>
        /// Moves row to index
        /// </summary>
        /// <param name="row">the row</param>
        /// <param name="index">new index</param>
        public void MoveRowTo(CListingRow row, int index)
        {
            rows.Remove(row);
            rows.Insert(index, row);
            SetNeedsUpdateLayout();
        }

        /// <summary>
        /// Returns index of the row
        /// </summary>
        /// <param name="row">the row</param>
        /// <returns>The zero-based index of the row, if found; otherwise, -1.</returns>
        public int IndexOfRow(CListingRow row)
        {
            return rows.IndexOf(row);
        }
    }
}

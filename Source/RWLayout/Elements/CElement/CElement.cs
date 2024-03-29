﻿using Cassowary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    /// <summary>
    /// View. Represents region defined by constraints. Provides geometry, update and drawing methods
    /// </summary>
    public partial class CElement
    {
        /// <summary>
        /// View ID. Used obly for debug info
        /// </summary>
        public int ID { get; }
        public CElement()
        {
            ID = nextId++; // todo: thread safety 
        }
     
        /// <summary>
        /// Field for custom data storage
        /// </summary>
        public object Tag = null;

        internal void UpdateLayoutInternal()
        {
            this.Root.UpdateLayout();
            this.Root.PostLayoutUpdate();
        }

        /// <summary>
        /// Updates constraints and anchor values
        /// </summary>
        /// <remarks>should not be called directly. Use UpdateLayoutIfNeeded</remarks>
        public virtual void UpdateLayout()
        {
            AddImpliedConstraints();

            if (intrinsicWidth_.cn != null || intrinsicHeight_.cn != null)
            {
                intrinsicSize = this.tryFit(Bounds.size);

                if (intrinsicWidth_.cn != null)
                {
                    UpdateStayConstrait(ref intrinsicWidth_, intrinsicSize.Value.x);
                }
                if (intrinsicHeight_.cn != null)
                {
                    UpdateStayConstrait(ref intrinsicHeight_, intrinsicSize.Value.y);
                }
            }

            foreach (var element in Elements)
            {
                element.UpdateLayout();
            }
        }

        public bool Clipping { get; set; } = false;

        [Obsolete("Use Clipping instead")]
        public bool Groupping => Clipping;

        Vector2? intrinsicSize = null;

        /// <summary>
        /// Called after constraints resolve. You can override it to perform inner geometry update.
        /// </summary>
        public virtual void PostLayoutUpdate()
        {
            var parent = Parent;
            if (parent != null)
            {
                globalOffset = Parent.GlobalOffset + Parent.LocalOffset;
            }
            else
            {
                globalOffset = Vector2.zero;
            }

            if (Clipping)
            {
                localOffset = new Vector2(-(float)left.Value - globalOffset.x, -(float)top.Value - globalOffset.y);
            }
            else
            {
                localOffset = Vector2.zero;
            }

            RecacheMetrics();


            foreach (var element in Elements)
            {
                element.PostLayoutUpdate();
            }

            var newIntrinsicSize = this.tryFit(Bounds.size);

            if (intrinsicSize.HasValue && newIntrinsicSize != intrinsicSize.Value)
            {
                //Log.Message($"{NamePrefix()}: intrinsicSize: {intrinsicSize}; newIntrinsicSize: {newIntrinsicSize}");
                SetNeedsUpdateLayout();
            }
        }

        protected void RecacheMetrics()
        {
            Frame = Rect.MinMaxRect(
                (float)left.Value + globalOffset.x,
                (float)top.Value + globalOffset.y,
                (float)right.Value + globalOffset.x,
                (float)bottom.Value + globalOffset.y);

            Bounds = Rect.MinMaxRect(
                (float)left.Value + globalOffset.x + localOffset.x,
                (float)top.Value + globalOffset.y + localOffset.y,
                (float)right.Value + globalOffset.x + localOffset.x,
                (float)bottom.Value + globalOffset.y + localOffset.y);

            BoundsRounded = Bounds.GUIRounded();
        }

        /// <summary>
        /// Is view hidden flag
        /// </summary>
        /// <remarks>is true hides current element and all its children</remarks>
        public virtual bool Hidden { set; get; } = false;


        /// <summary>
        /// Returns true if layout was changed and update is requited
        /// </summary>
        /// <returns></returns>
        public virtual bool NeedsUpdateLayout()
        {
            return Parent?.NeedsUpdateLayout() ?? false;
        }


        /// <summary>
        /// Requests layout update
        /// </summary>
        public virtual void SetNeedsUpdateLayout()
        {
            if (Parent != null)
            {
                Parent.SetNeedsUpdateLayout();
            }
        }

        /// <summary>
        /// Updates layout, but only if it really required
        /// </summary>
        public virtual void UpdateLayoutIfNeeded()
        {
            var root = Root;
            if (root.NeedsUpdateLayout())
            {
                root.UpdateLayoutInternal();
            }
            // Elements can ask for second update to resolve stays (scroll view, label)
            if (root.NeedsUpdateLayout())
            {
                root.UpdateLayoutInternal();
            }

        }

        // todo: intristic size

        /// <summary>
        /// String desctiption
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{base.ToString()}-{NamePrefix()}";
        }
    }
}

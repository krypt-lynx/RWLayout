// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Cassowary;
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
            ID = nextId++;
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

        Vector2? intrinsicSize = null;

        /// <summary>
        /// Called after constraints resolve. You can override it to perform inner geometry update.
        /// </summary>
        public virtual void PostLayoutUpdate()
        {
            Bounds = Rect.MinMaxRect((float)left.Value, (float)top.Value, (float)right.Value, (float)bottom.Value);
            BoundsRounded = Bounds.GUIRounded();

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

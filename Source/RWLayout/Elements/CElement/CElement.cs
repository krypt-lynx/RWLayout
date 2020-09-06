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
    public partial class CElement
    {
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


        public virtual void UpdateLayout()
        {
            AddImpliedConstraints();

            if (intrinsicWidth_.cn != null || intrinsicHeight_.cn != null)
            {
                var intrinsicSize = this.tryFit(Bounds.size);

                if (intrinsicWidth_.cn != null)
                {
                    UpdateStayConstrait(ref intrinsicWidth_, intrinsicSize.x);
                }
                if (intrinsicHeight_.cn != null)
                {
                    UpdateStayConstrait(ref intrinsicHeight_, intrinsicSize.y);
                }
            }

            foreach (var element in Elements)
            {
                element.UpdateLayout();
            }
        }

        public virtual void PostLayoutUpdate()
        {
            Bounds = Rect.MinMaxRect((float)left.Value, (float)top.Value, (float)right.Value, (float)bottom.Value);
            BoundsRounded = Bounds.Rounded2();

            foreach (var element in Elements)
            {
                element.PostLayoutUpdate();
            }
        }

        public virtual bool Hidden { set; get; } = false;


        //protected bool needsUpdateLayout = true;
        public virtual bool NeedsUpdateLayout()
        {
            return Parent?.NeedsUpdateLayout() ?? false;
        }

        public virtual void SetNeedsUpdateLayout()
        {
            if (Parent != null)
            {
                Parent.SetNeedsUpdateLayout();
            }
        }

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

        public override string ToString()
        {
            return $"{base.ToString()}-{NamePrefix()}";
        }
    }
}

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
    public interface IElement
    {
        T AddElement<T>(T element) where T : CElement;
        void RemoveElement(CElement element);
        CElement Parent { get; }
    }

    public partial class CElement
    {
        public int id { get; }
        public CElement()
        {
            id = nextId++;
        }

        /*public virtual void UpdateLayoutConstraints()
        {
            AddImpliedConstraints();
            foreach (var element in Elements)
            {
                element.UpdateLayoutConstraints();
            }
        }*/

        /* public virtual void PostConstraintsUpdate()
         {
             foreach (var element in Elements)
             {
                 element.PostConstraintsUpdate();
             }
         }
        */

        public void UpdateLayoutTemp()
        {
            this.Root.UpdateLayout();
            this.Root.PostLayoutUpdate();
        }


        public virtual void UpdateLayout()
        {
            AddImpliedConstraints();

            if (intrinsicWidth_.cn != null || intrinsicHeight_.cn != null)
            {
                var intrinsicSize = this.tryFit(bounds.size);

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
            bounds = Rect.MinMaxRect((float)left.Value, (float)top.Value, (float)right.Value, (float)bottom.Value);
            boundsRounded = bounds.Rounded2();

            foreach (var element in Elements)
            {
                element.PostLayoutUpdate();
            }
        }

        public bool Hidden = false;


        //protected bool needsUpdateLayout = true;
        public virtual bool NeedsUpdateLayout
        {
            get
            {
                return Parent?.NeedsUpdateLayout ?? false;
            }
            set
            {
                if (Parent != null)
                {
                    Parent.NeedsUpdateLayout = value;
                }
            }
        }


        public virtual void UpdateLayoutIfNeeded()
        {
            if (NeedsUpdateLayout)
            {
                UpdateLayoutTemp();
            }
        }

        // todo: intristic size
    }
}

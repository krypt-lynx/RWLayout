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

namespace GuiMinilib
{
    public partial class CElement
    {
        public static int nextId = 0;
        public string NamePrefix()
        {
            return $"{GetType().Name}_{id}";
        }

        public int id { get; }
        public CElement()
        {
            id = nextId++;

            CreateAnchors();
        }

        public virtual ClSimplexSolver solver { get { return parent?.solver; } }

        public virtual void UpdateLayoutConstraints(ClSimplexSolver solver)
        {
            AddImpliedConstraints(solver);

            foreach (var element in elements)
            {
                element.UpdateLayoutConstraints(solver);
            }
        }
        public virtual void PostConstraintsUpdate()
        {
            foreach (var element in elements)
            {
                element.PostConstraintsUpdate();
            }
        }

        public virtual void UpdateLayout()
        {

            if (intrinsicWidth_ != null || intrinsicHeight_ != null)
            {
                var intrinsicSize = this.tryFit(bounds.size);

                if (intrinsicWidth_ != null)
                {
                    solver.UpdateStayConstrait(ref intrinsicWidthConstraint_, intrinsicSize.x);
                }
                if (intrinsicHeight_ != null)
                {
                    solver.UpdateStayConstrait(ref intrinsicHeightConstraint_, intrinsicSize.y);
                }
            }
            foreach (var element in elements)
            {
                element.UpdateLayout();
            }
        }
        public virtual void PostLayoutUpdate()
        {
            bounds = Rect.MinMaxRect((float)left.Value, (float)top.Value, (float)right.Value, (float)bottom.Value);

            foreach (var element in elements)
            {
                element.PostLayoutUpdate();
            }
        }



        // todo: intristic size
    }
}

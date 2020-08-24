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

namespace RWLayout
{
    public interface IElement
    {
        ClSimplexSolver Solver { get; }
        T AddElement<T>(T element) where T : CElement;
        void RemoveElement(CElement element);
        CElement Parent { get; }
    }

    public partial class CElement : IElement
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

        public virtual ClSimplexSolver Solver { get { return Parent?.Solver; } }

        public virtual void UpdateLayoutConstraints(ClSimplexSolver solver)
        {
            AddImpliedConstraints(solver);

            foreach (var element in Elements)
            {
                element.UpdateLayoutConstraints(solver);
            }
        }
        public virtual void PostConstraintsUpdate()
        {
            foreach (var element in Elements)
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
                    Solver.UpdateStayConstrait(ref intrinsicWidthConstraint_, intrinsicSize.x);
                }
                if (intrinsicHeight_ != null)
                {
                    Solver.UpdateStayConstrait(ref intrinsicHeightConstraint_, intrinsicSize.y);
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

        // todo: intristic size
    }
}

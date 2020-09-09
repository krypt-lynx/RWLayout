using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Layout guide providing size anchors
    /// </summary>
    public class CSizeGuide : CLayoutGuide
    {
        public CSizeGuide(string nameInfix)
        {
            widthSuffix = nameInfix + "W";
            heightSuffix = nameInfix + "H";
        }

        private string widthSuffix;
        private string heightSuffix;

        private Anchor width_ = new Anchor();
        private Anchor height_ = new Anchor();

        public ClVariable width => Parent.GetVariable(ref width_, widthSuffix);
        public ClVariable height => Parent.GetVariable(ref height_, heightSuffix);

        public void UpdateSize(Vector2 size)
        {
            Parent.UpdateStayConstrait(ref width_, size.x);
            Parent.UpdateStayConstrait(ref height_, size.y);
        }

        public override void AddImpliedConstraints()
        {
            Parent.CreateConstraintIfNeeded(ref width_, () => new ClStayConstraint(width));
            Parent.CreateConstraintIfNeeded(ref height_, () => new ClStayConstraint(height));
        }

        public override void RemoveImpliedConstraints()
        {
            Parent.RemoveVariableIfNeeded(ref width_);
            Parent.RemoveVariableIfNeeded(ref height_);
        }

        public override IEnumerable<ClVariable> enumerateAnchors()
        {
            yield return width_.var;
            yield return height_.var;
        }
    }

    /// <summary>
    /// Gui root of CWindow
    /// </summary>
    public class CWindowRoot : CElementHost
    {
        /// <summary>
        /// Guide for desired window size
        /// </summary>
        public CSizeGuide WindowSize { get; } = new CSizeGuide("w");

        /// <summary>
        /// Guide for adjusted screen size (screen size - window margins size)
        /// </summary>
        public CSizeGuide AdjustedScreenSize { get; } = new CSizeGuide("s");

        public CWindowRoot()
        {
            AddGuide(WindowSize);
            AddGuide(AdjustedScreenSize);
        }

        internal Action LayoutUpdated;

        public override void AddImpliedConstraints()
        {
            base.AddImpliedConstraints();

            CreateConstraintIfNeeded(ref left_, () => new ClStayConstraint(left, ClStrength.Required));
            CreateConstraintIfNeeded(ref top_, () => new ClStayConstraint(top, ClStrength.Required));
        }

        public override void RemoveImpliedConstraints()
        {
            base.RemoveImpliedConstraints();

            RemoveVariableIfNeeded(ref left_);
            RemoveVariableIfNeeded(ref top_);
        }


        public override void UpdateLayout()
        {
            UpdateStayConstrait(ref left_, InRect.xMin);
            UpdateStayConstrait(ref top_, InRect.yMin);

            base.UpdateLayout();
        }

        internal void UpdateWindowGuide(Vector2 size)
        {
            WindowSize.UpdateSize(size);
        }

        internal void UpdateScreenGuide(Vector2 size)
        {
            AdjustedScreenSize.UpdateSize(size);
        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            LayoutUpdated?.Invoke();
        }

        int layoutUpdateDebugCounter = 0;
        internal void UpdateAndDoContent(Rect inRect, bool isResizing)
        {
            if (NeedsUpdateLayout() && !isResizing)
            {
                layoutUpdateDebugCounter++;
            } 
            else
            {
                layoutUpdateDebugCounter = 0;
            }

            if (layoutUpdateDebugCounter == 60)
            {
                Log.Warning($"{NamePrefix()} layout being updated every frame for 60 frames. Probably something is going wrong.");
            }

            UpdateWindowGuide(inRect.size);
            InRect = inRect;
            UpdateLayoutIfNeeded();
            DoElementContent();
        }
    }

}

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
    public class CSizeLayoutGuide : CLayoutGuide
    {
        public CSizeLayoutGuide(string nameInfix)
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

    public class CWindowRoot : CElementHost
    {
        public CSizeLayoutGuide WindowSize { get; } = new CSizeLayoutGuide("w");
        public CSizeLayoutGuide AdjustedScreenSize { get; } = new CSizeLayoutGuide("s");

        public CWindowRoot()
        {
            AddGuide(WindowSize);
            AddGuide(AdjustedScreenSize);
        }

        public Action LayoutUpdated;

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

        public void UpdateWindowGuide(Vector2 size)
        {
            WindowSize.UpdateSize(size);
        }

        public void UpdateScreenGuide(Vector2 size)
        {
            AdjustedScreenSize.UpdateSize(size);
        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            LayoutUpdated?.Invoke();
        }

        public void UpdateAndDoContent(Rect inRect)
        {
            UpdateWindowGuide(inRect.size);
            InRect = inRect;
            UpdateLayoutIfNeeded();
            DoElementContent();
        }
    }

}

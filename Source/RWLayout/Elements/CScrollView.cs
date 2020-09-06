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
    internal class CScrollContent : COwnedElement
    {
        public override ClSimplexSolver Solver
        {
            get
            {
                return Owner.Solver;
            }
        }

        public IEnumerable<ClVariable> exposeAnchors()
        {
            return allAnchors();
        }
    }

    public class CScrollViewGuide : CLayoutGuide
    {
        public override void AddImpliedConstraints()
        {
            Parent.CreateConstraintIfNeeded(ref width_, () => Parent.width - vBarWidth ^ width);
            Parent.CreateConstraintIfNeeded(ref height_, () => Parent.height - hBarHeight ^ height);

            Parent.CreateConstraintIfNeeded(ref vBarWidth_, () => new ClStayConstraint(vBarWidth_.var, ClStrength.Required));
            Parent.CreateConstraintIfNeeded(ref hBarHeight_, () => new ClStayConstraint(hBarHeight_.var, ClStrength.Required));
        }

        public override void RemoveImpliedConstraints()
        {
            Parent.RemoveVariableIfNeeded(ref width_);
            Parent.RemoveVariableIfNeeded(ref height_);

            Parent.RemoveVariableIfNeeded(ref vBarWidth_);
            Parent.RemoveVariableIfNeeded(ref hBarHeight_);
        }

        private Anchor width_ = new Anchor();
        private Anchor height_ = new Anchor();
        private Anchor vBarWidth_ = new Anchor();
        private Anchor hBarHeight_ = new Anchor();


        public ClVariable width => Parent?.GetVariable(ref width_, "cW");
        public ClVariable height => Parent?.GetVariable(ref height_, "cH");
        public ClVariable vBarWidth => Parent?.GetVariable(ref vBarWidth_, "bW");
        public ClVariable hBarHeight => Parent?.GetVariable(ref hBarHeight_, "bH");

        public override IEnumerable<ClVariable> enumerateAnchors()
        {
            yield return width_.var;
            yield return height_.var;
            yield return vBarWidth_.var;
            yield return hBarHeight_.var;
        }

        internal void UpdateVars(float vBarWidth, float hBarHeight)
        {
            Parent.UpdateStayConstrait(ref vBarWidth_, vBarWidth);
            Parent.UpdateStayConstrait(ref hBarHeight_, hBarHeight);
        }
    }

    public class CScrollView : CElement
    {
        private CScrollContent content = null;
        public CElement Content { get => content; }

        public CScrollView()
        {
            content = new CScrollContent();
            content.Owner = this;
            content.AddConstraint(Content.left ^ 0, ClStrength.Required);
            content.AddConstraint(Content.top ^ 0, ClStrength.Required);

            InnerSizeGuide = new CScrollViewGuide();
            AddGuide(InnerSizeGuide);
        }

        protected override IEnumerable<ClVariable> allAnchors()
        {
            return base.allAnchors().Concat(content.exposeAnchors());
        }


        public Vector2 ScrollPosition;
        public readonly CScrollViewGuide InnerSizeGuide;

        public override void UpdateLayout()
        {
            UpdateInnerSizeGuide();
            base.UpdateLayout();
            content.UpdateLayout();
            if (needResolveInnerSize)
            {
                SetNeedsUpdateLayout();
                needResolveInnerSize = false;
            }
        }


        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();
            content.PostLayoutUpdate();

        }

        bool needResolveInnerSize = true;
        double oldVBarWidth = 0;
        
        private void UpdateInnerSizeGuide()
        {
            var skin = GUI.skin;
            var vBarDefault = skin.verticalScrollbar.fixedWidth + skin.verticalScrollbar.margin.left;
            var hBarDefault = skin.horizontalScrollbar.fixedHeight + skin.horizontalScrollbar.margin.top;

            var hBarHeight = (content.Bounds.width + oldVBarWidth > Bounds.width ? hBarDefault : 0);
            var vBarWidth = (content.Bounds.height + hBarHeight > Bounds.height ? vBarDefault : 0);
            oldVBarWidth = vBarWidth;

            // todo: use intrinsic size (currently intrinsic is not implemented)

            InnerSizeGuide.UpdateVars(vBarWidth, hBarHeight);


            //Log.Message($"{NamePrefix()} vBarWidth: {vBarWidth}, hBarHeight: {hBarHeight}");
            //Log.Message($"CScrollViewGuide: {InnerSizeGuide.width}, {InnerSizeGuide.height}");
            //Log.Message($"CScrollView: {this.width}, {this.height}");

            //Log.Message($"{AllConstraintsString()}");
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

                return Content.hitTest(listPoint);
            }
            else
            {
                return null;
            }
        }

        public override void DoContent()
        {
            base.DoContent();

            Widgets.BeginScrollView(Bounds, ref ScrollPosition, Content.Bounds);
            
            Content.DoElementContent();

            Widgets.EndScrollView();
        }

        // override 
    }
}

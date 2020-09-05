﻿using System;
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
    public class CWindowRoot : CElementHost
    {
        private Anchor guideWidth_ = new Anchor();
        private Anchor guideHeight_ = new Anchor();
        internal Anchor adjustedScreenWidth_ = new Anchor();
        internal Anchor adjustedScreenHeight_ = new Anchor();


        public ClVariable guideWidth => GetVariable(ref guideWidth_, "wW");
        public ClVariable guideHeight => GetVariable(ref guideHeight_, "wH");
        public ClVariable adjustedScreenWidth => GetVariable(ref adjustedScreenWidth_, "sW");
        public ClVariable adjustedScreenHeight => GetVariable(ref adjustedScreenHeight_, "sH");


        public Action LayoutUpdated;

        public override void AddImpliedConstraints()
        {
            base.AddImpliedConstraints();

            CreateConstraintIfNeeded(ref left_, () => new ClStayConstraint(left, ClStrength.Required));
            CreateConstraintIfNeeded(ref top_, () => new ClStayConstraint(top, ClStrength.Required));

            CreateConstraintIfNeeded(ref guideWidth_, () => new ClStayConstraint(guideWidth));
            CreateConstraintIfNeeded(ref guideHeight_, () => new ClStayConstraint(guideHeight));

            CreateConstraintIfNeeded(ref adjustedScreenWidth_, () => new ClStayConstraint(adjustedScreenWidth));
            CreateConstraintIfNeeded(ref adjustedScreenHeight_, () => new ClStayConstraint(adjustedScreenHeight));
        }

        public override void RemoveImpliedConstraints()
        {
            base.RemoveImpliedConstraints();

            RemoveVariableIfNeeded(ref left_);
            RemoveVariableIfNeeded(ref top_);

            RemoveVariableIfNeeded(ref guideWidth_);
            RemoveVariableIfNeeded(ref guideHeight_);

            RemoveVariableIfNeeded(ref adjustedScreenWidth_);
            RemoveVariableIfNeeded(ref adjustedScreenHeight_);
        }

        private IEnumerable<ClVariable> enumerateAnchors()
        {
            yield return guideWidth_.var;
            yield return guideHeight_.var;
            yield return adjustedScreenWidth_.var;
            yield return adjustedScreenHeight_.var;
        }

        protected override IEnumerable<ClVariable> allAnchors()
        {
            IEnumerable<ClVariable> allAnchors = enumerateAnchors().Where(x => x != null);

            return enumerateAnchors().Where(x => x != null).Concat(base.allAnchors());
        }

        public override void UpdateLayout()
        {
            //Debug.WriteLine(InRect.width);

            UpdateStayConstrait(ref left_, InRect.xMin);
            UpdateStayConstrait(ref top_, InRect.yMin);

            base.UpdateLayout();
        }

        Vector2 oldSize = Vector2.negativeInfinity;

        public void UpdateGuideSize(Vector2 size)
        {
            if (oldSize != size)
            {
                //Log.Message($"UpdateGuideSize: ({guideWidth_.var?.ToString() ?? "null"},{guideWidth_.cn?.ToString() ?? "null"})");

                //GetVariable(ref guideWidth_, "wW");
                //GetVariable(ref guideHeight_, "wH");
                UpdateStayConstrait(ref guideWidth_, size.x);
                UpdateStayConstrait(ref guideHeight_, size.y);
                oldSize = size;
            }


        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            LayoutUpdated?.Invoke();
        }

        public void UpdateAndDoContent(Rect inRect)
        {
            UpdateGuideSize(inRect.size);
            InRect = inRect;
            UpdateLayoutIfNeeded();
            DoElementContent();
        }
    }

}

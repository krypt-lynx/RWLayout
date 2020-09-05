using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using Verse;

namespace RWLayout.alpha2
{
    public class CGuiRoot : CElementHost
    {
        //public bool FlexableWidth = false;
        //public bool FlexableHeight = false;

        public Action LayoutUpdated;

        public override void AddImpliedConstraints()
        {
            base.AddImpliedConstraints();


            CreateConstraintIfNeeded(ref left_, () => new ClStayConstraint(left, ClStrength.Required));
            CreateConstraintIfNeeded(ref top_, () => new ClStayConstraint(top, ClStrength.Required));

            //if (!FlexableWidth)
            //{
                CreateConstraintIfNeeded(ref right_, () => new ClStayConstraint(right, ClStrength.Required));
            //}
            //if (!FlexableHeight)
            //{
                CreateConstraintIfNeeded(ref bottom_, () => new ClStayConstraint(bottom, ClStrength.Required));
            //}
        }

        public override void RemoveImpliedConstraints()
        {
            base.RemoveImpliedConstraints();

            RemoveVariableIfNeeded(ref left_);
            RemoveVariableIfNeeded(ref right_);
            RemoveVariableIfNeeded(ref top_);
            RemoveVariableIfNeeded(ref bottom_);
        }

        public override void UpdateLayout()
        {
            Debug.WriteLine(InRect.width);

            UpdateStayConstrait(ref left_, InRect.xMin);
            UpdateStayConstrait(ref right_, InRect.xMax);
            UpdateStayConstrait(ref top_, InRect.yMin);
            UpdateStayConstrait(ref bottom_, InRect.yMax);

            base.UpdateLayout();
        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();
            LayoutUpdated?.Invoke();
        }
    }
}

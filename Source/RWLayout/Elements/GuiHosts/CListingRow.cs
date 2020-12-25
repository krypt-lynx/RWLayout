using Cassowary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{

    public class CListingRow : CElementHost
    {

        public override void AddImpliedConstraints()
        {
            base.AddImpliedConstraints();
            

            CreateConstraintIfNeeded(ref left_, () => new ClStayConstraint(left, ClStrength.Required));
            CreateConstraintIfNeeded(ref right_, () => new ClStayConstraint(right, ClStrength.Required));
            CreateConstraintIfNeeded(ref top_, () => new ClStayConstraint(top, ClStrength.Required));
        }

        public override void UpdateLayout()
        {
            UpdateCreateIfNeededStayConstrait(ref left_, InRect.xMin, () => new ClStayConstraint(left, ClStrength.Required));
            UpdateCreateIfNeededStayConstrait(ref right_, InRect.xMax, () => new ClStayConstraint(right, ClStrength.Required));
            UpdateCreateIfNeededStayConstrait(ref top_, InRect.yMin, () => new ClStayConstraint(top, ClStrength.Required));

            base.UpdateLayout();
        }
    }
}

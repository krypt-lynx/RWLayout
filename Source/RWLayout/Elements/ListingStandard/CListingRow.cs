// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

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
            UpdateStayConstrait(ref left_, InRect.xMin);
            UpdateStayConstrait(ref right_, InRect.xMax);
            UpdateStayConstrait(ref top_, InRect.yMin);

            base.UpdateLayout();

            Solver.Solve();
        }
    }
}

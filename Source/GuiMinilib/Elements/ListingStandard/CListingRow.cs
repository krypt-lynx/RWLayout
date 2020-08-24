// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Cassowary_moddiff;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout_moddiff
{

    public class CListingRow : CElementHost
    {
        private ClStayConstraint leftStay = null;
        private ClStayConstraint rightStay = null;
        private ClStayConstraint topStay = null;


        public override void UpdateLayoutConstraints(ClSimplexSolver solver)
        {
            leftStay = solver.CreateStayConstrait(left, InRect.xMin, ClStrength.Required);
            rightStay = solver.CreateStayConstrait(right, InRect.xMax, ClStrength.Required);
            topStay = solver.CreateStayConstrait(top, InRect.yMin, ClStrength.Required);

            base.UpdateLayoutConstraints(solver);
        }

        public override void UpdateLayout()
        {
            Solver.UpdateStayConstrait(ref leftStay, InRect.xMin);
            Solver.UpdateStayConstrait(ref rightStay, InRect.xMax);
            Solver.UpdateStayConstrait(ref topStay, InRect.yMin);

            base.UpdateLayout();

            Solver.Solve();
        }
    }
}

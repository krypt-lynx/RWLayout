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

namespace RWLayout.Alpha1
{

    public class CListingRow : CElementHost
    {
        private ClStayConstraint leftStay = null;
        private ClStayConstraint rightStay = null;
        private ClStayConstraint topStay = null;


        public override void UpdateLayoutConstraints()
        {
            leftStay = Solver.CreateStayConstrait(left, InRect.xMin, ClStrength.Required);
            rightStay = Solver.CreateStayConstrait(right, InRect.xMax, ClStrength.Required);
            topStay = Solver.CreateStayConstrait(top, InRect.yMin, ClStrength.Required);

            base.UpdateLayoutConstraints();
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

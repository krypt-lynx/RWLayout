using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using Verse;

namespace RWLayout.Alpha1
{
    public class CGuiRoot : CElementHost
    {
        public bool FlexableWidth = false;
        public bool FlexableHeight = false;

        public Action LayoutUpdated;

        private ClStayConstraint leftStay = null;
        private ClStayConstraint rightStay = null;
        private ClStayConstraint topStay = null;
        private ClStayConstraint bottomStay = null;

        public override void UpdateLayoutConstraints()
        {
            leftStay = Solver.CreateStayConstrait(left, InRect.xMin, ClStrength.Required);
            if (!FlexableWidth)
            {
                rightStay = Solver.CreateStayConstrait(right, InRect.xMax, ClStrength.Required);
            }
            topStay = Solver.CreateStayConstrait(top, InRect.yMin, ClStrength.Required);
            if (!FlexableHeight)
            {
                bottomStay = Solver.CreateStayConstrait(bottom, InRect.yMax, ClStrength.Required);
            }

            base.UpdateLayoutConstraints();
        }

        public override void UpdateLayout()
        {
            Debug.WriteLine(InRect.width);

            Solver.UpdateStayConstrait(ref leftStay, InRect.xMin);
            Solver.UpdateStayConstrait(ref rightStay, InRect.xMax);
            Solver.UpdateStayConstrait(ref topStay, InRect.yMin);
            Solver.UpdateStayConstrait(ref bottomStay, InRect.yMax);

            base.UpdateLayout();

            Solver.Solve();
            //Log.Message(solver.ToString());

        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();
            Debug.WriteLine(bounds);
            //Log.Message($"CGuiRoot.PostLayoutUpdate: InRect: {InRect}; bounds: {bounds}");
            //Log.Message($"solver state of {NamePrefix()}:\n{solver}");
            LayoutUpdated?.Invoke();
        }
    }

}

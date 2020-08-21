using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using Verse;

namespace GuiMinilib
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

        public override void UpdateLayoutConstraints(ClSimplexSolver solver)
        {
            leftStay = solver.CreateStayConstrait(left, InRect.xMin, ClStrength.Required);
            if (!FlexableWidth)
            {
                rightStay = solver.CreateStayConstrait(right, InRect.xMax, ClStrength.Required);
            }
            topStay = solver.CreateStayConstrait(top, InRect.yMin, ClStrength.Required);
            if (!FlexableHeight)
            {
                bottomStay = solver.CreateStayConstrait(bottom, InRect.yMax, ClStrength.Required);
            }

            base.UpdateLayoutConstraints(solver);
        }

        public override void UpdateLayout()
        {
            Debug.WriteLine(InRect.width);

            solver.UpdateStayConstrait(ref leftStay, InRect.xMin);
            solver.UpdateStayConstrait(ref rightStay, InRect.xMax);
            solver.UpdateStayConstrait(ref topStay, InRect.yMin);
            solver.UpdateStayConstrait(ref bottomStay, InRect.yMax);

            base.UpdateLayout();

            solver.Solve();
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

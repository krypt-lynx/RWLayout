using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;

namespace RWLayout.Alpha1
{
    public partial class CElement
    {
        protected virtual ClSimplexSolver Solver { get { return Parent?.Solver; } }

        public void AddConstraint(ClStrength strength, ClConstraint constraint)
        {
            Solver.AddConstraint(strength, constraint);
        }

        public void AddConstraint(ClConstraint constraint)
        {
            Solver.AddConstraint(ClStrength.Strong, constraint);
        }

        public void AddConstraints(ClStrength strength, IEnumerable<ClConstraint> constraints)
        {
            foreach (var cn in constraints)
            {
                Solver.AddConstraint(strength, cn);
            }
        }

        public void AddConstraints(ClStrength strength, params ClConstraint[] constraints)
        {
            Solver.AddConstraints(strength, (IEnumerable<ClConstraint>)constraints);
        }

        public void AddConstraints(IEnumerable<ClConstraint> constraints)
        {
            Solver.AddConstraints(ClStrength.Strong, constraints);
        }

        public void AddConstraints(params ClConstraint[] constraints)
        {
            Solver.AddConstraints(ClStrength.Strong, constraints);
        }

        public void RemoveConstraint(ClConstraint constraint)
        {
            Solver.RemoveConstraint(constraint);
        }

        public void RemoveConstraint(IEnumerable<ClConstraint> constraints)
        {
            foreach (var cn in constraints)
            {
                RemoveConstraint(cn);
            }
        }

        public void RemoveConstraint(params ClConstraint[] constraints)
        {
            RemoveConstraint((IEnumerable<ClConstraint>)constraints);
        }

    }
}

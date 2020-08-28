using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;

namespace RWLayout.alpha2
{
    public class CLayoutGuide
    {
        public List<Anchor> Anchors = new List<Anchor>();
    }


    public partial class CElement
    {
        // todo: split solvers (copy + clean each?)
      
        public List<CLayoutGuide> Guides = new List<CLayoutGuide>();


        private void CreateSolver()
        {
            solver = new ClSimplexSolver();
            solver.Name = NamePrefix();
            solver.AutoSolve = false;
        }

        private ClSimplexSolver solver;
        protected virtual ClSimplexSolver Solver { 
            get {
                var parent = Parent;
                if (parent == null)
                {
                    if (solver == null)
                    {
                        CreateSolver();
                    }
                    return solver;
                }
                else
                {
                    return Parent.Solver;
                }
            }
        }

        public void AddConstraint(ClStrength strength, ClConstraint constraint)
        {
            //if (constraints.Contains(constraint))
            //{
            //    throw new InvalidOperationException($"Constraint {constraint} is already added to {this}");
            //}
            ValidateVariables(constraint);

            constraint.SetStrength(strength);
            //constraints.Add(constraint);

            Solver?.AddConstraint(strength, constraint);
        }

        private void ValidateVariables(ClConstraint constraint)
        {
            HashSet<ClVariable> anchors = Root.allAnchors().ToHashSet();
            foreach (var var in constraint.Expression.Terms.Keys)
            {
                if (!anchors.Contains(var))
                {
                    throw new InvalidOperationException($"Constraint {constraint} contains variable {var} from outside current elements tree");
                }
            }
        }

        public void RemoveConstraint(ClConstraint constraint)
        {
            //if (!constraints.Contains(constraint))
            //{
            //    throw new InvalidOperationException($"Constraint {constraint} in not added to {this}");
            //}

            //constraints.Remove(constraint);
            Solver?.RemoveConstraint(constraint);
        }


        public void AddConstraint(ClConstraint constraint)
        {
            AddConstraint(ClStrength.Strong, constraint);
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

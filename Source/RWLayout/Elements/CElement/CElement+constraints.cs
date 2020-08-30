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

        protected ClSimplexSolver solver;
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

        public void AddConstraint(ClConstraint constraint, ClStrength strength = null)
        {
            ValidateVariables(constraint);

            if (strength != null)
            {
                constraint.SetStrength(strength);
            }

            Solver?.AddConstraint(constraint);
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

        /// <summary>
        /// Removes all constraints leading into element
        /// </summary>
        /// <param name="element"></param>
        /// <returns>isolated constraints of element not connected to outside tree</returns>
        private List<ClConstraint> ShearConstraints(CElement element)
        {
            var cns = Solver.AllConstraints();
            var detachedAnchors = element.allAnchors().ToHashSet();
            var movedConstraints = new List<ClConstraint>();

            foreach (var cn in cns)
            {
                bool hasDetached = false;
                bool hasAttached = false;

                foreach (var var in cn.Expression.Terms.Keys)
                {
                    if (detachedAnchors.Contains(var))
                    {
                        hasDetached = true;
                    }
                    else
                    {
                        hasAttached = true;
                    }

                    if (hasAttached && hasDetached)
                    {
                        break;
                    }
                }

                if (hasDetached)
                {
                    Solver.RemoveConstraint(cn);
                    if (!hasAttached)
                    {
                        movedConstraints.Add(cn);
                    }
                }
            }

            foreach (var var in detachedAnchors)
            {
                Solver.RemoveVariable(var);
            }

            return movedConstraints;
        }

             

        public void AddConstraints(ClStrength strength, IEnumerable<ClConstraint> constraints)
        {
            foreach (var cn in constraints)
            {
                AddConstraint(cn, strength);
            }
        }

        public void AddConstraints(ClStrength strength, params ClConstraint[] constraints)
        {
            AddConstraints(strength, (IEnumerable<ClConstraint>)constraints);
        }

        public void AddConstraints(IEnumerable<ClConstraint> constraints)
        {
            foreach (var cn in constraints)
            {
                AddConstraint(cn);
            }
        }

        public void AddConstraints(params ClConstraint[] constraints)
        {
            AddConstraints((IEnumerable<ClConstraint>)constraints);
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

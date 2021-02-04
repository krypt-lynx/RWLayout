using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;

namespace RWLayout.alpha2
{

    public partial class CElement
    {
        /// <summary>
        /// Creates and configures simplex solver
        /// </summary>
        private void CreateSolver()
        {
            solver = new ClSimplexSolver();
            solver.Name = NamePrefix();
            solver.AutoSolve = false;
        }

        protected ClSimplexSolver solver;
        /// <summary>
        /// Cassowary Simplex Solver. Responsible for constraints resolving.
        /// </summary>
        protected virtual ClSimplexSolver Solver { // todo: protected
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
                    return parent.Solver;
                }
            }
        }

        /// <summary>
        /// Adds constraint.
        /// </summary>
        /// <param name="constraint">the constraint</param>
        /// <param name="strength">constraints strengh override. Not changed if null</param>
        /// <remarks>Every variable of the constraint should belong to current view tree</remarks>
        public CElement AddConstraint(ClConstraint constraint, ClStrength strength = null)
        {
            ValidateVariables(constraint);

            if (strength != null)
            {
                constraint.SetStrength(strength);
            }

            Solver.AddConstraint(constraint);

            SetNeedsUpdateLayout();

            return this;
        }

        /// <summary>
        /// Checks if used variables belong to current view tree
        /// </summary>
        private void ValidateVariables(ClConstraint constraint)
        {
            HashSet<ClVariable> anchors = Root.allAnchors().ToHashSet();
            foreach (var var in constraint.Expression.Terms.Keys)
            {
                if (!anchors.Contains(var))
                {
                    throw new InvalidOperationException($"Constraint {constraint} contains variable {var} from outside current elements tree {Root.NamePrefix()}");
                }
            }
        }

        /// <summary>
        /// Removes contraint.
        /// </summary>
        public CElement RemoveConstraint(ClConstraint constraint)
        {
            //if (!constraints.Contains(constraint))
            //{
            //    throw new InvalidOperationException($"Constraint {constraint} in not added to {this}");
            //}

            //constraints.Remove(constraint);
            Solver?.RemoveConstraint(constraint);
            SetNeedsUpdateLayout();
            return this;
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



        /// <summary>
        /// Adds multiple constraints.
        /// </summary>
        /// <param name="strength">constraints strengh override. Not changed if null</param>
        /// <param name="constraints">constraints list</param>
        /// <remarks>Every variable of the constraint should belong to current view tree</remarks>
        public CElement AddConstraints(ClStrength strength, IEnumerable<ClConstraint> constraints)
        {
            foreach (var cn in constraints)
            {
                AddConstraint(cn, strength);
            }
            return this;
        }

        /// <summary>
        /// Adds multiple constraints.
        /// </summary>
        /// <param name="strength">constraints strengh override. Not changed if null</param>
        /// <param name="constraints">constraints list</param>
        /// <remarks>Every variable of the constraint should belong to current view tree</remarks>
        public CElement AddConstraints(ClStrength strength, params ClConstraint[] constraints)
        {
            AddConstraints(strength, (IEnumerable<ClConstraint>)constraints);
            return this;
        }

        /// <summary>
        /// Adds multiple constraints.
        /// </summary>
        /// <param name="constraints">constraints list</param>
        /// <remarks>Every variable of the constraint should belong to current view tree</remarks>
        public CElement AddConstraints(IEnumerable<ClConstraint> constraints)
        {
            foreach (var cn in constraints)
            {
                AddConstraint(cn);
            }
            return this;
        }

        /// <summary>
        /// Adds multiple constraints.
        /// </summary>
        /// <param name="constraints">constraints list</param>
        /// <remarks>Every variable of the constraint should belong to current view tree</remarks>
        public CElement AddConstraints(params ClConstraint[] constraints)
        {
            AddConstraints((IEnumerable<ClConstraint>)constraints);
            return this;
        }

        /// <summary>
        /// Removes contraints.
        /// </summary>
        public CElement RemoveConstraint(IEnumerable<ClConstraint> constraints)
        {
            foreach (var cn in constraints)
            {
                RemoveConstraint(cn);
            }
            return this;
        }

        /// <summary>
        /// Removes contraints.
        /// </summary>
        public CElement RemoveConstraint(params ClConstraint[] constraints)
        {
            RemoveConstraint((IEnumerable<ClConstraint>)constraints);
            return this;
        }

    }
}

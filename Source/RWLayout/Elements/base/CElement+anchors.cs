// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Cassowary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2
{
    public struct Anchor
    {
        public ClVariable var;
        public ClConstraint cn;
    }

    public partial class CElement
    {

        public virtual void AddImpliedConstraints()
        {
            CreateConstraintIfNeeded(ref width_, () => left + width ^ right);
            CreateConstraintIfNeeded(ref height_, () => top + height ^ bottom);

            CreateConstraintIfNeeded(ref centerX_, () => centerX ^ (left + right) / 2);
            CreateConstraintIfNeeded(ref centerY_, () => centerY ^ (top + bottom) / 2);

            CreateConstraintIfNeeded(ref intrinsicWidth_, () => new ClStayConstraint(intrinsicWidth));
            CreateConstraintIfNeeded(ref intrinsicHeight_, () => new ClStayConstraint(intrinsicHeight));
        }

        public void RemoveImpliedConstraints(ClSimplexSolver solver)
        {
            RemoveVariableIfNeeded(ref left_);
            RemoveVariableIfNeeded(ref top_);
            RemoveVariableIfNeeded(ref right_);
            RemoveVariableIfNeeded(ref bottom_);

            RemoveVariableIfNeeded(ref width_);
            RemoveVariableIfNeeded(ref height_);
            RemoveVariableIfNeeded(ref centerX_);
            RemoveVariableIfNeeded(ref centerY_);
            RemoveVariableIfNeeded(ref intrinsicWidth_);
            RemoveVariableIfNeeded(ref intrinsicHeight_);
        }

        // core variables
        protected Anchor left_ = new Anchor();
        protected Anchor top_ = new Anchor();
        protected Anchor right_ = new Anchor();
        protected Anchor bottom_ = new Anchor();

        public ClVariable left => GetVariable(ref left_, "L");
        public ClVariable top => GetVariable(ref top_, "T");
        public ClVariable right => GetVariable(ref right_, "R");
        public ClVariable bottom => GetVariable(ref bottom_, "B");

        // non-esential variables
        protected Anchor width_ = new Anchor();
        protected Anchor height_ = new Anchor();
        protected Anchor centerX_ = new Anchor();
        protected Anchor centerY_ = new Anchor();
        protected Anchor intrinsicWidth_ = new Anchor();
        protected Anchor intrinsicHeight_ = new Anchor();

        public ClVariable width => GetVariable(ref width_, "W");
        public ClVariable height => GetVariable(ref height_, "H");
        public ClVariable centerX => GetVariable(ref centerX_, "cX");
        public ClVariable centerY => GetVariable(ref centerY_, "cY");
        public ClVariable intrinsicWidth => GetVariable(ref intrinsicWidth_, "iW");
        public ClVariable intrinsicHeight => GetVariable(ref intrinsicHeight_, "iH");

        // anchor helpers
        public void CreateConstraintIfNeeded(ref Anchor pair, Func<ClConstraint> builder)
        {
            if (pair.var != null && pair.cn == null)
            {
                Solver.AddConstraint(ClStrength.Required, pair.cn = builder()); // implied constrains going directly into solver
            }
        }
        public ClVariable GetVariable(ref Anchor pair, string name)
        {
            if (pair.var == null)
            {
                pair.var = new ClVariable($"{NamePrefix()}_{name}");
            }
            return pair.var;
        }
        public void UpdateStayConstrait(ref Anchor pair, double value)
        {
            if (pair.cn == null)
            {
                return;
            }
            if (Cl.Approx(pair.var.Value, value))
            {
                return;
            }

            pair.var.Value = value;
            var newStay = new ClStayConstraint(pair.var, pair.cn.Strength);
            Solver.RemoveConstraint(pair.cn); // implied constrains going directly into solver
            Solver.AddConstraint(pair.cn = newStay); // implied constrains going directly into solver
        }
        public void RemoveVariableIfNeeded(ref Anchor pair)
        {
            if (pair.var != null)
            {
                Solver.RemoveVariable(pair.var); // implied constrains going directly into solver
                pair.cn = null;
            }
        }
    }
}

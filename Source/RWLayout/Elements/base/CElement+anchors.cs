// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Cassowary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.Alpha1
{
    public partial class CElement
    {

        private void CreateAnchors()
        {
            string variableNameBase = NamePrefix();

            left = new ClVariable(variableNameBase + "_L");
            top = new ClVariable(variableNameBase + "_T");
            right = new ClVariable(variableNameBase + "_R");
            bottom = new ClVariable(variableNameBase + "_B");
        }

        public virtual void AddImpliedConstraints()
        {
            AddImpliedConstraint(ref width_, () => left + width ^ right);
            AddImpliedConstraint(ref height_, () => top + height ^ bottom);

            AddImpliedConstraint(ref centerX_, () => centerX ^ (left + right) / 2);
            AddImpliedConstraint(ref centerY_, () => centerY ^ (top + bottom) / 2);

            AddImpliedConstraint(ref intrinsicWidth_, () => CreateStayConstrait(intrinsicWidth, 0, ClStrength.Required));
            AddImpliedConstraint(ref intrinsicHeight_, () => CreateStayConstrait(intrinsicHeight, 0, ClStrength.Required));
        }

        public void RemoveImpliedConstraints(ClSimplexSolver solver)
        {
            solver.RemoveVariable(left);
            solver.RemoveVariable(top);
            solver.RemoveVariable(right);
            solver.RemoveVariable(bottom);

            if (width_.cn != null) solver.RemoveVariable(width_.var);
            if (height_.cn != null) solver.RemoveVariable(height_.var);
            if (centerX_.cn != null) solver.RemoveVariable(centerX_.var);
            if (centerY_.cn != null) solver.RemoveVariable(centerY_.var);
            if (intrinsicWidth_.cn != null) solver.RemoveVariable(intrinsicWidth_.var);
            if (intrinsicHeight_.cn != null) solver.RemoveVariable(intrinsicHeight_.var);
        }

        public ClVariable left;
        public ClVariable top;
        public ClVariable right;
        public ClVariable bottom;

        // non-esential variables

        private (ClVariable var, ClConstraint cn) width_ = (null, null);
        private (ClVariable var, ClConstraint cn) height_ = (null, null);
        private (ClVariable var, ClConstraint cn) centerX_ = (null, null);
        private (ClVariable var, ClConstraint cn) centerY_ = (null, null);
        private (ClVariable var, ClConstraint cn) intrinsicWidth_ = (null, null);
        private (ClVariable var, ClConstraint cn) intrinsicHeight_ = (null, null);


        private void AddImpliedConstraint(ref (ClVariable var, ClConstraint cn) pair, Func<ClConstraint> builder)
        {
            if (pair.var != null && pair.cn == null)
            {                
                Solver.AddConstraint(ClStrength.Required, pair.cn = builder());
            }
        }
        private ClVariable GetVariable(ref (ClVariable var, ClConstraint cn) pair, string name)
        {
            if (pair.var == null)
            {
                pair.var = new ClVariable($"{NamePrefix()}_{name}");
            }
            return pair.var;
        }

        static public ClStayConstraint CreateStayConstrait(ClVariable variable, double value, ClStrength strength = null)
        {
            variable.Value = value;
            var newStay = new ClStayConstraint(variable, strength == null ? ClStrength.Required : strength);
            return newStay;
        }

        public void UpdateStayConstrait(ref (ClVariable var, ClConstraint cn) pair, double value)
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
            Solver.RemoveConstraint(pair.cn);
            Solver.AddConstraint(pair.cn = newStay);
        }

        public ClVariable width => GetVariable(ref width_, "W");
        public ClVariable height => GetVariable(ref height_, "H");
        public ClVariable centerX => GetVariable(ref centerX_, "cX");
        public ClVariable centerY => GetVariable(ref centerY_, "cY");
        public ClVariable intrinsicWidth => GetVariable(ref intrinsicWidth_, "iW");
        public ClVariable intrinsicHeight => GetVariable(ref intrinsicHeight_, "iH");
    }
}

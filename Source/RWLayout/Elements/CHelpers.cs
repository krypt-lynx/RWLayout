﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;

namespace RWLayout.alpha2
{
    static class CHelpers
    {
        /*
        static public ClStayConstraint CreateStayConstrait(this ClSimplexSolver solver, ClVariable variable, double value, ClStrength strength = null)
        {
            variable.Value = value;
            var newStay = new ClStayConstraint(variable, strength == null ? ClStrength.Required : strength);
            AddConstraint(newStay);
            return newStay;
        }
        static public void UpdateStayConstrait(this ClSimplexSolver solver, ref ClStayConstraint constraint, double value)
        {
            if (constraint == null)
            {
                return;
            }
            var var = constraint.Variable;
            if (Cl.Approx(var.Value, value))
            {
                return;
            }

            var.Value = value;
            var newStay = new ClStayConstraint(var, constraint.Strength);
            solver.RemoveConstraint(constraint);
            AddConstraint(newStay);
            constraint = newStay;
        }

        */


    }
}
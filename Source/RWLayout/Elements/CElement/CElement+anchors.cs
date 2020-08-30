// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Cassowary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

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

        
        private IEnumerable<ClVariable> enumerateAnchors()
        {
            yield return left_.var;
            yield return top_.var;
            yield return right_.var;
            yield return bottom_.var;

            yield return width_.var;
            yield return height_.var;
            yield return centerX_.var;
            yield return centerY_.var;
            yield return intrinsicWidth_.var;
            yield return intrinsicHeight_.var;

            foreach (var guide in Guides)
            {
                foreach (var anchor in guide.Anchors)
                {
                    yield return anchor.var;
                }
            }
        }

        protected virtual IEnumerable<ClVariable> allAnchors()
        {
            IEnumerable<ClVariable> allAnchors = enumerateAnchors().Where(x => x != null);

            foreach (var element in elements)
            {
                allAnchors = allAnchors.Concat(element.allAnchors());
            }

            return allAnchors;
        }

        // anchor helpers
        public void CreateConstraintIfNeeded(ref Anchor pair, Func<ClConstraint> builder)
        {
            if (pair.var != null && pair.cn == null)
            {
                try
                {
                    Solver.AddConstraint(ClStrength.Required, pair.cn = builder()); // implied constrains going directly into solver
                    // InErrorState = false; // todo:
                }
                catch (Exception e)
                {
                    // InErrorState = true;
                    Log.Error($"{e.GetType().Name} was thrown during constraint {pair.cn} adding: {e.Message}");
                    Log.Message($"solver's constraints:\n{string.Join("\n", Solver.AllConstraints().Select(x => x.ToString()))}");
                }
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
            if (pair.var == null || Cl.Approx(pair.var.Value, value))
            {
                return;
            }

            pair.var.Value = value;

            if (pair.cn == null)
            {
                return;
            }

            try
            {
                var newStay = new ClStayConstraint(pair.var, pair.cn.Strength);
                Solver.TryRemoveConstraint(pair.cn); // implied constrains going directly into solver
                Solver.AddConstraint(pair.cn = newStay); // implied constrains going directly into solver
                // InErrorState = false; // todo:
            }
            catch (Exception e)
            {
                // InErrorState = true;
                var sb = new StringBuilder();
                sb.AppendLine($"{e.GetType().Name} was thrown during stay {pair.var} update: {e.Message}");
                sb.AppendLine($"{e.StackTrace}");
                sb.AppendLine();
                sb.AppendLine($"solver's constraints:\n{string.Join("\n", Solver.AllConstraints().Select(x => x.ToString()))}");
                Log.Error(sb.ToString());
            }

        }
        public void RemoveVariableIfNeeded(ref Anchor pair)
        {
            if (pair.var != null)
            {
                try
                {
                    Solver.RemoveVariable(pair.var); // implied constrains going directly into solver
                    pair.cn = null;
                    // InErrorState = false; // todo:
                }
                catch (Exception e)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"{e.GetType().Name} thrown during constraint update: {e.Message}");
                    sb.AppendLine($"{e.StackTrace}");
                    sb.AppendLine();
                    sb.AppendLine($"solver's constraints:\n{string.Join("\n", Solver.AllConstraints().Select(x => x.ToString()))}");
                    Log.Error(sb.ToString());
                    // InErrorState = true;
                }
            }
        }
    }
}

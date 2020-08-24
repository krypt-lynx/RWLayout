// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Cassowary_moddiff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout_moddiff
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

        private void AddImpliedConstraints(ClSimplexSolver solver)
        {
            if (width_ != null)
            {
                solver.AddConstraint(new ClLinearEquation(right, left + width, ClStrength.Required));
            }
            if (height_ != null)
            {
                solver.AddConstraint(new ClLinearEquation(bottom, top + height, ClStrength.Required));
            }
            if (centerX_ != null)
            {
                solver.AddConstraint(new ClLinearEquation(centerX, (left + right) / 2, ClStrength.Required));
            }
            if (centerY_ != null)
            {
                solver.AddConstraint(new ClLinearEquation(centerY, (top + bottom) / 2, ClStrength.Required));
            }
        }

        private void RemoveAnchors(ClSimplexSolver solver)
        {
            solver.RemoveVariable(left);
            solver.RemoveVariable(top);
            solver.RemoveVariable(right);
            solver.RemoveVariable(bottom);

            if (width_ != null) solver.RemoveVariable(width_);
            if (height_ != null) solver.RemoveVariable(height_);
            if (centerX_ != null) solver.RemoveVariable(centerX_);
            if (centerY_ != null) solver.RemoveVariable(centerY_);
            if (intrinsicWidth_ != null) solver.RemoveVariable(intrinsicWidth_);
            if (intrinsicHeight_ != null) solver.RemoveVariable(intrinsicHeight_);
        }

        public ClVariable left;
        public ClVariable top;
        public ClVariable right;
        public ClVariable bottom;

        // non-esential variables
        private ClVariable width_;
        private ClVariable height_;
        private ClVariable centerX_;
        private ClVariable centerY_;
        private ClVariable intrinsicWidth_;
        private ClStayConstraint intrinsicWidthConstraint_;
        private ClVariable intrinsicHeight_;
        private ClStayConstraint intrinsicHeightConstraint_;

        public ClVariable width
        {
            get
            {
                if (width_ == null)
                {
                    width_ = new ClVariable(NamePrefix() + "_W");
                }
                return width_;
            }
        }
        public ClVariable height
        {
            get
            {
                if (height_ == null)
                {
                    height_ = new ClVariable(NamePrefix() + "_H");
                }
                return height_;
            }
        }

        public ClVariable centerX
        {
            get
            {
                if (centerX_ == null)
                {
                    centerX_ = new ClVariable(NamePrefix() + "_cX");
                }
                return centerX_;
            }
        }
        public ClVariable centerY
        {
            get
            {
                if (centerY_ == null)
                {
                    centerY_ = new ClVariable(NamePrefix() + "_cY");
                }
                return centerY_;
            }
        }
        public ClVariable intrinsicWidth
        {
            get
            {
                if (intrinsicWidth_ == null)
                {
                    intrinsicWidth_ = new ClVariable(NamePrefix() + "_iW");
                    intrinsicWidthConstraint_ = Solver.CreateStayConstrait(intrinsicWidth_, 0, ClStrength.Required);
                }
                return intrinsicWidth_;
            }
        }
        public ClVariable intrinsicHeight
        {
            get
            {
                if (intrinsicHeight_ == null)
                {
                    intrinsicHeight_ = new ClVariable(NamePrefix() + "_iH");
                    intrinsicHeightConstraint_ = Solver.CreateStayConstrait(intrinsicHeight_, 0, ClStrength.Required);
                }
                return intrinsicHeight_;
            }
        }

    }
}

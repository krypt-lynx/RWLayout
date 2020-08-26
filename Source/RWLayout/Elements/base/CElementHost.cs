// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Cassowary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.Alpha1
{
    public class CElementHost : CElement
    {
        public Rect _inRect;
        public Rect InRect
        {
            get { return _inRect; }
            set
            {
                needsUpdateLayout = _inRect != value;
                _inRect = value;

                UpdateLayoutConstraintsIfNeeded();
                UpdateLayoutIfNeeded();
            }
        }

        ClSimplexSolver solver_;
        public override ClSimplexSolver Solver
        {
            get
            {
                return solver_;
            }
        }

        public CElementHost() : base()
        {
            solver_ = new ClSimplexSolver();
            //solver_.DebugName = NamePrefix() + "_solver";
            Solver.AutoSolve = false;
        }

        protected bool needsUpdateLayoutConstraints = true;
        public void UpdateLayoutConstraintsIfNeeded()
        {
            if (needsUpdateLayoutConstraints)
            {
                UpdateLayoutConstraints();

                PostConstraintsUpdate();

                UpdateLayout(); // forcefully updating layout after init to resolve multiline labels
                PostLayoutUpdate();
                needsUpdateLayout = true;
            }
        }
        public override void UpdateLayoutConstraints()
        {
            needsUpdateLayoutConstraints = false;

            base.UpdateLayoutConstraints();
            Solver.Solve();
        }

        protected bool needsUpdateLayout = true;
        public void UpdateLayoutIfNeeded()
        {
            if (needsUpdateLayout)
            {

                UpdateLayout();

                PostLayoutUpdate();
            }
        }
        public override void UpdateLayout()
        {
            needsUpdateLayout = false;

            UpdateLayoutConstraintsIfNeeded();

            base.UpdateLayout();

        }
    }
}

﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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

namespace RWLayout.alpha2
{
    public class CElementHost : CElement
    {
        public bool InErrorState { get; private set; } = false;
        public Rect _inRect;
        public Rect InRect
        {
            get { return _inRect; }
            set
            {
                needsUpdateLayout = _inRect != value;
                _inRect = value;

                //UpdateLayoutConstraintsIfNeeded();
                UpdateLayoutIfNeeded();
            }
        }



        WeakReference owner_ = null;

        /// <summary>
        /// Weak reverese to owning element
        /// </summary>
        public CElement Owner
        {
            get { return owner_?.IsAlive ?? false ? owner_.Target as CElement : null; }
            set { owner_ = new WeakReference(value, false); }
        }


        /*
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
        }*/

        protected bool needsUpdateLayout = true;

        public override bool NeedsUpdateLayout { get => needsUpdateLayout; set => needsUpdateLayout |= value; }
        /*public void UpdateLayoutIfNeeded()
        {
            if (needsUpdateLayout)
            {

                UpdateLayout();

                PostLayoutUpdate();
            }
        }*/
        bool retry = false;
        public override void UpdateLayout()
        {
            needsUpdateLayout = true;

            //UpdateLayoutConstraintsIfNeeded();

            base.UpdateLayout();
            try
            {
                Solver.Solve();
                InErrorState = false;
            }
            catch (Exception e)
            {
                InErrorState = true;
                var sb = new StringBuilder();
                sb.AppendLine($"{e.GetType().Name} thrown during constraints solving: {e.Message}");
                sb.AppendLine($"{e.StackTrace}");
                sb.AppendLine();
                sb.AppendLine($"solver's constraints:\n{string.Join("\n", Solver.AllConstraints().Select(x => x.ToString()))}");
                Log.Error(sb.ToString());

                if (solver == Solver)
                {
                    var copy = solver.AllConstraints().ToArray();
                    solver = null;
                    Solver.AddConstraints(copy);
                    Log.Warning("solver reseted");
                }

                if (!retry)
                {
                    retry = true;
                    UpdateLayout();
                    retry = false;
                }
            }
        }

        public override void DoContent()
        {
            base.DoContent();
            if (InErrorState)
            {
                GuiTools.ColorPush(Color.red);
                GuiTools.Box(bounds, new EdgeInsets(3));

                GuiTools.TextAnchorPush(TextAnchor.MiddleCenter);
                GuiTools.FontPush(GameFont.Tiny);

                Widgets.Label(bounds, NamePrefix() + "\nexception during update");

                GuiTools.FontPop();
                GuiTools.TextAnchorPop();
                GuiTools.ColorPop();
            }
        }
    }
}

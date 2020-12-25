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
    /// <summary>
    /// This view can be oftend by other view without actually adding it as a child
    /// </summary>
    public class COwnedElement : CElement
    {
        Verse.WeakReference<CElement> owner_ = null;

        /// <summary>
        /// Weak reverese to owning element
        /// </summary>
        public CElement Owner
        {
            get { return owner_?.Target; }
            set { owner_ = new Verse.WeakReference<CElement>(value); }
        }
    }

    /// <summary>
    /// base class for hierarchy root.
    /// </summary>
    public class CElementHost : COwnedElement
    {
        /// <summary>
        /// Indicates inability to resolve constraints
        /// </summary>
        public bool InErrorState { get; private set; } = false;
        public Rect _inRect;

        /// <summary>
        /// Expected view rect
        /// </summary>
        public Rect InRect
        {
            get { return _inRect; }
            set
            {
                if (_inRect != value)
                {
                    SetNeedsUpdateLayout();
                    _inRect = value;
                }
            }
        }


        protected bool needsUpdateLayout = true;

        public override bool NeedsUpdateLayout()
        {
            return needsUpdateLayout;
        }

        public override void SetNeedsUpdateLayout()
        {
            needsUpdateLayout = true;
        }

        bool retry = false;
        public override void UpdateLayout()
        {
            //Log.Message($"Layout Update: {NamePrefix()}");

            needsUpdateLayout = false;

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
                GuiTools.PushColor(Color.red);
                GuiTools.Box(BoundsRounded, new EdgeInsets(3));

                GuiTools.PushTextAnchor(TextAnchor.MiddleCenter);
                GuiTools.PushFont(GameFont.Tiny);

                Widgets.Label(BoundsRounded, NamePrefix() + "\nexception during update");

                GuiTools.PopFont();
                GuiTools.PopTextAnchor();
                GuiTools.PopColor();
            }
        }
    }
}

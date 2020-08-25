using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using UnityEngine;
using Verse;

namespace RWLayout.Alpha1
{

    public class CWindow : Window, IElement
    {
        public ClVariable guideWidth = new ClVariable("CWindow_W");
        private ClStayConstraint guideWidthConstraint_;
        public ClVariable guideHeight = new ClVariable("CWindow_H");
        private ClStayConstraint guideHeightConstraint_;
        public ClVariable adjustedScreenWidth = new ClVariable("screen_W");
        private ClStayConstraint screenWidthConstraint_;
        public ClVariable adjustedScreenHeight = new ClVariable("screen_H");
        private ClStayConstraint screenHeightConstraint_;

        CWindowResizer resizer;

        public override void WindowOnGUI()
        {
            if (resizer == null)
            {
                resizer = new CWindowResizer();
                typeof(Window).GetField("resizer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, resizer);

                resizer.UpdateSize = (ref Vector2 winSize) =>
                { 
                    var fixedSize = winSize - MarginsSize();

                    Gui.Solver.UpdateStayConstrait(ref guideWidthConstraint_, fixedSize.x);
                    Gui.Solver.UpdateStayConstrait(ref guideHeightConstraint_, fixedSize.y);

                    Gui.InRect = new Rect(Vector2.zero, fixedSize);
                    winSize = Gui.bounds.size + MarginsSize();
                };
            }

            base.WindowOnGUI();
        }

        public CGuiRoot Gui { get; private set; }
        public Vector2 initSize = new Vector2(100, 100);
        public override Vector2 InitialSize
        {
            get
            {
                return initSize + MarginsSize();
            }
        }

        public Vector2 MarginsSize()
        {
            var size = new Vector2(Margin * 2, Margin * 2 + (optionalTitle == null ? 0 : Margin + 25f)); 
            return size;
        }

        public Vector2 InnerSize
        {
            get
            {
                return windowRect.size - MarginsSize();
            }
            set
            {
                initSize = value;
                windowRect = new Rect(windowRect.position, value + MarginsSize());
            }
        }

        public CWindow() : this(new Vector2(100, 100)) { }

        public CWindow(Vector2 estimatedSize) : base()
        {
            Gui = new CGuiRoot();
            Gui.FlexableHeight = true; 
            Gui.FlexableWidth = true;
            initSize = estimatedSize;        
        }

        public override void PreOpen()
        {
            var timer = new Stopwatch();
            timer.Start();
            ConstructGui();
            timer.Stop();

            Log.Message($"{this.GetType().Name}: gui constructed in: {timer.Elapsed}");

            //Gui.Solver.Remove

            guideWidthConstraint_ = Gui.Solver.CreateStayConstrait(guideWidth, initSize.x, ClStrength.Required);
            guideHeightConstraint_ = Gui.Solver.CreateStayConstrait(guideHeight, initSize.y, ClStrength.Required);

            var margins = MarginsSize();
            screenWidthConstraint_ = Gui.Solver.CreateStayConstrait(adjustedScreenWidth, UI.screenWidth - margins.x, ClStrength.Required);
            screenHeightConstraint_ = Gui.Solver.CreateStayConstrait(adjustedScreenHeight, UI.screenHeight - margins.y, ClStrength.Required);

            Gui.InRect = new Rect(Vector2.zero, initSize);
            initSize = Gui.bounds.size;
            Gui.LayoutUpdated = () =>
            {
                //Log.Message($"CWindow new Gui size: {Gui.bounds.size}");
                InnerSize = Gui.bounds.size;
            };

            base.PreOpen();
        }

        public virtual void ConstructGui()
        {

        }

        public override void DoWindowContents(Rect inRect)
        {
            Gui.Solver.UpdateStayConstrait(ref guideWidthConstraint_, inRect.width);
            Gui.Solver.UpdateStayConstrait(ref guideHeightConstraint_, inRect.height);

            Gui.InRect = inRect;
            
            Gui.DoElementContent();
        }

        public override void Notify_ResolutionChanged()
        {
            var margins = MarginsSize();
            Gui.Solver.UpdateStayConstrait(ref screenWidthConstraint_, UI.screenWidth - margins.x);
            Gui.Solver.UpdateStayConstrait(ref screenHeightConstraint_, UI.screenHeight - margins.y);

            Gui.Solver.Solve();
            Gui.UpdateLayout();
            base.Notify_ResolutionChanged();
        }

        public ClSimplexSolver Solver
        {
            get
            {
                return Gui.Solver;
            }
        }

        public CElement Parent
        {
            get
            {
                return null;
            }
        }

        public T AddElement<T>(T element) where T : CElement
        {
            return Gui.AddElement(element);
        }

        public void RemoveElement(CElement element)
        {
            Gui.RemoveElement(element);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using UnityEngine;
using Verse;

namespace GuiMinilib
{

    public class CWindow : Window
    {
        public ClVariable width = new ClVariable("CWindow_W");
        private ClStayConstraint widthConstraint_;
        public ClVariable height = new ClVariable("CWindow_H");
        private ClStayConstraint heightConstraint_;
        public ClVariable screenWidth = new ClVariable("screen_W");
        private ClStayConstraint screenWidthConstraint_;
        public ClVariable screenHeight = new ClVariable("screen_H");
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
                    var fixedSize = winSize - MarginSize();

                    Gui.solver.UpdateStayConstrait(ref widthConstraint_, fixedSize.x);
                    Gui.solver.UpdateStayConstrait(ref heightConstraint_, fixedSize.y);

                    Gui.InRect = new Rect(Vector2.zero, fixedSize);
                    winSize = Gui.bounds.size + MarginSize();
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
                return initSize + MarginSize();
            }
        }

        public Vector2 MarginSize()
        {
            var size = new Vector2(Margin * 2, Margin * 2 + (optionalTitle == null ? 0 : Margin + 25f)); 
            return size;
        }

        public Vector2 InnerSize
        {
            get
            {
                return windowRect.size - MarginSize();
            }
            set
            {
                initSize = value;
                windowRect = new Rect(windowRect.position, value + MarginSize());
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

            widthConstraint_ = Gui.solver.CreateStayConstrait(width, initSize.x, ClStrength.Required);
            heightConstraint_ = Gui.solver.CreateStayConstrait(height, initSize.y, ClStrength.Required);
            screenWidthConstraint_ = Gui.solver.CreateStayConstrait(screenWidth, UI.screenWidth, ClStrength.Required);
            screenHeightConstraint_ = Gui.solver.CreateStayConstrait(screenHeight, UI.screenHeight, ClStrength.Required);

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
            Gui.solver.UpdateStayConstrait(ref widthConstraint_, inRect.width);
            Gui.solver.UpdateStayConstrait(ref heightConstraint_, inRect.height);

            Gui.InRect = inRect;
            
            Gui.DoElementContent();
        }

        public override void Notify_ResolutionChanged()
        {
            Gui.solver.UpdateStayConstrait(ref screenWidthConstraint_, UI.screenWidth);
            Gui.solver.UpdateStayConstrait(ref screenHeightConstraint_, UI.screenHeight);
            Gui.solver.Solve();
            Gui.UpdateLayout();
            base.Notify_ResolutionChanged();
        }
    }
}

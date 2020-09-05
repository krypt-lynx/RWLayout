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

namespace RWLayout.alpha2
{

    public class CWindow : Window
    {



        CWindowResizer resizer;

        public override void WindowOnGUI()
        {
            if (resizer == null)
            {
                resizer = new CWindowResizer();
                typeof(Window).GetField("resizer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, resizer);
                resizer.minWindowSize = MarginsSize();
                resizer.UpdateSize = (ref Vector2 winSize) =>
                { 
                    var fixedSize = winSize - MarginsSize();

                    Gui.UpdateWindowGuide(fixedSize);

                    Gui.InRect = new Rect(Vector2.zero, fixedSize);
                    Gui.UpdateLayoutIfNeeded();
                    winSize = Gui.Bounds.size + MarginsSize();
                };
            }

            base.WindowOnGUI();
        }

        public CWindowRoot Gui { get; private set; }
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
            Gui = new CWindowRoot();
            initSize = estimatedSize;        
        }

        public override void PreOpen()
        {
            var timer = new Stopwatch();

            timer.Start();
            ConstructGui();

            //Gui.Solver.Remove

            Gui.AddImpliedConstraints();

            Gui.UpdateWindowGuide(initSize);


            Gui.UpdateScreenGuide(new Vector2(UI.screenWidth, UI.screenHeight) - MarginsSize());

            Gui.InRect = new Rect(Vector2.zero, initSize);
            Gui.UpdateLayoutIfNeeded();
            initSize = Gui.Bounds.size;
            Gui.LayoutUpdated = () =>
            {
                //Log.Message($"CWindow new Gui size: {Gui.bounds.size}");
                InnerSize = Gui.Bounds.size;
            };

            timer.Stop();
            Log.Message($"{this.GetType().Name}: gui constructed in: {timer.Elapsed}");

            base.PreOpen();
        }

        public virtual void ConstructGui()
        {

        }

        public virtual void MakeResizable(bool vertical = false, bool horizontal = false)
        {
            MakeResizable(vertical, horizontal, null);
        }

        public virtual void MakeResizable(bool vertical, bool horizontal, ClStrength strength)
        {
            if (strength == null)
            {
                strength = ClStrength.Weak;
            }
            if (horizontal)
            {
                Gui.AddConstraint(Gui.width ^ Gui.WindowSize.width, strength);
            }
            if (vertical)
            {
                Gui.AddConstraint(Gui.height ^ Gui.WindowSize.height, strength);
            }
            resizeable = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Gui.UpdateAndDoContent(inRect);
        }

        public override void Notify_ResolutionChanged()
        {
            Gui.UpdateScreenGuide(new Vector2(UI.screenWidth, UI.screenHeight) - MarginsSize());

            //Gui.UpdateLayoutIfNeeded();

            base.Notify_ResolutionChanged();
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

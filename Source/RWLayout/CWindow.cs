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

        private Anchor guideWidth_ = new Anchor();
        private Anchor guideHeight_ = new Anchor();
        private Anchor adjustedScreenWidth_ = new Anchor();
        private Anchor adjustedScreenHeight_ = new Anchor();


        public ClVariable guideWidth => Gui.GetVariable(ref guideWidth_, "wW");
        public ClVariable guideHeight => Gui.GetVariable(ref guideHeight_, "wH");
        public ClVariable adjustedScreenWidth => Gui.GetVariable(ref adjustedScreenWidth_, "sW");
        public ClVariable adjustedScreenHeight => Gui.GetVariable(ref adjustedScreenHeight_, "sH");

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

                    Gui.UpdateStayConstrait(ref guideWidth_, fixedSize.x);
                    Gui.UpdateStayConstrait(ref guideHeight_, fixedSize.y);

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

            //Gui.Solver.Remove

            Gui.CreateConstraintIfNeeded(ref guideWidth_, () => { 
                guideWidth.Value = initSize.x;
                return new ClStayConstraint(guideWidth);
            });

            Gui.CreateConstraintIfNeeded(ref guideHeight_, () => { 
                guideHeight.Value = initSize.y; 
                return new ClStayConstraint(guideHeight); 
            });

            Gui.UpdateStayConstrait(ref guideWidth_, initSize.x);
            Gui.UpdateStayConstrait(ref guideHeight_, initSize.y);

            Gui.CreateConstraintIfNeeded(ref adjustedScreenWidth_, () => new ClStayConstraint(adjustedScreenWidth));
            Gui.CreateConstraintIfNeeded(ref adjustedScreenHeight_, () => new ClStayConstraint(adjustedScreenHeight));

            var margins = MarginsSize();
            Gui.UpdateStayConstrait(ref adjustedScreenWidth_, UI.screenWidth - margins.x);
            Gui.UpdateStayConstrait(ref adjustedScreenHeight_, UI.screenHeight - margins.y);

            Gui.InRect = new Rect(Vector2.zero, initSize);
            initSize = Gui.bounds.size;
            Gui.LayoutUpdated = () =>
            {
                //Log.Message($"CWindow new Gui size: {Gui.bounds.size}");
                InnerSize = Gui.bounds.size;
            };

            timer.Stop();
            Log.Message($"{this.GetType().Name}: gui constructed in: {timer.Elapsed}");

            base.PreOpen();
        }

        public virtual void ConstructGui()
        {

        }

        public override void DoWindowContents(Rect inRect)
        {
            Gui.UpdateStayConstrait(ref guideWidth_, inRect.width);
            Gui.UpdateStayConstrait(ref guideHeight_, inRect.height);

            Gui.InRect = inRect;
            
            Gui.DoElementContent();
        }

        public override void Notify_ResolutionChanged()
        {
            var margins = MarginsSize();
            Gui.UpdateStayConstrait(ref adjustedScreenWidth_, UI.screenWidth - margins.x);
            Gui.UpdateStayConstrait(ref adjustedScreenHeight_, UI.screenHeight - margins.y);

            Gui.UpdateLayout();

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

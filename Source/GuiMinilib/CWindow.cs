using System;
using System.Collections.Generic;
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

        /*
        public WindowResizer Resizer()
        {
            return (WindowResizer)typeof(Window).GetField("resizer", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this);
        }*/
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
        Vector2 initSize = new Vector2(100, 100);
        public override Vector2 InitialSize
        {
            get
            {
                return initSize + MarginSize();
            }
        }

        private Vector2 MarginSize()
        {
            return new Vector2(Margin * 2, Margin * 2);
        }

        public CWindow() : this(new Vector2(100, 100)) { }

        public CWindow(Vector2 estimatedSize) : base()
        {
            Gui = new CGuiRoot();
            Gui.FlexableHeight = true; 
            Gui.FlexableWidth = true;
            initSize = estimatedSize;

            widthConstraint_ = Gui.solver.CreateStayConstrait(width, initSize.x, ClStrength.Required);
            heightConstraint_ = Gui.solver.CreateStayConstrait(height, initSize.y, ClStrength.Required);

            ConstructGui();

            Gui.InRect = new Rect(Vector2.zero, estimatedSize);
            initSize = Gui.bounds.size;
            Gui.LayoutUpdated = () =>
            {
                //Log.Message($"CWindow new Gui size: {Gui.bounds.size}");
                //windowRect = new Rect(windowRect.position, Gui.bounds.size + MarginSize());
               
                if (resizer != null)
                {
                    resizer.minWindowSize = Gui.bounds.size + MarginSize();
                }
            };
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
    }
}

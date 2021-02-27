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
using RWLayout.alpha2.FastAccess;

namespace RWLayout.alpha2
{
    public interface IWindow
    {
        public bool ForceOnGUI { get; }
    }

    /// <summary>
    /// Window subclass providing host for constraint based views and methods for window geometry 
    /// </summary>
    public class CWindow : Window, IWindow
    {
        CWindowResizer resizer;

        static Setter<Window, WindowResizer> set_Window_resizer = Dynamic.InstanceSetField<Window, WindowResizer>("resizer");
        public override void WindowOnGUI()
        {
            if (resizer == null)
            {
                resizer = new CWindowResizer();
                set_Window_resizer(this, resizer);
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

        /// <summary>
        /// Root gui view of the window
        /// </summary>
        public CWindowRoot Gui { get; private set; }
        private Vector2 initSize = new Vector2(100, 100);

        public static bool ShowWindowDrawingTime = false;
        Stopwatch timer = new Stopwatch();

        /// <summary>
        /// Intitial size of the view (native RW method)
        /// </summary>
        public override Vector2 InitialSize
        {
            get
            {
                return initSize + MarginsSize();
            }
        }

        /// <summary>
        /// Size of the window margins (accounted to all modifications of it)
        /// </summary>
        /// <returns></returns>
        public Vector2 MarginsSize()
        {            
            return MarginInsets().Size;
        }

        public EdgeInsets MarginInsets()
        {
            var scaleFix = GuiScaleFix();
            return new EdgeInsets(Margin + (optionalTitle == null ? 0 : Margin + 25f), Margin + scaleFix.x, Margin + scaleFix.y, Margin);
        }

        /// <summary>
        /// modifier to fight gui jittering on fractional scales
        /// </summary>
        /// <returns></returns>
        public Vector2 GuiScaleFix()
        {
            return (Prefs.UIScale % 1) == 0 ? Vector2.zero : Vector2.one;
        }

        /// <summary>
        /// Inner size of the window (window size - margins)
        /// </summary>
        public virtual Vector2 InnerSize
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

        public virtual bool ForceOnGUI
        {
            get
            {
                return true;
            }
        }

        public CWindow() : this(new Vector2(100, 100)) { }

        /// <summary>
        /// creates window with excpected window size
        /// </summary>
        /// <param name="estimatedSize">excpected window size</param>
        public CWindow(Vector2 estimatedSize) : base()
        {
            Gui = new CWindowRoot();
            initSize = estimatedSize;        
        }

        /// <summary>
        /// Method called before window is shown for the first time (native RW method)
        /// </summary>
        public override void PreOpen()
        {
            timer.Restart();
            ConstructGui();


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
            $"{this.GetType().Name}: gui constructed in: {((decimal)timer.Elapsed.Ticks) / 10000000:0.0000000}".Log();

            base.PreOpen();
        }

        /// <summary>
        /// Override this method and construct window content in it
        /// </summary>
        public virtual void ConstructGui()
        {

        }

        /// <summary>
        /// Creates constraints to make window resizable
        /// </summary>
        /// <param name="vertical">make vertically resizable</param>
        /// <param name="horizontal">make horizontally resizable</param>
        public virtual void MakeResizable(bool vertical = true, bool horizontal = true)
        {
            MakeResizable(vertical, horizontal, null);
        }

        /// <summary>
        /// Creates constraints to make window resizable
        /// </summary>
        /// <param name="vertical">make vertically resizable</param>
        /// <param name="horizontal">make horizontally resizable</param>
        /// <param name="strength">strength of created constraints</param>
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

        /// <summary>
        /// draws window content (native RW method)
        /// </summary>
        /// <param name="inRect"></param>
        public override void DoWindowContents(Rect inRect)
        {
            bool debug = ShowWindowDrawingTime;
            if (debug)
            {
                timer.Restart();
            }

            var fix = GuiScaleFix();
            inRect.width -= fix.x;
            inRect.height -= fix.y;

            Gui.UpdateAndDoContent(inRect, resizer.IsResizing);

            if (debug)
            {
                GUI.EndGroup(); // escaping group to draw on window margin
                timer.Stop();

                var rect = new Rect(4, 0, 75, Margin);

                //Log.Message($"{this.GetType().Name}: gui constructed in: {((decimal)timer.Elapsed.Ticks) / 10000000:0.0000000}");
                //GuiTools.UsingColor(Color.black, () => GUI.DrawTexture(rect, BaseContent.WhiteTex));

                GuiTools.PushFont(GameFont.Tiny);
                GuiTools.PushColor(Color.green);
                Widgets.Label(rect, $"{((decimal)timer.Elapsed.Ticks) / 10000000:0.0000000}s");
                GuiTools.PopColor();
                GuiTools.PopFont();
                var margins = MarginInsets();
                GUI.BeginGroup(new Rect(margins.Left, margins.Top, inRect.width, inRect.height)); // recovering group
            }
        }

        /// <summary>
        /// screen resolution/rw window size changed notification (native RW method)
        /// </summary>
        public override void Notify_ResolutionChanged()
        {
            Gui.UpdateScreenGuide(new Vector2(UI.screenWidth, UI.screenHeight) - MarginsSize());

            //Gui.UpdateLayoutIfNeeded();

            base.Notify_ResolutionChanged();
        }

        public Vector2 TranslateToScreenCoordiates(Vector2 vector)
        {
            var margins = MarginInsets();
            return new Vector2(windowRect.xMin + margins.Left + vector.x, windowRect.yMin + margins.Top + vector.y);
        }

        public Rect TranslateToScreenCoordiates(Rect rect)
        {
            var margins = MarginInsets();
            return new Rect(windowRect.xMin + margins.Left + rect.x, windowRect.yMin + margins.Top + rect.y, rect.width, rect.height);
        }
    }
}

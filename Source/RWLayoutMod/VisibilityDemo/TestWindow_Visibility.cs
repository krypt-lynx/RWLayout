using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod.VisibilityDemo
{
    class Light : CElement
    {
        public Light(Color color) : base()
        {
            var indicator = this.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => Widgets.DrawBoxSolid(sender.Bounds, color),
            });

            this.Embed(indicator, new EdgeInsets(10));
        }
    }

    // It was intended to be element use example, but then I had through: people will use that code to implement show/hide. 
    // So, there is your show/hide in right way
    class TestWindow_Visibility : CWindow
    {
        Light red;
        Light yellow;
        Light green;

        Color borderColor = new Color(1, 1, 1, 0.3f);

        int stage = 3;

        public override void ConstructGui()
        {
            base.ConstructGui();

            doCloseX = true;
            draggable = true;

            var lightsFrame = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(borderColor, () => Widgets.DrawBox(sender.BoundsRounded)),
            });

            lightsFrame.AddConstraints(lightsFrame.width ^ 100, lightsFrame.height ^ 300);

            var switchLightsBtn = Gui.AddElement(new CButton
            {
                Title = "Next",
                Action = NextLight
            });

            Gui.StackTop(true, true, ClStrength.Strong, lightsFrame, 16, (switchLightsBtn, 40));

            red = new Light(new Color(0.9f, 0.1f, 0.1f));
            yellow = new Light(new Color(0.8f, 0.8f, 0.1f));
            green = new Light(new Color(0.1f, 0.8f, 0.1f));

            Gui.AddElements(red, yellow, green);

            lightsFrame.StackTop(true, true, ClStrength.Strong, red, (yellow, red.height), (green, red.height));
        }

        private void NextLight(CElement obj)
        {
            stage = (stage + 1) % 4;
            switch (stage)
            {
                case 0:
                    {
                        red.Hidden = false;
                        yellow.Hidden = true;
                        green.Hidden = true;
                    } break;
                case 1:
                    {
                        red.Hidden = false;
                        yellow.Hidden = false;
                        green.Hidden = true;
                    }
                    break;
                case 2:
                    {
                        red.Hidden = true;
                        yellow.Hidden = true;
                        green.Hidden = false;
                    }
                    break;
                case 3:
                    {
                        red.Hidden = true;
                        yellow.Hidden = false;
                        green.Hidden = true;
                    }
                    break;
            }
        }
    }
}

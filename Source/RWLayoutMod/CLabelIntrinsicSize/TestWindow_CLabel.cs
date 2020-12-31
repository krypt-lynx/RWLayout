using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod.CLabelIntrinsicSize
{
    class TestWindow_CLabel : CWindow
    {
        CLabel testLabel;
        ClConstraint intristicWidth = null;
        ClConstraint forcedWidth = null;
        ClConstraint intristicHeight = null;
        ClConstraint forcedHeight = null;

        public override void ConstructGui()
        {
            MakeResizable(true, true, ClStrength.Medium);
            doCloseX = true;
            draggable = true;
            InnerSize = new Vector2(400, 400);


            var panel1 = Gui.AddElement(new CElement());
            testLabel = new CLabel
            {
                //Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\nAenean tellus nisi.",
                Title = "Good news: I do know how to make intrinsic width and word wrap work together.\nBad news: Not in this release",
                //Title = "G",

                TextAlignment = TextAnchor.MiddleCenter
            };

            var wordWrap = panel1.AddElement(new CCheckboxLabeled
            {
                Title = "Word Wrap",
                Changed = (_, x) => testLabel.WordWrap = x,
                Checked = testLabel.WordWrap
            }); 
            var width = panel1.AddElement(new CCheckboxLabeled
            {
                Title = "Intrinsic width",
                Changed = (_, x) =>
                {
                    if (x)
                    {
                        testLabel.RemoveConstraint(forcedWidth);
                        testLabel.AddConstraint(intristicWidth);
                    } 
                    else
                    {
                        testLabel.RemoveConstraint(intristicWidth);
                        testLabel.AddConstraint(forcedWidth);
                    }
                }
            });
            var height = panel1.AddElement(new CCheckboxLabeled
            {
                Title = "Intrinsic height",
                Changed = (_, x) =>
                {
                    if (x)
                    {
                        testLabel.RemoveConstraint(forcedHeight);
                        testLabel.AddConstraint(intristicHeight);
                    }
                    else
                    {
                        testLabel.RemoveConstraint(intristicHeight);
                        testLabel.AddConstraint(forcedHeight);
                    }
                }
            });

            var panel2 = Gui.AddElement(new CElement());
            var downsizeFast = panel2.AddElement(new CButton
            {
                Title = "-10",
                Action = (_) =>
                {
                    var sz = InnerSize;
                    sz.x -= 10;
                    InnerSize = sz;
                }
            });
            var downsize = panel2.AddElement(new CButton
            {
                Title = "-1",
                Action = (_) =>
                {
                    var sz = InnerSize;
                    sz.x -= 1;
                    InnerSize = sz;
                }
            });
            var fitWidth = panel2.AddElement(new CButton
            {
                Title = "fit width",
                Action = (_) =>
                {
                    var sz = InnerSize;
                    sz.x = (float)testLabel.intrinsicWidth.Value; 
                    InnerSize = sz;
                }
            });
            var upsize = panel2.AddElement(new CButton
            {
                Title = "+1",
                Action = (_) =>
                {
                    var sz = InnerSize;
                    sz.x += 1;
                    InnerSize = sz;
                }
            });
            var upsizeFast = panel2.AddElement(new CButton
            {
                Title = "+10",
                Action = (_) =>
                {
                    var sz = InnerSize;
                    sz.x += 10;
                    InnerSize = sz;
                }
            });

            var guide = new CVarListGuide();
            var pan1Grid = new ClVariable("pan1Grid");
            var pan2Grid = new ClVariable("pan2Grid");
            guide.Variables.Add(pan1Grid);
            guide.Variables.Add(pan2Grid);
            Gui.AddGuide(guide);

            panel1.StackLeft((wordWrap, pan1Grid), (width, pan1Grid), (height, pan1Grid));
            panel2.StackLeft((downsizeFast, pan2Grid), (downsize, pan2Grid), (fitWidth, pan2Grid * 2), (upsize, pan2Grid), (upsizeFast, pan2Grid));

            Gui.AddConstraint(Gui.height >= panel2.bottom);

            var testArea = Gui.AddElement(new CElement());
            Gui.StackTop((panel1, 25), (panel2, 25), testArea);

            var labelFrame = testArea.AddElement(new CFrame
            {
                Color = Color.green
            });



            testArea.AddElement(testLabel);
            testArea.AddConstraints(
                testLabel.centerX ^ testArea.centerX,
                testLabel.centerY ^ testArea.centerY,
                testLabel.left >= testArea.left,
                testLabel.top >= testArea.top
                );

            testLabel.Embed(labelFrame);
                      

            intristicWidth = testLabel.width ^ testLabel.intrinsicWidth;
            intristicHeight = testLabel.height ^ testLabel.intrinsicHeight;
            forcedWidth = testLabel.width ^ testArea.width;
            forcedHeight = testLabel.height ^ testArea.height;

            intristicWidth.SetStrength(ClStrength.Weak);
            forcedWidth.SetStrength(ClStrength.Weak);
            intristicHeight.SetStrength(ClStrength.Weak);
            forcedHeight.SetStrength(ClStrength.Weak);

            testArea.AddConstraints(forcedWidth, forcedHeight);



            // Good news: I do know how to make intrinsic width and word wrap work together.
            // Bad news: Not in this release
            // But there is your hack if you really need it 

            /*
            wordWrap.Changed(wordWrap, wordWrap.Checked = false);
            width.Changed(width, width.Checked = true);
            height.Changed(height, height.Checked = true);
            testLabel.Hidden = true;
            labelFrame.Hidden = true;
            var testLabel2 = testArea.AddElement(new CLabel
            {
                Title = testLabel.Title,
                TextAlignment = TextAnchor.MiddleCenter,
                WordWrap = true,
            });
            var labelFrame2 = testArea.AddElement(new CFrame
            {
                Color = Color.red
            });
            testLabel2.Embed(labelFrame2);

            testLabel2.AddConstraints(
                testLabel2.left ^ testLabel.left,
                testLabel2.right ^ testLabel.right,
                testLabel2.centerY ^ testLabel.centerY,
                testLabel2.height ^ testLabel2.intrinsicHeight
                );
            */
        }

    }
}

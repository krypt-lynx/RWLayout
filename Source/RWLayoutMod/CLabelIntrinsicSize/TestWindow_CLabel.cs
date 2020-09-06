using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;

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
            MakeResizable(true, true, ClStrength.Strong);
            doCloseX = true;
            draggable = true;
            InnerSize = new Vector2(400, 400);


            var panel = Gui.AddElement(new CElement());
            testLabel = new CLabel
            {
                Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\nAenean tellus nisi."
            };

            var wordWrap = panel.AddElement(new CCheckBox
            {
                Title = "Word Wrap",
                Changed = (_, x) => testLabel.WordWrap = x,
                Checked = testLabel.WordWrap
            }); 
            var width = panel.AddElement(new CCheckBox
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
            var height = panel.AddElement(new CCheckBox
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

            panel.StackLeft(true, true, ClStrength.Strong, wordWrap, (width, wordWrap.width), (height, wordWrap.width));

            var testArea = Gui.AddElement(new CElement());
            Gui.StackTop(true, true, ClStrength.Strong, (panel, 25), testArea);

            testArea.AddElement(testLabel);
            testArea.AddConstraints(
                testLabel.centerX ^ testArea.centerX,
                testLabel.centerY ^ testArea.centerY,
                testLabel.left >= testArea.left,
                testLabel.top >= testArea.top
                );


            intristicWidth = testLabel.width ^ testLabel.intrinsicWidth;
            forcedWidth = testLabel.height ^ testLabel.intrinsicHeight;
            intristicHeight = testLabel.left ^ testArea.left;
            forcedHeight = testLabel.top ^ testArea.top;

            intristicWidth.SetStrength(ClStrength.Medium);
            forcedWidth.SetStrength(ClStrength.Medium);
            intristicHeight.SetStrength(ClStrength.Medium);
            forcedHeight.SetStrength(ClStrength.Medium);

            testArea.AddConstraints(forcedWidth, forcedHeight);
        }

    }
}

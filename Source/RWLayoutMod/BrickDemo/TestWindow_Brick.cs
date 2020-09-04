using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod.BrickDemo
{
    class TestWindow_Brick : CWindow
    {
        enum TetrisState
        {
            Interactive,
            CurtainUp,
            CurtainDown,
        }

        CElement bottle;
        Figure previewNext;
        Curtain curtain;
        CLabel debugInfoLabel;

        ClVariable grid = null;
        List<Figure> figures = new List<Figure>();
        Color borderColor = new Color(1, 1, 1, 0.3f);
        public override void ConstructGui()
        {
            base.ConstructGui();

            // Window 
            optionalTitle = "Poor Man's Tetris";
            doCloseX = true;
            draggable = true;
            MakeResizable(horizontal: true);

            Gui.AddConstraint(Gui.height >= 200);
            Gui.AddConstraint(Gui.height <= Gui.adjustedScreenHeight);

            grid = new ClVariable("grid");
            var guide = new CVarListGuide();
            guide.Anchors.Add(new Anchor { var = grid });
            Gui.AddGuide(guide);

            // 12 x 20 bottle
            var bottleFrame = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(borderColor, () => Widgets.DrawBox(sender.BoundsRounded)),
                Name = "Bottle",
            });


            Gui.AddConstraint(Gui.left ^ bottleFrame.left);
            Gui.AddConstraint(Gui.top ^ bottleFrame.top);
            Gui.AddConstraint(Gui.bottom ^ bottleFrame.bottom);

            Gui.AddConstraint(bottleFrame.width ^ 12 * grid);
            Gui.AddConstraint(bottleFrame.height ^ 20 * grid);

            bottle = bottleFrame.AddElement(new CElement());
            bottleFrame.Embed(bottle);

            curtain = bottleFrame.AddElement(new Curtain());
            bottleFrame.Embed(curtain);
            curtain.Hidden = true;


            // 4 x 4 preview

            var previewFrame = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(borderColor, () => Widgets.DrawBox(sender.BoundsRounded)),
                Name = "Preview",
            });


            Gui.AddConstraint(previewFrame.left ^ bottleFrame.right + grid);
            Gui.AddConstraint(previewFrame.top ^ Gui.top);
            Gui.AddConstraint(previewFrame.right ^ Gui.right);
            Gui.AddConstraint(previewFrame.width ^ previewFrame.height);
            Gui.AddConstraint(previewFrame.width ^ 4 * grid);


            previewNext = previewFrame.AddElement(new Figure(((uint)Rand.Int) % 7, true));
            previewFrame.Embed(previewNext);

            // add figure button

            var btnAdd = Gui.AddElement(new CButton
            {
                Title = "Throw",
                Action = AddFigure,
            });

            Gui.AddConstraint(btnAdd.left ^ previewFrame.left);
            Gui.AddConstraint(btnAdd.right ^ previewFrame.right);
            Gui.AddConstraint(btnAdd.top ^ previewFrame.bottom + grid);
            Gui.AddConstraint(btnAdd.height ^ 2 * grid);

            // remove figure button
            var btnRemove = Gui.AddElement(new CButton {
                Title = "Destroy",
                Action = RemoveFigure,
            });

            Gui.AddConstraint(btnRemove.left ^ previewFrame.left);
            Gui.AddConstraint(btnRemove.right ^ previewFrame.right);
            Gui.AddConstraint(btnRemove.top ^ btnAdd.bottom + grid);
            Gui.AddConstraint(btnRemove.height ^ 2 * grid);

            var btnTest = Gui.AddElement(new CButton
            {
                Title = "Stress test",
                Action = RepeatTest,
            });

            Gui.AddConstraint(btnTest.left ^ previewFrame.left);
            Gui.AddConstraint(btnTest.right ^ previewFrame.right);
            Gui.AddConstraint(btnTest.top ^ btnRemove.bottom + grid);
            Gui.AddConstraint(btnTest.height ^ 2 * grid);

            debugInfoLabel = Gui.AddElement(new CLabel
            {
                Font = GameFont.Tiny,
            });

            Gui.AddConstraint(debugInfoLabel.left ^ previewFrame.left);
            Gui.AddConstraint(debugInfoLabel.right ^ previewFrame.right);
            Gui.AddConstraint(debugInfoLabel.top ^ btnTest.bottom + grid);


            var logCnsBtn = Gui.AddElement(new CButton
            {
                Title = "log cns",
                Action = (sender) =>
                {
                    Log.Warning("constraints:\n" + string.Join("\n", sender.AllConstraintsDebug().Select(c => c.ToString())));
                },
            });


            Gui.AddConstraint(debugInfoLabel.bottom + grid ^ logCnsBtn.top);
            Gui.AddConstraint(logCnsBtn.left ^ previewFrame.left);
            Gui.AddConstraint(logCnsBtn.right ^ previewFrame.right);
            Gui.AddConstraint(logCnsBtn.height ^ grid);

            var logVarsBtn = Gui.AddElement(new CButton
            {
                Title = "log vars",
                Action = (sender) =>
                {
                    Log.Warning("variables:\n" + string.Join("\n", sender.AllVariablesDebug().Select(c => c.ToString())));
                },
            });


            Gui.AddConstraint(logCnsBtn.bottom ^ logVarsBtn.top);
            Gui.AddConstraint(logVarsBtn.left ^ previewFrame.left);
            Gui.AddConstraint(logVarsBtn.right ^ previewFrame.right);
            Gui.AddConstraint(logVarsBtn.height ^ grid);
            Gui.AddConstraint(logVarsBtn.bottom ^ Gui.bottom);


            InnerSize = new Vector2(400, 400);
        }


        void AddFigure(CElement sender)
        {
            if (state != TetrisState.Interactive)
            {
                return;
            }

            var index = ((uint)Rand.Int) % 7;
            var addingIndex = previewNext.FigureIndex;
            previewNext.FigureIndex = index;

            var figure = bottle.AddElement(new Figure(addingIndex));

            bottle.AddConstraint(figure.left ^ bottle.left);
            bottle.AddConstraint(figure.right ^ bottle.right);
            bottle.AddConstraint(figure.bottom ^ (figures.LastOrDefault()?.top ?? bottle.bottom));
            bottle.AddConstraint(figure.height ^ figure.LinesCount() * grid);

            figures.Add(figure);

            Gui.UpdateLayoutIfNeeded();

            if (figures.Count() > 0 && figures.Last().top.Value <= bottle.top.Value + 1)
            {
                ShowCurtain();
            }
        }

        int testCounter = 0;
        void RepeatTest(CElement sender)
        {
            testCounter = 1000;
        }


        TetrisState state = TetrisState.Interactive;
        int animationStep = 0;

        private void RemoveFigure(CElement obj)
        {
            if (state != TetrisState.Interactive)
            {
                return;
            }

            if (figures.Count() == 0)
            {
                return;
            }

            int indexToRemove = (int)(((uint)Rand.Int) % figures.Count());
            bottle.RemoveElement(figures[indexToRemove]);
            figures.RemoveAt(indexToRemove);
            if (indexToRemove < figures.Count())
            {
                var bottom = indexToRemove == 0 ? bottle.bottom : figures[indexToRemove - 1].top;
                bottle.AddConstraint(figures[indexToRemove].bottom ^ bottom);
            }

            Gui.UpdateLayoutIfNeeded();
        }

        void ClearBottle()
        {
            bottle.RemoveAllElements();
            figures.Clear();
        }

        void ShowCurtain()
        {
            state = TetrisState.CurtainUp;

            curtain.Hidden = false;
        }

        void HideCurtain()
        {
            state = TetrisState.Interactive;

            curtain.Hidden = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            DoTest();

            DoAnimations();

            DoDebug();

            base.DoWindowContents(inRect);
        }

        private void DoDebug()
        {

            debugInfoLabel.Title =
                $"Constrants: {Gui.AllConstraintsDebug().Length}\nVariables: {Gui.AllVariablesDebug().Count()}\nTest counter: {testCounter}";

            Dictionary<string, int> varCounts = new Dictionary<string, int>();

            foreach (var variable in Gui.AllVariablesDebug())
            {
                if (!varCounts.ContainsKey(variable.Name))
                {
                    varCounts[variable.Name] = 1;
                } 
                else
                {
                    varCounts[variable.Name] = varCounts[variable.Name] + 1;
                }
            }

            var dups = varCounts.Where(kvp => kvp.Value > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (dups.Count > 0)
            {
                Log.Warning("duplicated variables in solver:\n" + string.Join(", ", dups.Select(kvp => $"{{{kvp.Key}:{kvp.Value}}}")));
            }
            
        }

        private void DoTest()
        {
            if (state == TetrisState.Interactive && testCounter > 0)
            {
                AddFigure(null);
                testCounter--;
            }
        }

        int tick = 0;
        private void DoAnimations()
        {
            if (tick > 1)
            {
                switch (state)
                {
                    case TetrisState.CurtainUp:
                        {
                            animationStep++;
                            curtain.Lines = animationStep;
                            if (animationStep >= 20 || testCounter > 0)
                            {
                                state = TetrisState.CurtainDown;
                                ClearBottle();
                            }
                        }
                        break;
                    case TetrisState.CurtainDown:
                        {
                            animationStep--;
                            curtain.Lines = animationStep;
                            if (animationStep <= 0)
                            {
                                state = TetrisState.Interactive;
                                HideCurtain();
                            }
                        }
                        break;
                }
                tick = 0;
            }
            tick++;
        }
    }
}










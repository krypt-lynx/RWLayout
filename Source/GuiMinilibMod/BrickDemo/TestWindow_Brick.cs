using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout;
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
            resizeable = true;
            Gui.Solver.AddConstraint(Gui.height >= 200);
            Gui.Solver.AddConstraint(Gui.height <= adjustedScreenHeight);
            Gui.Solver.AddConstraint(ClStrength.Weak, Gui.width ^ guideWidth);


            // 12 x 20 bottle
            var bottleFrame = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(borderColor, () => Widgets.DrawBox(sender.boundsRounded))
            });

            grid = new ClVariable("grid");

            Gui.Solver.AddConstraint(Gui.left ^ bottleFrame.left);
            Gui.Solver.AddConstraint(Gui.top ^ bottleFrame.top);
            Gui.Solver.AddConstraint(Gui.bottom ^ bottleFrame.bottom);

            Gui.Solver.AddConstraint(bottleFrame.width ^ 12 * grid);
            Gui.Solver.AddConstraint(bottleFrame.height ^ 20 * grid);

            bottle = bottleFrame.AddElement(new CElement());
            bottleFrame.Embed(bottle);

            curtain = bottleFrame.AddElement(new Curtain());
            bottleFrame.Embed(curtain);
            curtain.Hidden = true;


            // 4 x 4 preview

            var previewFrame = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(borderColor, () => Widgets.DrawBox(sender.boundsRounded))
            });


            Gui.Solver.AddConstraint(previewFrame.left ^ bottleFrame.right + grid);
            Gui.Solver.AddConstraint(previewFrame.top ^ Gui.top);
            Gui.Solver.AddConstraint(previewFrame.right ^ Gui.right);
            Gui.Solver.AddConstraint(previewFrame.width ^ previewFrame.height);
            Gui.Solver.AddConstraint(previewFrame.width ^ 4 * grid);


            previewNext = previewFrame.AddElement(new Figure(((uint)Rand.Int) % 7, true));
            previewFrame.Embed(previewNext);

            // add figure button

            var btnAdd = Gui.AddElement(new CButton
            {
                Title = "Throw",
                Action = AddFigure,
            });

            Gui.Solver.AddConstraint(btnAdd.left ^ previewFrame.left);
            Gui.Solver.AddConstraint(btnAdd.right ^ previewFrame.right);
            Gui.Solver.AddConstraint(btnAdd.top ^ previewFrame.bottom + grid);
            Gui.Solver.AddConstraint(btnAdd.height ^ 2 * grid);

            // remove figure button
            var btnRemove = Gui.AddElement(new CButton {
               Title = "Destroy",
               Action = RemoveFigure,
            });

            Gui.Solver.AddConstraint(btnRemove.left ^ previewFrame.left);
            Gui.Solver.AddConstraint(btnRemove.right ^ previewFrame.right);
            Gui.Solver.AddConstraint(btnRemove.top ^ btnAdd.bottom + grid);
            Gui.Solver.AddConstraint(btnRemove.height ^ 2 * grid);



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

            bottle.Solver.AddConstraint(figure.left ^ bottle.left);
            bottle.Solver.AddConstraint(figure.right ^ bottle.right);
            bottle.Solver.AddConstraint(figure.bottom ^ (figures.LastOrDefault()?.top ?? bottle.bottom));
            bottle.Solver.AddConstraint(figure.height ^ figure.LinesCount() * grid);

            figures.Add(figure);

            figure.UpdateLayoutConstraints(Gui.Solver);
            Gui.UpdateLayout();
            Gui.PostLayoutUpdate();

            if (figures.Count() > 0 && figures.Last().top.Value <= bottle.top.Value + 1)
            {
                ShowCurtain();
            }
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
                bottle.Solver.AddConstraint(figures[indexToRemove].bottom ^ bottom);
            }

            Gui.UpdateLayout();
            Gui.PostLayoutUpdate();
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

        int tick = 0;

        public override void DoWindowContents(Rect inRect)
        {
            if (tick > 1)
            {
                switch (state)
                {
                    case TetrisState.CurtainUp:
                        {
                            animationStep++;
                            curtain.Lines = animationStep;
                            if (animationStep >= 20)
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

            base.DoWindowContents(inRect);

        }
    }
}










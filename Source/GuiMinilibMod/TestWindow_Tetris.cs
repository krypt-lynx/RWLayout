using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using GuiMinilib;
using UnityEngine;
using Verse;

namespace GuiMinilibMod
{
    public class Figure : CElement
    {
        public static string[][] figures = {
            new string[] {
                "- x ",
                "  xx",
                "  x ",
                },
            new string[] {
                "  x ",
                "xxx ",
                },
            new string[] {
                "-xx ",
                "  xx",
                },
            new string[] {
                " xx ",
                " xx ",
                },
            new string[] {
                "- xx",
                " xx ",
                },
            new string[] {
                "  x ",
                "  x ",
                " xx ",
                },
            new string[] {
                "xxxx",
                },

        };

        public static Color[] colors = {
            new Color(0.5f,    0,       0.75f),
            new Color(0.75f,   0.5f,    0),
            new Color(1,       0.1f,    0.1f),
            new Color(0.90f,   0.90f,   0),
            new Color(0,       0.80f,   0),
            new Color(0.3f,    0.3f,    1),
            new Color(0,       0.80f,   0.8f),
        };

        float size = 0;

        private string[] shape;
        private Color color;
        private bool isPreview;

        public int LinesCount()
        {
            return shape.Length;
        }

        public Figure(uint figureIndex, bool preview = false) : base()
        {
            FigureIndex = figureIndex;
            isPreview = preview;
        }

        private uint figureIndex = 0;
        public uint FigureIndex
        {
            set
            {
                figureIndex = value;
                shape = figures[figureIndex];
                color = colors[figureIndex];
                tex = SolidColorMaterials.NewSolidColorTexture(color);
            }
            get
            {
                return figureIndex;
            }
        }

        public override void PostAdd()
        {
            base.PostAdd();

            size = (float?)(Solver.GetVariable("grid") as ClVariable)?.Value ?? 10;
        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            size = (float?)(Solver.GetVariable("grid") as ClVariable)?.Value ?? 10;
        }

        public Texture2D tex;

        public override void DoContent()
        {
            base.DoContent();
            GUI.BeginGroup(boundsRounded);
            if (!isPreview)
            {
                Log.Message($"Figure: {bounds}");
            }

            int offsetX;
            int offsetY;

            if (isPreview)
            {
                offsetX = 0;
                offsetY = shape.Length <= 2 ? 1 : 0;
            }
            else
            {
                offsetX = ((int)Math.Round(bounds.width / size) - 4) / 2;
                offsetY = 0;
            }

            for (int y = 0; y < shape.Length; y++)
            {
                string line = shape[y];
                for (int x = 0; x < line.Length; x++)
                {
                    if (isPreview && line[x] == '-')
                    {
                        offsetX--;
                    }
                    if (line[x] == 'x')
                    {
                        var rect = new Rect((x + offsetX) * size, (y + offsetY) * size, size, size).Rounded2();
                        GUI.DrawTexture(rect, tex);
                        
                        GuiTools.UsingColor(Color.gray, () => GuiTools.Box(rect, new EdgeInsets(0, 2, 2, 0)));
                        GuiTools.UsingColor(Color.white, () => GuiTools.Box(rect, new EdgeInsets(2, 0, 0, 2)));
                    }
                }
            }
            GUI.EndGroup();
        }
    }


    class TestWindow_Tetris : CWindow
    {
        CElement bottle;
        Figure previewNext;

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
            bottle = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(borderColor, () => Widgets.DrawBox(sender.boundsRounded))
            });

            grid = new ClVariable("grid");

            Gui.Solver.AddConstraint(Gui.left ^ bottle.left);
            Gui.Solver.AddConstraint(Gui.top ^ bottle.top);
            Gui.Solver.AddConstraint(Gui.bottom ^ bottle.bottom);

            Gui.Solver.AddConstraint(bottle.width ^ 12 * grid);
            Gui.Solver.AddConstraint(bottle.height ^ 20 * grid);

            


            // 4 x 4 preview

            var previewFrame = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(borderColor, () => Widgets.DrawBox(sender.boundsRounded))
            });


            Gui.Solver.AddConstraint(previewFrame.left ^ bottle.right + grid);
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
            

           // AddFigure(null);

            InnerSize = new Vector2(401, 401);
        }


        void AddFigure(CElement sender)
        {
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
        }

        private void RemoveFigure(CElement obj)
        {
            //Gui.solver.re
            throw new NotImplementedException();
        }

        public override void DoWindowContents(Rect inRect)
        {
            base.DoWindowContents(inRect);

        }
    }
}










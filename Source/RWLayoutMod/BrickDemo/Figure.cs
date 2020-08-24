using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.Alpha1;
using UnityEngine;
using Verse;

namespace RWLayoutMod.BrickDemo
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

        public override void DoContent()
        {
            base.DoContent();
            GUI.BeginGroup(boundsRounded);

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
                        Widgets.DrawBoxSolid(rect, color);
                        GuiTools.UsingColor(Color.gray, () => GuiTools.Box(rect, new EdgeInsets(0, 2, 2, 0)));
                        GuiTools.UsingColor(Color.white, () => GuiTools.Box(rect, new EdgeInsets(2, 0, 0, 2)));
                    }
                }
            }
            GUI.EndGroup();
        }
    }

}

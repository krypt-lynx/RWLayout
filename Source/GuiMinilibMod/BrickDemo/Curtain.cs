using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary_moddiff;
using RWLayout_moddiff;
using UnityEngine;
using Verse;

namespace RWLayoutMod.BrickDemo
{

    public class Curtain : CElement
    {
        float size = 0;

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

        static Color color = new Color(0.75f, 0.75f, 0.75f);

        public override void DoContent()
        {
            base.DoContent();
            GUI.BeginGroup(boundsRounded);

            int maxX = (int)Math.Round(bounds.width / size);
            int maxY = (int)Math.Round(bounds.height / size);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = maxY - Lines; y < maxY; y++)
                {
                    var rect = new Rect(x * size, y * size, size, size).Rounded2();
                    Widgets.DrawBoxSolid(rect, color);
                    GuiTools.UsingColor(Color.gray, () => GuiTools.Box(rect, new EdgeInsets(0, 2, 2, 0)));
                    GuiTools.UsingColor(Color.white, () => GuiTools.Box(rect, new EdgeInsets(2, 0, 0, 2)));
                }
            }
            GUI.EndGroup();
        }

        public int Lines { get; internal set; }
    }
}

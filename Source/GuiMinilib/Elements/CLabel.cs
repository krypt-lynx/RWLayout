// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout
{
    public class CLabel : CElement
    {
        public string Title;
        public GameFont Font = GameFont.Small;
        public Color? Color = null;
        public bool Multiline = false;

        public override Vector2 tryFit(Vector2 size)
        {
            Vector2 result;
            if (Multiline)
            {
                var y = GuiTools.UsingFont(Font, () => Text.CalcHeight(Title, size.x));
                result = new Vector2(size.x, y);
            }
            else
            {
                result = GuiTools.UsingFont(Font, () => Text.CalcSize(Title));
            }

            //Log.Message($"fitting size of {NamePrefix()} (\"{Title}\"): {result} for {size}");
            return result;
        }

        public override void DoContent()
        {
            base.DoContent();
            GuiTools.UsingFont(Font, () =>
            {
                GuiTools.UsingColor(Color, () => Widgets.Label(bounds, Title));
            });
        }
    }
}

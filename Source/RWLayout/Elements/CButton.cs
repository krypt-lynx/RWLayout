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
    public class CButton : CElement
    {
        public TaggedString Title { get; set; }
        public Action<CElement> Action { get; set; }
        public GameFont Font = GameFont.Small;

        public override Vector2 tryFit(Vector2 size)
        {
            var result = GuiTools.UsingFont(Font, () => Text.CalcSize(Title));

            return result + new Vector2(14, 10);
        }

        public override void DoContent()
        {           
            base.DoContent();
            GuiTools.UsingFont(Font, () =>
            {
                if (Widgets.ButtonText(bounds, Title, doMouseoverSound: true))
                {
                    this.Action?.Invoke(this);
                }
            });
        }
    }
}

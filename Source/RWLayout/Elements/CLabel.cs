// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public class CLabel : CTitledElement
    {

        static GUIContent contentForTesting = new GUIContent();
        public override Vector2 tryFit(Vector2 size)
        {
            ApplyGeometryOnly();
            contentForTesting.text = Title;
            var result = GuiTools.UsingFont(Font, () => Text.CurFontStyle.CalcSize(contentForTesting));
            RestoreGeometryOnly();
            return result;
        }

        public override void DoContent()
        {
            base.DoContent();

            ApplyAll();

            Widgets.Label(Bounds, Title);

            RestoreAll();
        }
    }
}

﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
    /// <summary>
    /// Widgets.Label wrapper 
    /// </summary>
    public class CLabel : CTitledElement
    {
        public override Vector2 tryFit(Vector2 size)
        {
            return tryFitText(size, Vector2.zero);
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

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
    public class CButton : CTitledElement
    {
        public Action<CElement> Action { get; set; }

        public override Vector2 tryFit(Vector2 size)
        {
            return tryFitText(size, new Vector2(14, 10));
        }

        public override void DoContent()
        {           
            base.DoContent();
            ApplyAll();
            if (Widgets.ButtonText(Bounds, Title, doMouseoverSound: true))
            {
                this.Action?.Invoke(this);
            }
            RestoreAll();
        }
    }
}

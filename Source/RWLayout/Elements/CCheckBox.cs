﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public class CCheckBox : CElement
    {
        public string Title;
        public bool Checked = false;
        public bool Disabled = false;

        public Action<CCheckBox> Changed = null;

        public override Vector2 tryFit(Vector2 size)
        {
            return base.tryFit(size);
        }

        public override void DoContent()
        {
            base.DoContent();
            bool oldChecked = Checked;
            GuiTools.UsingFont(GameFont.Small, () =>
            {
                Widgets.CheckboxLabeled(bounds, Title, ref Checked);
            });
            if (oldChecked != Checked)
            {
                Changed?.Invoke(this);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    public class CCheckBox : CTitledElement
    {
        public bool Checked = false;
        public bool Disabled = false;

        public Action<CCheckBox> Changed = null;

        public override Vector2 tryFit(Vector2 size)
        {            
            return tryFitText(size, new Vector2(10 + 24, 0));
        }

        public override void DoContent()
        {
            base.DoContent();
            bool oldChecked = Checked;

            ApplyAll();
            Widgets.CheckboxLabeled(Bounds, Title, ref Checked);
            RestoreAll();
            if (oldChecked != Checked)
            {
                Changed?.Invoke(this);
            }
        }
    }
}

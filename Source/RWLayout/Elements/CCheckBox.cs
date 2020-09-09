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
    /// Widgets.CheckboxLabeled wrapper
    /// </summary>
    public class CCheckBox : CTitledElement
    {
        /// <summary>
        /// Is checkbox checked?
        /// </summary>
        public bool Checked = false;

        /// <summary>
        /// Called on Checked state change
        /// </summary>
        /// <remarks>first argument is wrapper itself, second argument is Checked state</remarks>
        public Action<CCheckBox, bool> Changed = null;

        public override Vector2 tryFit(Vector2 size)
        {            
            return tryFitText(size, new Vector2(10 + 24, 0));
        }

        public override void DoContent()
        {
            base.DoContent();
            bool oldChecked = Checked;

            ApplyAll();
            Widgets.CheckboxLabeled(BoundsRounded, Title, ref Checked);
            RestoreAll();
            if (oldChecked != Checked)
            {
                Changed?.Invoke(this, Checked);
            }
        }
    }
}

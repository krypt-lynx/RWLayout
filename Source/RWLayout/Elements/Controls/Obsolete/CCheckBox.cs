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
    /// 
    [Obsolete("Use CCheckboxLabeled instead")]
    public class CCheckBox : CCheckboxLabeled
    {
        /// <summary>
        /// Called on Checked state change
        /// </summary>
        /// <remarks>first argument is wrapper itself, second argument is Checked state</remarks>
        new public Action<CCheckBox, bool> Changed = null;

        public CCheckBox()
        {
            base.Changed = (sender, value) =>
            {
                Changed((CCheckBox)sender, value);
            };
        }
    }
}

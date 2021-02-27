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
    /// Widgets.ButtonText wrapper
    /// </summary>
    public class CButtonInvisible : CElement
    {
        /// <summary>
        /// Called then button is pressed
        /// </summary>
        public Action<CElement> Action { get; set; }

        public bool MouseoverSound { get; set; } = true;

        public override void DoContent()
        {
            base.DoContent();


            if (Widgets.ButtonInvisible(BoundsRounded, doMouseoverSound: MouseoverSound))
            {
                this.Action?.Invoke(this);
            }
        }
    }
}

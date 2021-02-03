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
    public class CCheckboxLabeled : CTitledElement
    {
        /// <summary>
        /// Is checkbox checked?
        /// </summary>
        public bool Checked = false;

        public readonly Bindable<bool> CheckedProp = new Bindable<bool>();

        /// <summary>
        /// Called on Checked state change
        /// </summary>
        /// <remarks>first argument is wrapper itself, second argument is Checked state</remarks>
        public Action<CCheckboxLabeled, bool> Changed { get; set; }


        public bool Disabled { get; set; } = false;
        public Texture2D TextureChecked { get; set; } = null;
        public Texture2D TextureUnchecked { get; set; } = null;
        public bool PlaceCheckboxNearText { get; set; } = false;


        public override Vector2 tryFit(Vector2 size)
        {
            return tryFitText(size, new Vector2(10 + 24, 0));
        }

        public override void DoContent()
        {
            base.DoContent();
            CheckedProp.SynchronizeFrom();
            bool oldChecked = Checked;
            ApplyAll();

            Widgets.CheckboxLabeled(BoundsRounded, Title, ref Checked, Disabled, TextureChecked, TextureUnchecked, PlaceCheckboxNearText);
            RestoreAll();

            if (oldChecked != Checked)
            {
                CheckedProp.SynchronizeTo();
                Changed?.Invoke(this, Checked);
            }
        }


    }
}

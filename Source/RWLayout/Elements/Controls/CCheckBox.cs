using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2.Elements.Controls
{
    [Obsolete("For binary compatimility. Use RWLayout.alpha2.CCheckbox instead")]
    public class CCheckbox : RWLayout.alpha2.CCheckbox
    {
        public CCheckbox() : base()
        {
            ((RWLayout.alpha2.CCheckbox)this).Changed = (sender, value) =>
            {
                this.Changed?.Invoke((CCheckbox)sender, value);
            };
        }

        new public Action<CCheckbox, bool> Changed = null;
    } 
}

namespace RWLayout.alpha2
{
    /// <summary>
    /// Widgets.Checkbox wrapper
    /// </summary>
    public class CCheckbox : CTintedElement
    {
        /// <summary>
        /// Is checkbox checked?
        /// </summary>
        public bool Checked = false;

        /// <summary>
        /// Called on Checked state change
        /// </summary>
        /// <remarks>first argument is wrapper itself, second argument is Checked state</remarks>
        public Action<CCheckbox, bool> Changed = null;

        public bool Disabled { get; set; } = false;
        public bool Paintable { get; set; } = false;
        public Texture2D TextureChecked { get; set; } = null;
        public Texture2D TextureUnchecked { get; set; } = null;
        public bool PlaceCheckboxNearText { get; set; } = false;

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(24, 24);
        }

        public override void DoContent()
        {
            base.DoContent();
            bool oldChecked = Checked;

            ApplyAll();
            Widgets.Checkbox(BoundsRounded.min, ref Checked, Mathf.Min(BoundsRounded.width, BoundsRounded.height), Disabled, Paintable, TextureChecked, TextureUnchecked);
            RestoreAll();

            if (oldChecked != Checked)
            {
                Changed?.Invoke(this, Checked);
            }
        }


    }
}

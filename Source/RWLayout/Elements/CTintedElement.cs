using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RWLayout.alpha2
{
    public class CTintedElement : CElement
    {
        /// <summary>
        /// tint color
        /// </summary>
        public Color Color { get; set; } = UnityEngine.Color.white;

        protected virtual void ApplyAll()
        {
            GuiTools.PushColor(Color);
        }

        protected virtual void RestoreAll()
        {
            GuiTools.PopColor();
        }

    }
}

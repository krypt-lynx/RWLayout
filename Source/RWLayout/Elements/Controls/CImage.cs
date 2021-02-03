using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Statuc Image
    /// </summary>
    /// <remarks>Untested. Added at last moment because it was missing</remarks>
    public class CImage : CElement
    {
        /// <summary>
        /// Texture to draw
        /// </summary>
        public Texture2D Texture { get; set; } = null;
        /// <summary>
        /// Texture scaling mode
        /// </summary>
        public ScaleMode ScaleMode { get; set; } = ScaleMode.StretchToFill;

        /// <summary>
        /// Enable aphla blending 
        /// </summary>
        /// <remarks>Better to disable if possible</remarks>
        public bool AlphaBlend { get; set; } = true;
        /// <summary>
        /// Texture side ratio
        /// </summary>
        public float ImageAspect { get; set; } = 0f;
        /// <summary>
        /// Texture tint color
        /// </summary>
        /// <remarks>does not follow GUI.color die to static nature</remarks>
        public Color TintColor { get; set; } = Color.white;

        /// <summary>
        /// The width of the borders. If EdgeInsets.zero, the full texture is drawn.
        /// </summary>
        public EdgeInsets Borders
        {
            get => new EdgeInsets(borderWidths);
            set => borderWidths = value.AsVector4();
        }

        /// <summary>
        /// The widths of morders (left, top, right and bottom)
        /// </summary>
        Vector4 borderWidths { get; set; } = Vector4.zero; // (left, top, right and bottom)
        /// <summary>
        /// The radiuses for rounded corners (top-left, top-right, bottom-right and bottom-left). If Vector4.zero, corners will not be rounded.
        /// </summary>
        public Vector4 BorderRadiuses { get; set; } = Vector4.zero; // (top-left, top-right, bottom-right and bottom-left)

        public override Vector2 tryFit(Vector2 size)
        {
            return Texture?.Size() ?? Vector2.zero;
        }

        public override void DoContent()
        {
            base.DoContent();
            if (Texture != null)
            {
                GuiTools.PushColor(TintColor);
                GUI.DrawTexture(BoundsRounded, Texture, ScaleMode, AlphaBlend, ImageAspect, TintColor, borderWidths, BorderRadiuses);
                GuiTools.PopColor();
            }
        }
    }
}

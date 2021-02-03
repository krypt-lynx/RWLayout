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
    public class CButtonImage : CElement
    {
        /// <summary>
        /// Texture
        /// </summary>
        public Texture2D Texture { get; set; } = null;
        /// <summary>
        /// Texture tint color
        /// </summary>
        /// <remarks>does not follow GUI.color die to static nature</remarks>
        public Color TintColor { get; set; } = Color.white;
        /// <summary>
        /// Texture mouse over color
        /// </summary>
        /// <remarks>does not follow GUI.color die to static nature</remarks>
        public Color MouseOverColor { get; set; } = Color.white;

        public Vector2 tryFitTex(Vector2 size, Vector2 margin)
        {
            // ApplyGeometryOnly();
            var result = (Texture?.Size() ?? Vector2.zero) + margin;
            // RestoreGeometryOnly();

            return result;
        }

        /// <summary>
        /// Called then button is pressed
        /// </summary>
        public Action<CElement> Action { get; set; }

        public bool MouseoverSound { get; set; } = true;

        public override Vector2 tryFit(Vector2 size)
        {
            return tryFitTex(size, new Vector2(16, 10));
        }

        public override void DoContent()
        {
            base.DoContent();

            if (Widgets.ButtonImage(BoundsRounded, Texture, TintColor, MouseOverColor, doMouseoverSound: MouseoverSound))
            {
                this.Action?.Invoke(this);
            }
        }
    }
}

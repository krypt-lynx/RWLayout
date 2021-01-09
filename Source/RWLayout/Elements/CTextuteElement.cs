using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RWLayout.alpha2
{
    public class CTextuteElement : CElement
    {
        /// <summary>
        /// Texture
        /// </summary>
        public Texture2D Texture = null;
        /// <summary>
        /// Texture tint color
        /// </summary>
        /// <remarks>does not follow GUI.color die to static nature</remarks>
        public Color TintColor = Color.white;

        public Vector2 tryFitTex(Vector2 size, Vector2 margin)
        {
            // ApplyGeometryOnly();
            var result = (Texture?.Size() ?? Vector2.zero) + margin;
            // RestoreGeometryOnly();

            return result;
        }

        protected void ApplyAll()
        {
            GuiTools.PushColor(TintColor);
        }

        protected void RestoreAll()
        {
            GuiTools.PopColor();
        }

    }
}

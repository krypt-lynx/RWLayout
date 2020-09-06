using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RWLayout.alpha2
{
    public partial class CElement
    {
        /// <summary>
        /// can view able to detect interactions?
        /// </summary>
        public bool userInteractionEnabled = true;

        /// <summary>
        /// searches topmost view in current view tree at given point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual CElement hitTest(Vector2 point)
        {
            if (userInteractionEnabled && Bounds.Contains(point))
            {
                foreach (var element in Elements.Reverse())
                {
                    var subElement = element.hitTest(point);
                    if (subElement != null)
                    {
                        return subElement;
                    }
                }
                return this;
            }
            else
            {
                return null;
            }
        }
    }
}

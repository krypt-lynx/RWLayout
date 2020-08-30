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
        public bool userInteractionEnabled = true;
        public virtual CElement hitTest(Vector2 point)
        {
            if (userInteractionEnabled && bounds.Contains(point))
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

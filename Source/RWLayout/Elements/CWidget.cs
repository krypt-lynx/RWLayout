// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RWLayout
{
    public class CWidget : CElement
    {
        public Func<Vector2, Vector2> TryFitContect;
        public Action<CWidget> DoWidgetContent;

        public override Vector2 tryFit(Vector2 size)
        {
            if (TryFitContect == null)
            {
                return base.tryFit(size);
            }
            else
            {
                return TryFitContect(size);
            }
        }

        public override void DoContent()
        {
            base.DoContent();
            DoWidgetContent?.Invoke(this);
        }
    }
}

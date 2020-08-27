using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.Alpha1
{
    public class CWindowResizer : WindowResizer
    {
        public delegate void VerifyWindowSizeDelegate(ref Vector2 winSize);

        public Rect override_DoResizeControl(Rect winRect) // hooked by Harmony, original method is not virtual
        {
            Vector2 mousePosition = Event.current.mousePosition;
            Rect rect = new Rect(winRect.width - 24f, winRect.height - 24f, 24f, 24f);
            if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect))
            {
                this.isResizing = true;
                this.resizeStart = new Rect(mousePosition.x, mousePosition.y, winRect.width, winRect.height);
            }
            if (this.isResizing)
            {
                Vector2 size = new Vector2(
                     Mathf.Min(resizeStart.width + (mousePosition.x - this.resizeStart.x), UI.screenWidth - winRect.xMin),
                     Mathf.Min(resizeStart.height + (mousePosition.y - this.resizeStart.y), UI.screenHeight - winRect.yMin)
                    );

                UpdateSize(ref size);

                winRect.xMax = winRect.xMin + size.x;
                winRect.yMax = winRect.yMin + size.y;


                if (Event.current.type == EventType.MouseUp ||
                    !Input.GetMouseButton(0))
                {
                    this.isResizing = false;
                }
            }
            Widgets.ButtonImage(rect, TexUI.WinExpandWidget, true);

            return new Rect(winRect.x, winRect.y, (int)winRect.width, (int)winRect.height);
            // base.DoResizeControl
        }

        public VerifyWindowSizeDelegate UpdateSize = (ref Vector2 size) => { };

        private bool isResizing;
        private Rect resizeStart;
    }

}

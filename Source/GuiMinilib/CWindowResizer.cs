using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout_moddiff
{
    public class CWindowResizer : WindowResizer
    {
        public delegate void VerifyWindowSizeDelegate(ref Vector2 winSize);

        public Rect override_DoResizeControl(Rect winRect)
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
                    resizeStart.width + (mousePosition.x - this.resizeStart.x),
                    resizeStart.height + (mousePosition.y - this.resizeStart.y)
                    );

                size = new Vector2(
                    Mathf.Min(size.x, UI.screenWidth - winRect.xMin),
                    Mathf.Min(size.y, UI.screenHeight - winRect.yMin));

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

        public VerifyWindowSizeDelegate UpdateSize;

        // Token: 0x040012A8 RID: 4776
        private bool isResizing;

        // Token: 0x040012A9 RID: 4777
        private Rect resizeStart;
    }

}

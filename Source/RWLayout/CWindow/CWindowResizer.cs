﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
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
                this.IsResizing = true;
                this.resizeStart = new Rect(mousePosition.x, mousePosition.y, winRect.width, winRect.height);
            }
            bool wasResizing = this.IsResizing;
            if (this.IsResizing)
            {
                Vector2 size = new Vector2(
                    Mathf.Max(minWindowSize.x, Mathf.Min(resizeStart.width + (mousePosition.x - this.resizeStart.x), UI.screenWidth - winRect.xMin)),
                    Mathf.Max(minWindowSize.y, Mathf.Min(resizeStart.height + (mousePosition.y - this.resizeStart.y), UI.screenHeight - winRect.yMin))
                    );

                UpdateSize(ref size);

                winRect.xMax = winRect.xMin + size.x;
                winRect.yMax = winRect.yMin + size.y;


                if (Event.current.type == EventType.MouseUp ||
                    !Input.GetMouseButton(0))
                {
                    this.IsResizing = false;
                }
            }
            Widgets.ButtonImage(rect, TexUI.WinExpandWidget, true);

            if (wasResizing) 
            {
                return winRect.GUIRoundedPreserveOrigin();
            }
            else
            {
                return winRect;
            }
            // base.DoResizeControl
        }

        public VerifyWindowSizeDelegate UpdateSize = (ref Vector2 size) => { };

        public bool IsResizing { get; private set; }
        private Rect resizeStart;
    }

}

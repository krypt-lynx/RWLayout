using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod.ListingStandardDemo
{
    public class CDraggableElement : CElement
    {
        public static bool itemDraggig = false;
        public bool IsDragging = false;
        protected bool inCustomDrawing = false;

        public override bool Hidden
        {
            get => (base.Hidden || IsDragging) && !inCustomDrawing;
            set => base.Hidden = value;
        }

        public void DrawAt(Vector2 point)
        {
            var oldBounds = bounds;
            var oldRounded = boundsRounded;
            inCustomDrawing = true;

            GUI.BeginGroup(new Rect(-bounds.position + point, bounds.position + bounds.size));
            DoElementContent();
            GUI.EndGroup();

            inCustomDrawing = false;
            bounds = oldBounds;
            boundsRounded = oldRounded;
        }
    }


    public class ListItem : CDraggableElement
    {
        public ListItem(string Title)
        {
            var label = this.AddElement(new CLabel
            {
                Title = Title,
                TextAlignment = TextAnchor.MiddleCenter,
            });
            label.userInteractionEnabled = false;
            this.Embed(label);

            AddConstraint(height ^ 50);
        }


        public override void DoContent()
        {
            if ((Mouse.IsOver(bounds) && !itemDraggig) || inCustomDrawing)
            {
                Widgets.DrawHighlight(bounds);
            }

            base.DoContent();
        }
    }


    class TestWindow_ListingStandard : CWindow
    {
        string[] leftStrings = { "parka", "jacket", "pants" };
        string[] rightStrings = { "T-shirt" };

        CLabel elementInfo;

        public override void ConstructGui()
        {
            doCloseX = true;
            draggable = true;
            resizeable = true;

            InnerSize = new Vector2(500, 350);

            var leftFrame = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(new Color(1, 1, 1, 0.3f), () => GuiTools.Box(sender.bounds, new EdgeInsets(1))),
            });

            var listLeft = leftFrame.AddElement(new CListView());
            BuildList(listLeft, leftStrings);
            leftFrame.Embed(listLeft, new EdgeInsets(2));

            var rightFrame = Gui.AddElement(new CWidget
            {
                DoWidgetContent = (sender) => GuiTools.UsingColor(new Color(1, 1, 1, 0.3f), () => GuiTools.Box(sender.bounds, new EdgeInsets(1))),
            });

            var listRight = rightFrame.AddElement(new CListView());
            BuildList(listRight, rightStrings);
            rightFrame.Embed(listRight, new EdgeInsets(2));

            elementInfo = Gui.AddElement(new CLabel
            {
                Font = GameFont.Tiny
            });

            Gui.AddConstraints(
                Gui.left ^ leftFrame.left,
                leftFrame.right + 40 ^ rightFrame.left,
                rightFrame.width ^ leftFrame.width,
                rightFrame.right ^ Gui.right,

                Gui.left ^ elementInfo.left, Gui.right ^ elementInfo.right,

                Gui.top ^ leftFrame.top, Gui.top ^ rightFrame.top,
                leftFrame.bottom + 10 ^ elementInfo.top, rightFrame.bottom + 10 ^ elementInfo.top,
                elementInfo.height ^ GuiTools.UsingFont(GameFont.Tiny, () => Text.CurFontStyle.CalcSize(new GUIContent(" \n \n ")).y),
                elementInfo.bottom ^ Gui.bottom
                );

            Gui.AddConstraint(Gui.width ^ Gui.guideWidth);
            Gui.AddConstraint(Gui.height ^ Gui.guideHeight);
        }

        private void BuildList(CListView listLeft, string[] strings)
        {
            foreach (var str in strings)
            {
                var row = listLeft.AppendRow(new CListingRow());
                row.Embed(row.AddElement(new ListItem(str)));
            }
        }


        void TraceTree(CElement topleaf, out CListingRow row, out CListView list, out string debug)
        {
            list = null;
            row = null;
            var sb = new StringBuilder();
            var element = topleaf;
            while (element != null)
            {
                if (element is CListView listView)
                {
                    list = listView;
                }

                if (element is CListingRow listRow)
                {
                    row = listRow;
                }

                sb.Append(element.NamePrefix());
                if (element is CElementHost host)
                {
                    sb.Append(" + ");
                    element = host.Owner;
                }
                else
                {
                    sb.Append(" → ");
                    element = element.Parent;
                }
            }

            debug = sb.ToString();
        }

        Vector2 dragPoint;
        CListingRow draggingRow = null;

        public override void DoWindowContents(Rect inRect)
        {
            DoDrag(inRect);

            base.DoWindowContents(inRect);
              
            DrawDragGhost();
        }

        private void DrawDragGhost()
        {
            var elementToDraw = draggingRow?.Elements.FirstOrDefault() as CDraggableElement;
            if (elementToDraw != null)
            {              
                elementToDraw.DrawAt(Event.current.mousePosition - dragPoint);
            }
        }

        private void DoDrag(Rect inRect)
        {
            var element = Gui.hitTest(Event.current.mousePosition);

            CListingRow item;
            CListView targetList;
            string treeTrace;
            TraceTree(element, out item, out targetList, out treeTrace);

            // pick
            if (Mouse.IsOver(inRect) && Event.current.type == EventType.MouseDown && draggingRow == null)
            {
                draggingRow = item;
                var elementToDraw = draggingRow?.Elements.FirstOrDefault() as CDraggableElement;
                if (draggingRow != null)
                {
                    elementToDraw.IsDragging = true;
                    CDraggableElement.itemDraggig = true;
                }
                if (item != null)
                {
                    dragPoint = Event.current.mousePosition 
                        - targetList.bounds.position // translate to list coordinates
                        + targetList.ScrollPosition // scroll offset
                        - item.bounds.position; // delta
                    Event.current.Use();
                }
            }
           
            // drop
            if (((Event.current.type == EventType.MouseUp) || !Input.GetMouseButton(0)) && draggingRow != null)
            {

                if (draggingRow != null)
                {
                    if (Event.current.type == EventType.MouseUp)
                    {
                        Event.current.Use();
                    }

                    if (targetList != null)
                    {
                        var elementToDraw = draggingRow?.Elements.FirstOrDefault() as CDraggableElement;
                        elementToDraw.IsDragging = false;
                        CDraggableElement.itemDraggig = false;

                        int index = -1;
                        if (item != null)
                        {
                            index = targetList.IndexOfRow(item);
                        }
                        
                        ((CListView)draggingRow.Owner).RemoveRow(draggingRow);

                        if (index != -1)
                        {
                            targetList.InsertRow(index, draggingRow);
                        }
                        else
                        {
                            targetList.AppendRow(draggingRow);
                        }
                        targetList.UpdateLayoutTemp();
                    }

                    draggingRow = null;
                }
            }

            // drag
            if (draggingRow != null && Event.current.type == EventType.MouseDrag)
            {
                Event.current.Use();
            }

            var sb = new StringBuilder();
            sb.AppendLine(treeTrace);
            sb.AppendLine($"{draggingRow?.NamePrefix() ?? "-"}");
            sb.AppendLine($"{targetList?.NamePrefix() ?? "-"} {item?.NamePrefix() ?? "-"}");


            elementInfo.Title = sb.ToString();
        }
    }
}

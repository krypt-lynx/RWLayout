using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace RWLayoutMod.ListViewDemo
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
            var oldBounds = Bounds;
            var oldRounded = BoundsRounded;
            inCustomDrawing = true;

            GUI.BeginGroup(new Rect(-Bounds.position + point, Bounds.position + Bounds.size));
            DoElementContent();
            GUI.EndGroup();

            inCustomDrawing = false;
            Bounds = oldBounds;
            BoundsRounded = oldRounded;
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
            if ((Mouse.IsOver(BoundsRounded) && !itemDraggig) || inCustomDrawing)
            {
                Widgets.DrawHighlight(BoundsRounded);
            }

            base.DoContent();
        }
    }


    class TestWindow_ListView : CWindow
    {
        string[] leftStrings = { "parka", "jacket", "pants" };
        string[] rightStrings = { "T-shirt" };

        CLabel elementInfo;

        public override void ConstructGui()
        {
            doCloseX = true;
            draggable = true;
            MakeResizable(true, true);

            InnerSize = new Vector2(500, 350);

            var leftFrame = Gui.AddElement(new CFrame());

            var listLeft = leftFrame.AddElement(new CListView());
            BuildList(listLeft, leftStrings);
            leftFrame.Embed(listLeft, new EdgeInsets(2));

            var rightFrame = Gui.AddElement(new CFrame());

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
                if (element is IOwnedElement host)
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
                        - targetList.Bounds.position // translate to list coordinates
                        + targetList.ScrollPosition // scroll offset
                        - item.Bounds.position; // delta
                    Event.current.Use();
                }
            }
           
            // drop?
            if (((Event.current.type == EventType.MouseUp) || !Input.GetMouseButton(0)) && draggingRow != null)
            {

                if (draggingRow != null)
                {
                    if (Event.current.type == EventType.MouseUp)
                    {
                        Event.current.Use();
                    }

                    var elementToDraw = draggingRow?.Elements.FirstOrDefault() as CDraggableElement;
                    elementToDraw.IsDragging = false;
                    CDraggableElement.itemDraggig = false;

                    if (targetList != null)
                    {
                        // drop.

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
                        targetList.UpdateLayoutIfNeeded();
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

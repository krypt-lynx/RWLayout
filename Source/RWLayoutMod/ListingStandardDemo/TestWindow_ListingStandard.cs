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
    class ListItem : CElement
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
            if (Mouse.IsOver(bounds))
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
            //optionalTitle = "Listing_Standard";
            resizeable = true;

            InnerSize = new Vector2(500, 350);


            var listLeft = Gui.AddElement(new CListView());
            BuildList(listLeft, leftStrings);

            var listRight = Gui.AddElement(new CListView());
            BuildList(listRight, rightStrings);

            elementInfo = Gui.AddElement(new CLabel());
            elementInfo.Title = "test";

            Gui.AddConstraints(
                Gui.left ^ listLeft.left,
                listLeft.right + 40 ^ listRight.left,
                listRight.width ^ listLeft.width,
                listRight.right ^ Gui.right,

                Gui.left ^ elementInfo.left, Gui.right ^ elementInfo.right,

                Gui.top ^ listLeft.top, Gui.top ^ listRight.top,
                listLeft.bottom + 10 ^ elementInfo.top, listRight.bottom + 10 ^ elementInfo.top,
                elementInfo.height ^ 60, elementInfo.bottom ^ Gui.bottom
                );

            Gui.AddConstraint(Gui.width ^ Gui.guideWidth);
            Gui.AddConstraint(Gui.height ^ Gui.guideHeight);

            //Gui.StackLeft(true, true, ClStrength.Strong, listLeft, 40, (listRight, listLeft.width));
        }

        private void BuildList(CListView listLeft, string[] strings)
        {
            foreach (var str in strings)
            {
                var row = listLeft.NewRow();
                row.Embed(row.AddElement(new ListItem(str)));
            }
        }

        ListItem draggingItem = null;

        void TraceTree(CElement topleaf, out CListView list, out string debug)
        {
            list = null;
            var sb = new StringBuilder();
            var element = topleaf;
            while (element != null)
            {
                if (element is CListView listView)
                {
                    list = listView;
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

        public override void DoWindowContents(Rect inRect)
        {
            var element = Gui.hitTest(Event.current.mousePosition);

            CListView targetList;
            string treeTrace;
            TraceTree(element, out targetList, out treeTrace);


            if (Input.GetMouseButtonDown(0))
            {
                //Log.Message(element?.ToString() ?? "<null>");
                draggingItem = element as ListItem;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (draggingItem != null && targetList != null)
                {
                    var oldRow = draggingItem?.Parent as CListingRow;
                    oldRow.AddConstraint(oldRow.height ^ 20);
                    oldRow?.RemoveElement(draggingItem);

                    var row = targetList.NewRow();
                    row.Embed(row.AddElement(draggingItem));
                    targetList.UpdateLayoutTemp();

                    draggingItem = null;
                }
            }




            var sb = new StringBuilder();
            sb.AppendLine(treeTrace);
            sb.Append($"item: {draggingItem?.NamePrefix() ?? "null"}; ");
            sb.AppendLine($"target: {targetList?.NamePrefix() ?? "null"}");
            sb.AppendLine($"{Input.GetMouseButtonDown(0)}; {Mouse.IsOver(inRect)}");


            elementInfo.Title = sb.ToString();




            base.DoWindowContents(inRect);
        }

    }
}

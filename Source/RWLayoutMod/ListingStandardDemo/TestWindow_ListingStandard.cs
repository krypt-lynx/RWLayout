using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;
using UnityEngine;

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
            }) ;

            this.Embed(label);

            AddConstraint(height ^ 30);
        }
    }


    class TestWindow_ListingStandard : CWindow
    {
        string[] leftStrings = { "parka", "jacket", "pants" };
        string[] rightStrings = { "T-shirt" };

        public override void ConstructGui()
        {
            doCloseX = true;
            //optionalTitle = "Listing_Standard";
            //resizeable = true;

            Gui.AddConstraint(ClStrength.Weak, Gui.width ^ Gui.guideWidth);
            Gui.AddConstraint(ClStrength.Weak, Gui.height ^ Gui.guideHeight);

            var listLeft = Gui.AddElement(new CListView());
            BuildList(listLeft, leftStrings);

            var listRight = Gui.AddElement(new CListView());
            BuildList(listRight, rightStrings);

            Gui.ConstrainSize(500, 350);

            Gui.StackLeft(true, true, ClStrength.Strong, listLeft, 40, (listRight, listLeft.width));
        }

        private void BuildList(CListView listLeft, string[] strings)
        {
            foreach (var str in strings)
            {
                var row = listLeft.NewRow();
                row.Embed(row.AddElement(new ListItem(str)));
            }
        }

    }
}

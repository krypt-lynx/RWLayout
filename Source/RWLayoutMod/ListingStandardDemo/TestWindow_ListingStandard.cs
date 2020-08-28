using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.alpha2;

namespace RWLayoutMod.ListingStandardDemo
{
    class TestWindow_ListingStandard : CWindow
    {
        public override void ConstructGui()
        {
            doCloseX = true;
            optionalTitle = "Listing_Standard";
            //resizeable = true;

            Gui.AddConstraint(ClStrength.Weak, Gui.width ^ Gui.guideWidth);
            Gui.AddConstraint(ClStrength.Weak, Gui.height ^ Gui.guideHeight);

            var listing = Gui.AddElement(new CListingStandart {

            });

            Gui.ConstrainSize(500, 350);
            Gui.Embed(listing);


            //base.ConstructGui();
        }
    }
}

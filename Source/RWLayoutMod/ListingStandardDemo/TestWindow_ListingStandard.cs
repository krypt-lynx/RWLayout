using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using RWLayout.Alpha1;

namespace RWLayoutMod.ListingStandardDemo
{
    class TestWindow_ListingStandard : CWindow
    {
        public override void ConstructGui()
        {
            doCloseX = true;
            optionalTitle = "Listing_Standard";
            //resizeable = true;

            Gui.AddConstraint(ClStrength.Weak, Gui.width ^ this.guideWidth);
            Gui.AddConstraint(ClStrength.Weak, Gui.height ^ this.guideHeight);

            var listing = Gui.AddElement(new CListingStandart {

            });

            Gui.ConstrainSize(500, 350);
            Gui.Embed(listing);


            //base.ConstructGui();
        }
    }
}

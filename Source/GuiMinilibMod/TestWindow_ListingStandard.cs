using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using GuiMinilib;

namespace GuiMinilibMod
{
    class TestWindow_ListingStandard : CWindow
    {
        public override void ConstructGui()
        {
            doCloseX = true;
            optionalTitle = "Listing_Standard";
            //resizeable = true;

            Gui.solver.AddConstraint(Gui.width, this.width, (a, b) => a == b, ClStrength.Weak);
            Gui.solver.AddConstraint(Gui.height, this.height, (a, b) => a == b, ClStrength.Weak);

            var listing = Gui.AddElement(new CListingStandart {

            });

            Gui.ConstrainSize(500, 350);
            Gui.Embed(listing);




            //base.ConstructGui();
        }
    }
}

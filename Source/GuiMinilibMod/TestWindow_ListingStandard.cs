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

            Gui.solver.AddConstraints(ClStrength.Weak, Gui.width ^ this.width);
            Gui.solver.AddConstraints(ClStrength.Weak, Gui.height ^ this.height);

            var listing = Gui.AddElement(new CListingStandart {

            });

            Gui.ConstrainSize(500, 350);
            Gui.Embed(listing);




            //base.ConstructGui();
        }
    }
}

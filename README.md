Die to some unreconsilable differences in my and Microsoft's views on convinience, reasonable security risks, and potentioal force major factors, I had to move my repositories from from GitHub.
GitLab mirror for this repo: https://gitlab.com/krypt_lynx/RWLayout

# RWLayout
RWLayout is a constraint based layout implementation for RimWorld.

# Disclimer
This early alpha version. Everything is subject to change. Bugs are expected. Missing functionality is known.

# Classes
`CWindow`
Window subclass hosing constraint based UI: provides features such constraint based window size and resizing.
	
`CGuiRoot`
Host for constrait based ui elements. Can be inserted in vanilla GUI.
	
`CElement`
Base class for every constraint compatible element. Can be used to host vanilla controls, to implement custom wrappers or controls or just as a container for other elements
	
`CButton`
RWLayout wrapper for button
	
`CLabel`
RWLayout wrapper for label
	
`CCheckBox `
RWLayout wrapper for checkbox
	
`CWidget`
Bridging class for fast wrapping of simple gui elements, such as `Widgets.DrawAltColor`
	
`CListView`
RWLayout List implementation. Provides a way to display set of rows. Based on `Widgets.BeginScrollView`
	
`CListingStandart`
Rudementary wrapper for `Listing_Standart`. Not fully implemented

# Versioning
I will move every release to different namespace. It should help to keep mod compatibility
Current one is RWLayout.alpha1

# Steam
SteamWorkshop mod to reference: https://steamcommunity.com/sharedfiles/filedetails/?edit=true&id=2209393954

# Known issues
- Unit tests are missing
- Intrinsic size anchors are functional only for CLabel, CListView, and CListingStandart

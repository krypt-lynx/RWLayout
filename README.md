RWLayout

Constraint based layout implementation for RimWorld.
This early alpha version. Everything is subject to change. Bugs are expected. Missing functionality is known.

RWLayout dinamically resolves gui elements locations and sizes based on constraints defined by developer.

Intended to be compatible with vanilla RimWorld interface: you can combine both approaches in single window.

Classes:

CWindow
    Window subclass hosing constraint based UI: provides features such constraint based window size and resizing.
CGuiRoot
	Host for constrait based ui elements. Can be inserted in vanilla GUI.
CElement
	Base class for every constraint compatible element.
	Can be used to host vanilla controls, to implement custom wrappers or controls or just as a container for other elements
CButton
	RWLayout wrapper for button
CLabel 
	RWLayout wrapper for label
CCheckBox 
	RWLayout wrapper for checkbox
CWidget
	Bridging class for fast wrapping of simple gui elements, such as `Widgets.DrawAltColor`
CListView
	RWLayout List implementation. Provides a way to display set of rows. Based on `Widgets.BeginScrollView`
CListingStandart
	Rudementary wrapper for `Listing_Standart`. Not fully implemented
	
Known important issues:
- Removed views cannot be readded.

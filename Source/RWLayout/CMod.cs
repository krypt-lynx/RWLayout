using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Extended base mod class. Provides constraints functionality for mod settigns window.
    /// </summary>
    public abstract class CMod : Mod
    {
        public CMod(ModContentPack content) : base(content) { }

        CGuiRoot gui = null;
        /// <summary>
        /// Root Gui element
        /// </summary>
        public CGuiRoot Gui
        {
            get
            {
                if (gui == null)
                {
                    gui = new CGuiRoot();
                    ConstructGui();
                }
                return gui;
            }
        }

        public override void WriteSettings()
        {
            base.WriteSettings();

            gui = null; // this method is called after settings window close
        }

        /// <summary>
        /// Override this method to construct settings Gui in it.
        /// </summary>
        public virtual void ConstructGui() { }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            var settingsGui = Gui;
            settingsGui.InRect = inRect;
            settingsGui.UpdateLayoutIfNeeded();
            settingsGui.DoElementContent();
        }
    }
}

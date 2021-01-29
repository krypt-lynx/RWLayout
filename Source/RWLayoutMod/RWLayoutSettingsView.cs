using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayoutMod
{
    class RWLayoutSettingsView : CSettingsView
    {
        public override string Category()
        {
            return "RWLayout";
        }

        public override string FooterText()
        {
            return $"Mod version: {RWLayoutMod.VersionString()}\nLib version: {RWLayoutMod.GetLibVersionString()}";
        }

        public RWLayoutSettingsView(CMod mod) : base(mod)
        {
            this.StackTop(StackOptions.Create(intrinsicIfNotSet: true, constrainEnd: false),
                this.AddElement(new CCheckboxLabeled
                {
                    Title = "Layout debug",
                    Checked = RWLayoutMod.settings.layoutDebug,
                    Changed = (_, value) =>
                    {
                        RWLayoutMod.settings.layoutDebug = value;
                        CElement.DebugDraw = value;
                    },
                }), 10,
                this.AddElement(new CCheckboxLabeled
                {
                    Title = "Patch missing Log null check (requires restart)",
                    Tip = "Fixes Log.Message, Log.Warning, and Log.Error breaking LogWindow if called with null argument",
                    Checked = RWLayoutMod.settings.patchLog,
                    Changed = (_, value) => RWLayoutMod.settings.patchLog = value,
                }), 2,
                this.AddElement(new CCheckboxLabeled
                {
                    Title = "Patch sticky window resizing bug (requires restart)",
                    Tip = "Fixes windows missing mouse up event during resizing if mouse was outside the window at the moment of event",
                    Checked = RWLayoutMod.settings.patchWindowResize,
                    Changed = (_, value) => RWLayoutMod.settings.patchWindowResize = value,
                }), 10,
                this.AddElement(new CLabel {
                    Title = "Examples was moved to separate mod"
                }));
        }
    }
}

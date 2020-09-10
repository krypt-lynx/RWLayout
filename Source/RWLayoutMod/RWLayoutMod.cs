using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RWLayout.alpha2;
using HarmonyLib;
using UnityEngine;
using Verse;
using System.IO;
using Cassowary;

namespace RWLayoutMod
{
    public class Settings : ModSettings
    {
        public bool layoutDebug = false;
        public bool showExamplesButtom = false;

        public bool patchLog = false;
        public bool patchWindowResize = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref layoutDebug, "layoutDebug", false);
            Scribe_Values.Look(ref showExamplesButtom, "showExamplesButtom", false);
            Scribe_Values.Look(ref patchLog, "patchLog", false);
            Scribe_Values.Look(ref patchWindowResize, "patchWindowResize", false);

            CElement.DebugDraw = layoutDebug;

            base.ExposeData();
        }
    }

    sealed public class RWLayoutMod : Mod
    {
        public static string packageIdOfMine = null;
        public static Settings settings = null;
        public static string commitInfo = null;

        public RWLayoutMod(ModContentPack content) : base(content)
        {
            ReadModInfo(content);
            settings = GetSettings<Settings>();

            Harmony harmony = new Harmony(packageIdOfMine);

            ApplyPatches(harmony);
        }

        private static void ApplyPatches(Harmony harmony)
        {
            // Extra button in devmode toolbar
            if (settings.showExamplesButtom)
            {
                harmony.Patch(AccessTools.Method(typeof(DebugWindowsOpener), "DevToolStarterOnGUI"),
                   postfix: new HarmonyMethod(typeof(DevToolsPatches), "DevToolStarterOnGUI_postfix"));
                harmony.Patch(AccessTools.Method(typeof(DebugWindowsOpener), "DrawButtons"),
                    postfix: new HarmonyMethod(typeof(DevToolsPatches), "DrawButtons_postfix"));
            }
            // CWindow resize injection
            harmony.Patch(AccessTools.Method(typeof(WindowResizer), "DoResizeControl"),
                prefix: new HarmonyMethod(typeof(RWLayoutResizerPatches), "DoResizeControl_prefix"));

            // RWLayout harmony example
            NativeWindow.MixedUseExamplePatches.Prepare();
            harmony.Patch(AccessTools.PropertyGetter(typeof(NativeWindow.WindowTest_NativeWindow), "InitialSize"),
                postfix: new HarmonyMethod(typeof(NativeWindow.MixedUseExamplePatches), "InitialSize_postfix"));
            harmony.Patch(AccessTools.Method(typeof(NativeWindow.WindowTest_NativeWindow), "DoWindowContents"),
                prefix: new HarmonyMethod(typeof(NativeWindow.MixedUseExamplePatches), "DoWindowContents_prefix"));

            // bugfixes should not be there but in separate mod, but those are annoying during development and I'm lazy to create a new one
            // bugfixes:
            // missing null checks in Log class
            if (settings.patchLog)
            {
                harmony.Patch(AccessTools.Constructor(typeof(LogMessage), new Type[] { typeof(string) }),
                    prefix: new HarmonyMethod(typeof(MiscFixes), "LogMessage_ctor_string_prefix"));
                harmony.Patch(AccessTools.Constructor(typeof(LogMessage), new Type[] { typeof(LogMessageType), typeof(string), typeof(string) }),
                    prefix: new HarmonyMethod(typeof(MiscFixes), "LogMessage_ctor_LogMessageType_string_string_prefix"));
            }

            // window resizing issues
            if (settings.patchWindowResize)
            {
                harmony.Patch(AccessTools.Method(typeof(WindowResizer), "DoResizeControl"),
                    postfix: new HarmonyMethod(typeof(MiscFixes), "DoResizeControl_postfix"));
            }
        }

        private static void ReadModInfo(ModContentPack content)
        {
            packageIdOfMine = content.PackageId;

            var name = Assembly.GetExecutingAssembly().GetName().Name;

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(name + ".git.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    commitInfo = reader.ReadToEnd()?.TrimEndNewlines();
                }
            }
            catch
            {
                commitInfo = null;
            }
        }

        CGuiRoot settingsGui = null;
        CGuiRoot SettingsGui
        {
            get
            {
                if (settingsGui == null)
                {
                    settingsGui = new CGuiRoot();
                    ConstructSettingsGUI(settingsGui);
                }
                return settingsGui;
            }
        }

        public override void WriteSettings()
        {
            base.WriteSettings();

            settingsGui = null; // this method is called after settings window close
        }

        public override string SettingsCategory()
        {
            return "RWLayout";
        }


        private void ConstructSettingsGUI(CGuiRoot settingsGui)
        {
            var layoutDebug = settingsGui.AddElement(new CCheckBox
            {
                Title = "Layout debug",
                Checked = settings.layoutDebug,
                Changed = (_, value) => {
                    settings.layoutDebug = value;
                    CElement.DebugDraw = value;
                },
            });

            var showExamples = settingsGui.AddElement(new CCheckBox
            {
                Title = "Show examples button (requires restart)",
                Tip = "Show examples button in dev mode tools panel",
                Checked = settings.showExamplesButtom,
                Changed = (_, value) => settings.showExamplesButtom = value,
            });

            var patchLog = settingsGui.AddElement(new CCheckBox
            {
                Title = "Patch missing Log null check (requires restart)",
                Tip = "Fixes Log.Message, Log.Warning, and Log.Error breaking LogWindow if called with null argument",
                Checked = settings.patchLog,
                Changed = (_, value) => settings.patchLog = value,
            });

            var patchResize = settingsGui.AddElement(new CCheckBox
            {
                Title = "Patch sticky window resizing bug (requires restart)",
                Tip = "Fixes windows missing mouse up event during resizing if mouse was outside the window at the moment of event",
                Checked = settings.patchWindowResize,
                Changed = (_, value) => settings.patchWindowResize = value,
            });

            settingsGui.StackTop(StackOptions.Create(intrinsicIfNotSet: true, constrainEnd: false), layoutDebug, 2, showExamples, 10, patchLog, 2, patchResize);

            var examplesButton = settingsGui.AddElement(new CButton
            {
                Title = "RWLayout Examples",
                Action = (_) => Find.WindowStack.Add(new TestsListWindow()),
            });

            examplesButton.ConstrainSize(200, 35);
            settingsGui.AddConstraint(examplesButton.left ^ settingsGui.left);
            settingsGui.AddConstraint(examplesButton.top ^ patchResize.bottom + 10);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            var settingsGui = SettingsGui;
            settingsGui.InRect = inRect;
            settingsGui.UpdateLayoutIfNeeded();
            settingsGui.DoElementContent();
        }
    }
}

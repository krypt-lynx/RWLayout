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

    sealed public class RWLayoutMod : CMod
    {
        public static string packageIdOfMine { get; private set; } = null; 
        public static Settings settings { get; private set; } = null;
        public static string commitInfo { get; private set; } = null;
        public static bool isDevBuild { get; private set; } = false;
        public static string VersionString()
        {
            return commitInfo + (isDevBuild ? "-dev" : "");
        }

        public RWLayoutMod(ModContentPack content) : base(content)
        {
            ReadModInfo(content);
            settings = GetSettings<Settings>();

            Harmony harmony = new Harmony(packageIdOfMine);

            ApplyPatches(harmony);

            RWLayoutHarmonyPatches.PatchOnce();
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
            isDevBuild = packageIdOfMine.EndsWith(".dev");

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
         

        public override string SettingsCategory()
        {
            return "RWLayout";
        }

        public override void ConstructGui()
        {

            CElement lastElement;

            Gui.StackTop(StackOptions.Create(intrinsicIfNotSet: true, constrainEnd: false),
                Gui.AddElement(new CCheckboxLabeled
                {
                    Title = "Layout debug",
                    Checked = settings.layoutDebug,
                    Changed = (_, value) =>
                    {
                        settings.layoutDebug = value;
                        CElement.DebugDraw = value;
                    },
                }), 2,
                Gui.AddElement(new CCheckboxLabeled
                {
                    Title = "Show examples button (requires restart)",
                    Tip = "Show examples button in dev mode tools panel",
                    Checked = settings.showExamplesButtom,
                    Changed = (_, value) => settings.showExamplesButtom = value,
                }), 10,
                Gui.AddElement(new CCheckboxLabeled
                {
                    Title = "Patch missing Log null check (requires restart)",
                    Tip = "Fixes Log.Message, Log.Warning, and Log.Error breaking LogWindow if called with null argument",
                    Checked = settings.patchLog,
                    Changed = (_, value) => settings.patchLog = value,
                }), 2,
                Gui.AddElement(lastElement = new CCheckboxLabeled
                {
                    Title = "Patch sticky window resizing bug (requires restart)",
                    Tip = "Fixes windows missing mouse up event during resizing if mouse was outside the window at the moment of event",
                    Checked = settings.patchWindowResize,
                    Changed = (_, value) => settings.patchWindowResize = value,
                }));



            var examplesButton = Gui.AddElement(new CButtonText
            {
                Title = "RWLayout Examples",
                Action = (_) => Find.WindowStack.Add(new TestsListWindow()),
            });

            examplesButton.ConstrainSize(200, 35);
            Gui.AddConstraints(
                examplesButton.left ^ Gui.left,
                examplesButton.top ^ lastElement.bottom + 10);

            var versionInfo = Gui.AddElement(new CLabel
            {
                Title = $"RWLayout version: {RWLayoutMod.VersionString()}",
                //Multiline = true,
                Color = new Color(1, 1, 1, 0.5f),
                Font = GameFont.Tiny,
                TextAlignment = TextAnchor.UpperRight,
            });
            Gui.StackBottom(StackOptions.Create(intrinsicIfNotSet: true, constrainEnd: false), versionInfo);
        }
    }
}

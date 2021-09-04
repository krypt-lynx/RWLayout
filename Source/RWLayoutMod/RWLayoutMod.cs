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

        public bool patchLog = false;
        public bool patchWindowResize = false;
        public bool patchLoadPatches = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref layoutDebug, nameof(layoutDebug), false);
            Scribe_Values.Look(ref patchLog, nameof(patchLog), false);
            Scribe_Values.Look(ref patchWindowResize, nameof(patchWindowResize), false);
            Scribe_Values.Look(ref patchLoadPatches, nameof(patchLoadPatches), true);

            CElement.DebugDraw = layoutDebug;

            base.ExposeData();
        }
    }

    sealed public class RWLayoutMod : CMod
    {
        public static Settings settings { get; private set; } = null;
        public static string commitInfo { get; private set; } = null;
        public static bool isDevBuild { get; private set; } = false;

        public static CMod Instance = null;
        public static string PackageIdOfMine
        {
            get
            {
                return Instance.Content?.PackageId;
            }
        }

        public static string VersionString()
        {
            return commitInfo;
        }

        public static string GetLibVersionString()
        {
            string libVersionString = null;
             
            var type = GenTypes.GetTypeInAnyAssembly("RWLayout.alpha2.RWLayoutInfo");
            if (type != null)
            {
                try
                {
                    libVersionString = (string)type.GetProperty("VersionInfo")?.GetValue(null);
                }
                catch
                {
                    libVersionString = null;
                }
            }

            return libVersionString ?? "unknown";
        }

        public RWLayoutMod(ModContentPack content) : base(content)
        {
            ReadModInfo(content);
            settings = GetSettings<Settings>();

            Harmony harmony = new Harmony(PackageIdOfMine);

            ApplyPatches(harmony);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            RWLayoutInfo.WriteSettings();

        }

        private static void ApplyPatches(Harmony harmony)
        {
            // bugfixes should not be there but in separate mod, but those are annoying during development and I'm lazy to create a new one
            // bugfixes:
            // missing null checks in Log class
            if (settings.patchLog)
            {
                MiscFixes.PatchLogMessage(harmony);
            }

            // window resizing issues
            if (settings.patchWindowResize)
            {
                MiscFixes.PatchDoResizeControl(harmony);
            }

            // malforned xml patch crash
            if (settings.patchLoadPatches)
            {
                MiscFixes.TryPatchLoadPatches(harmony);
            }
        }

        private void ReadModInfo(ModContentPack content)
        {
            Instance = this;
            isDevBuild = PackageIdOfMine.EndsWith(".dev");

            var name = MethodBase.GetCurrentMethod().DeclaringType.Namespace;

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

        public override string FooterText()
        {
            return $"Mod version: {RWLayoutMod.VersionString()}\nLib version: {RWLayoutMod.GetLibVersionString()}";
        }

        public void DoLayoutDebugChanged(CCheckboxLabeled sender, bool value)
        {
            CElement.DebugDraw = value;
        }

        public override CElement CreateSettingsView()
        {
            return DefDatabase<LayoutDef>.GetNamed("RWLayout_ModSettings").Instantiate(new Dictionary<string, object> {
                { "mod", this },
                { "settings", settings },
                { "rwsettings", typeof(RWLayoutInfo) }
            }).FirstOrDefault();
        }
    }
}

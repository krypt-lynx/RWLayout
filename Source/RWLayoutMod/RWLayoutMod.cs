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

        public override void ExposeData()
        {
            Scribe_Values.Look(ref layoutDebug, "layoutDebug", false);
            Scribe_Values.Look(ref patchLog, "patchLog", false);
            Scribe_Values.Look(ref patchWindowResize, "patchWindowResize", false);

            CElement.DebugDraw = layoutDebug;

            base.ExposeData();
        }
    }

    sealed public class RWLayoutMod : CMod
    {
        public static Settings settings { get; private set; } = null;
        public static string commitInfo { get; private set; } = null;
        public static bool isDevBuild { get; private set; } = false;

        public static string VersionString()
        {
            return commitInfo + (isDevBuild ? "-dev" : "");
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

        private static void ApplyPatches(Harmony harmony)
        {
            // bugfixes should not be there but in separate mod, but those are annoying during development and I'm lazy to create a new one
            // bugfixes:
            // missing null checks in Log class
            if (settings.patchLog)
            {
                

                harmony.Patch(AccessTools.Constructor(typeof(LogMessage), new Type[] { typeof(string) }),
                    prefix: new HarmonyMethod(typeof(MiscFixes), nameof(MiscFixes.LogMessage_ctor_string_prefix)));
                harmony.Patch(AccessTools.Constructor(typeof(LogMessage), new Type[] { typeof(LogMessageType), typeof(string), typeof(string) }),
                    prefix: new HarmonyMethod(typeof(MiscFixes), nameof(MiscFixes.LogMessage_ctor_LogMessageType_string_string_prefix)));
            }

            // window resizing issues
            if (settings.patchWindowResize)
            {
                harmony.Patch(AccessTools.Method(typeof(WindowResizer), nameof(WindowResizer.DoResizeControl)),
                    postfix: new HarmonyMethod(typeof(MiscFixes), nameof(MiscFixes.DoResizeControl_postfix)));
            }
        }

        private static void ReadModInfo(ModContentPack content)
        {
            isDevBuild = PackageIdOfMine.EndsWith(".dev");

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

        public override CSettingsView CreateSettingsView()
        {
            return new RWLayoutSettingsView(this);
        }
    }

}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    internal class RWSettings : ModSettings
    {
        public bool patchAllActiveAssemblies = true;
        public bool verboseLogging = false;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref patchAllActiveAssemblies, nameof(patchAllActiveAssemblies), true);
            Scribe_Values.Look(ref verboseLogging, nameof(verboseLogging), false);
        }
    }


    sealed public class RWLayoutModInternal : Mod
    {
        
        const string rwLayoutHarmonyId = "rwlayout.alpha2.internal";

        public RWLayoutModInternal(ModContentPack content) : base(content)
        {
            RWLayoutInfo.modInstance = this;
            RWLayoutInfo.settings = GetSettings<RWSettings>();

            Harmony harmony = new Harmony(rwLayoutHarmonyId);
            RWLayoutHarmonyPatches.Patch(harmony);

            CModHelper.Patch(harmony);

            ParseHelper.Parsers<EdgeInsets>.Register(ParseEdgeInsets);
        }


        static EdgeInsets ParseEdgeInsets(string value)
        {
            int start = value.IndexOf('{');
            int end = value.IndexOf('}');

            if (start == -1 || end == -1 || end - start <= 0)
            {
                throw new FormatException("braces are not ballanced");
            }

            var values = value.Substring(start + 1, end - start - 1)
                .Split(',')
                .Select(x => (float)Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray();

            if (values.Length == 1)
            {
                return new EdgeInsets(values[0]);
            } 
            else if (values.Length == 4)
            {
                return new EdgeInsets(values[0], values[1], values[2], values[3]);
            }
            else
            {
                throw new FormatException($"unexpected number of components: given {values.Length} expected 1 or 4");
            }
        }
    }

    public class RWLayoutInfo
    {
        static RWLayoutInfo()
        {
            ReadVersionInfo();
        }

        internal static RWSettings settings = null;
        internal static RWLayoutModInternal modInstance;

        public static void WriteSettings()
        {
            modInstance.WriteSettings();
        }

        public static bool PatchAllActiveAssemblies
        {
            get => settings.patchAllActiveAssemblies;
            set => settings.patchAllActiveAssemblies = value;
        }
        public static bool VerboseLogging
        {
            get => settings.verboseLogging;
            set => settings.verboseLogging = value;
        }

        private static string versionInfo = null;

        public static string VersionInfo => versionInfo;

        private static void ReadVersionInfo()
        {
          
            var name = Assembly.GetExecutingAssembly().GetName().Name;

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("git.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    versionInfo = reader.ReadToEnd()?.TrimEndNewlines();
                }
            }
            catch
            {
                versionInfo = "missing";
            }
        }
    }
}

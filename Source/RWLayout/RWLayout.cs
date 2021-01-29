using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    [StaticConstructorOnStartup]
    internal class RWLayoutInit
    {
        const string rwLayoutHarmonyId = "rwlayout.alpha2";

        static RWLayoutInit()
        {
            Harmony harmony = new Harmony(rwLayoutHarmonyId);

            RWLayoutHarmonyPatches.Patch(harmony);
            CModHelper.Patch(harmony);
        }
    }

    public class RWLayoutInfo
    {
        static RWLayoutInfo()
        {
            ReadVersionInfo();
        }

        private static string versionInfo = null;
        public static string VersionInfo => versionInfo;

        private static void ReadVersionInfo()
        {
          
            var name = Assembly.GetExecutingAssembly().GetName().Name;

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("git.txt")) // can I use manifest resources with matching names in differenct assemblies?
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

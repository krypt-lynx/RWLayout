using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayoutVersionTester
{

    sealed public class RWLayoutVersionTester : Mod
    {
        public static string commitInfo { get; private set; } = null;

        public static Mod Instance = null;
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

        public RWLayoutVersionTester(ModContentPack content) : base(content)
        {
            ReadModInfo(content);

            Harmony harmony = new Harmony(PackageIdOfMine);

            ApplyPatches(harmony);
        }

        static bool showMismatchMessageOnce = true;
        private static void ShowMismatchMessage()
        {
            if (showMismatchMessageOnce)
            {
                showMismatchMessageOnce = false;

                var modVer = VersionString();
                var libVer = GetLibVersionString();

                if (modVer != libVer)
                {
                    Log.Warning($"RWLayout version mismatch: mod {modVer}; lib {libVer}");

                    Find.WindowStack.Add(new Dialog_MessageBox(
                        text:
                        $"RWLayout service mod version does not match lib version.\n" +
                        $"\n" +
                        $"It can be caused by wrong mod order and mod embedding a version of the lib. Ensure RWLayout is placed right after core mods\n" +
                        $"\n" +
                        $"Mod version:\t{modVer}\n" +
                        $"Lib version:\t{libVer}",
                        title: "Unexpected RWLayout version"));
                }
            }
        }

        private static void ApplyPatches(Harmony harmony)
        {
            // Version tester
            harmony.Patch(AccessTools.Method(typeof(UIRoot_Entry), nameof(UIRoot_Entry.Init)),
                postfix: new HarmonyMethod(typeof(RWLayoutVersionTester), nameof(RWLayoutVersionTester.ShowMismatchMessage)));
        }

        private void ReadModInfo(ModContentPack content)
        {
            Instance = this;

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
    }
}

using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{

    internal static class CModHelper
    {
        internal static void Patch(Harmony harmony) // todo: move call to static constructor of CMod
        {
            // CWindow resize injection
            try
            {
                harmony.Patch(AccessTools.Method(typeof(Dialog_ModSettings), nameof(Dialog_ModSettings.PreClose)),
                    postfix: new HarmonyMethod(typeof(CModHelper), nameof(CModHelper.DestroyCModUI)));

                var hugsLibPreClose = GenTypes.GetTypeInAnyAssembly("RimWorld.Dialog_VanillaModSettings")?.GetMethod(nameof(Window.PreClose), new Type[] { });
                if (hugsLibPreClose != null)
                {
                    harmony.Patch(hugsLibPreClose, postfix: new HarmonyMethod(typeof(CModHelper), nameof(CModHelper.DestroyCModUI)));
                }
            } 
            catch (Exception e)
            {
                LogHelper.LogException("exception during settings window patching", e);
            }
        }

        private static void DestroyCModUI()
        {
            foreach (Mod mod in LoadedModManager.ModHandles)
            {
                if (mod is CMod cmod)
                {
                    cmod.DestroyGui();
                }
            }
        }
    }

    /// <summary>
    /// Extended base mod class. Provides constraints functionality for mod settigns window.
    /// </summary>
    public abstract class CMod : Mod
    {
        public static CMod Instance = null;
        public static string PackageIdOfMine
        {
            get
            {
                return Instance.Content?.PackageId;
            }
        }

        public CMod(ModContentPack content) : base(content)
        {
            Instance = this;
        }

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

        CSettingsView settingsView = null;
        /// <summary>
        /// Root Gui element
        /// </summary>
        public virtual CSettingsView SettingsView
        {
            get
            {
                if (settingsView == null)
                {
                    settingsView = CreateSettingsView();
                }
                return settingsView;
            }
        }

        public override string SettingsCategory()
        {
            return SettingsView?.Category();
        }


        /// <summary>
        /// Override this method to construct settings Gui in it.
        /// </summary>
        public virtual void ConstructGui() {

            if (SettingsView != null)
            {
                Gui.Embed(Gui.AddElement(SettingsView));
            }

            string footerText = null;
            if ((footerText = SettingsView?.FooterText()) != null)
            {
                var footer = Gui.AddElement(new CLabel
                {
                    Title = footerText,
                    TextAlignment = TextAnchor.LowerRight,
                    Color = new Color(1, 1, 1, 0.5f),
                    Font = GameFont.Tiny
                });

                Gui.AddConstraints(
                    footer.top ^ Gui.bottom + 3,
                    footer.width ^ footer.intrinsicWidth,
                    footer.right ^ Gui.right,
                    footer.height ^ footer.intrinsicHeight);
            }
        }

        /// <summary>
        /// Override this method to create settings view
        /// </summary>
        /// <returns>the settings view</returns>
        public virtual CSettingsView CreateSettingsView() { return null; }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            var settingsGui = Gui;
            settingsGui.InRect = inRect;
            settingsGui.UpdateLayoutIfNeeded();
            settingsGui.DoElementContent();
        }

        public virtual void DestroyGui()
        {
            gui = null;
            settingsView = null;
        }
    }

    /// <summary>
    /// View for mod settings
    /// </summary>
    public abstract class CSettingsView : CElement
    {
        public CMod Mod { get; }

        public CSettingsView(CMod mod)
        {
            Mod = mod;
        }

        /// <summary>
        /// Mod settings category
        /// </summary>
        /// <returns></returns>
        public virtual string Category()
        {
            return Mod.Content.Name;
        }

        /// <summary>
        /// Mod version (will be show at right if close button)
        /// </summary>
        /// <returns></returns>
        public virtual string FooterText()
        {
            string version = Version();
            if (version == null)
            {
                return null;
            }
            else
            {
                return string.Format($"Version: {0}".Translate(), version);
            }
        }

        /// <summary>
        /// Mod version (will be show at right if close button)
        /// </summary>
        /// <returns></returns>
        public virtual string Version()
        {
            return null;
        }
    }
}

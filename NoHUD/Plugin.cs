using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using IllusionPlugin;
using Harmony;
using System.Reflection;
using IllusionInjector;
using CustomUI.GameplaySettings;
using CustomUI.Utilities;
using UnityEngine.UI;

namespace NoHUDPlus
{
    public class Plugin : IPlugin
    {
        public string Name => "NoHUDPlus";
        public string Version => "2.0.0";

        bool init = false; 
        public static bool CamPlusInstalled = false;
        public static bool CountersPlusInstalled = false;

        public static readonly string[] cameraplugins = { "CameraPlus", "CameraPlusOrbitEdition", "DynamicCamera" };

        private void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GameCore" &&
                (ModPrefs.GetBool("NoHUDPlus", "HMDEnabled", false, true) || ModPrefs.GetBool("NoHUDPlus", "MirrorEnabled", false, true)))
                new GameObject("HUDHider").AddComponent<HUDHider>();
        }
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (!init)
            {
                CamPlusInstalled = PluginManager.Plugins.Any(p => cameraplugins.Contains(p.Name));
                CountersPlusInstalled = PluginManager.Plugins.Any(p => p.Name == "Counters+");

                if (CamPlusInstalled) Console.WriteLine("[NoHUDPlus] Found CameraPlus (or related camera mod)");
                if (CountersPlusInstalled) Console.WriteLine("[NoHUDPlus] Found CountersPlus");

                init = true;
            }

            if (arg0.name != "Menu") return;

            var sprite = UIUtilities.LoadSpriteFromResources("NoHUD.Resources.NOHUDIcon.png");
            
            ToggleOption hmd = GameplaySettingsUI.CreateToggleOption("NoHUD - HMD", "Hide UI elements that show in-game in your VR headset.", sprite);
            hmd.GetValue = ModPrefs.GetBool("NoHUDPlus", "HMDEnabled", false, true);
            hmd.OnToggle += delegate (bool value) { ModPrefs.SetBool("NoHUDPlus", "HMDEnabled", value); };

            if (CamPlusInstalled)
            {
                ToggleOption mirror = GameplaySettingsUI.CreateToggleOption("NoHUD - Mirror", "Hide UI elements that show in-game in the game mirror.", sprite);
                mirror.GetValue = ModPrefs.GetBool("NoHUDPlus", "MirrorEnabled", false, true);
                mirror.OnToggle += delegate (bool value) { ModPrefs.SetBool("NoHUDPlus", "MirrorEnabled", value); };
            }
        }

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // set default modpref values
            ModPrefs.GetBool("NoHUDPlus", "HMDEnabled", false, true);
            ModPrefs.GetBool("NoHUDPlus", "MirrorEnabled", false, true);

            try
            {
                var harmony = HarmonyInstance.Create("com.bigfoot.BeatSaber.NoHUDPlus");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Console.WriteLine("[NoHUDPlus] This plugin requires Harmony. Make sure you installed the mod correctly.\n" + e);
            }
        }
        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        public void OnLevelWasLoaded(int level) { }
        public void OnLevelWasInitialized(int level) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }
    }
}
using IllusionPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using NoHUDPlus.Util;
using CountersPlus;

namespace NoHUDPlus
{
    class HUDHider : MonoBehaviour
    {
        GameObject multi;
        GameObject combo;
        GameObject energy;
        GameObject score;
        GameObject failText;
        GameObject progress;
        
        GameObject fcdisplay_ring;
        GameObject tweaks_time;

        private void Awake()
        {
            Console.WriteLine("[NoHUDPlus] Object initialized. Waiting for load.");
            StartCoroutine(WaitForLoad());
        }

        IEnumerator WaitForLoad()
        {
            while (multi == null || combo == null || score == null)
            {
                yield return new WaitForSeconds(0.02f);

                multi = GameObject.Find("MultiplierPanel");
                combo = GameObject.Find("ComboPanel");
                score = GameObject.Find("ScorePanel");
            }

            Console.WriteLine("[NoHUDPlus] Found base game objects. Initializing HUDHider.");

            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            Console.WriteLine("[NoHUDPlus] Initialized.");

            yield return new WaitForSeconds(0.05f);
            
            energy = GameObject.Find("EnergyPanel");
            failText = GameObject.Find("LevelFailedTextEffect");
            progress = GameObject.Find("SongProgressPanel");

            fcdisplay_ring = GameObject.Find("FCRing");
            tweaks_time = GameObject.Find("Clock Canvas");
            
            Camera mainCamera = FindObjectsOfType<Camera>().FirstOrDefault(x => x.CompareTag("MainCamera"));
            Camera livCamera = FindObjectsOfType<Camera>().FirstOrDefault(x => x.name == "LIV External Camera");
            var baseMask = mainCamera.cullingMask;

            if (ModPrefs.GetBool("NoHUDPlus", "HMDEnabled", false, true))
                mainCamera.cullingMask &= ~(1 << 26);
            else
                mainCamera.cullingMask |= (1 << 26);
            
            foreach (var pl in IllusionInjector.PluginManager.Plugins.Where(p => Plugin.cameraplugins.Contains(p.Name)))
            {
                MonoBehaviour camPlus = Util.ReflectionUtil.GetPrivateField<MonoBehaviour>(pl, "_cameraPlus");
                while (camPlus == null)
                {
                    yield return new WaitForEndOfFrame();
                    camPlus = Util.ReflectionUtil.GetPrivateField<MonoBehaviour>(pl, "_cameraPlus");
                }

                Camera cam = Util.ReflectionUtil.GetPrivateField<Camera>(camPlus, "_cam");
                if (cam != null)
                {
                    if (ModPrefs.GetBool("NoHUDPlus", "MirrorEnabled", false, true))
                        cam.cullingMask &= ~(1 << 26);
                    else
                        cam.cullingMask |= (1 << 26);
                }
                break;
            }

            if (livCamera != null)
            {
                var liv = livCamera.GetComponent<LIV.SDK.Unity.LIV>();
                liv.SpectatorLayerMask = baseMask;
                if (ModPrefs.GetBool("NoHUDPlus", "MirrorEnabled", false, true))
                    liv.SpectatorLayerMask &= ~(1 << 26);
                else
                    liv.SpectatorLayerMask |= (1 << 26);
            }
            
            multi.layer = 26;
            foreach (Transform c in multi.transform) c.gameObject.layer = 26;

            combo.layer = 26;
            foreach (Transform c in combo.transform) c.gameObject.layer = 26;

            score.layer = 26;
            foreach (Transform c in score.transform) c.gameObject.layer = 26;

            if (progress != null)
            {
                progress.layer = 26;
                foreach (Transform c in progress.transform) c.gameObject.layer = 26;
            }

            if (energy != null)
            {
                energy.layer = 26;
                foreach (Transform c in energy.transform) c.gameObject.layer = 26;
            }
            if (failText != null) failText.layer = 26;
            
            if (fcdisplay_ring != null)
            {
                fcdisplay_ring.layer = 26;
                foreach (Transform c in fcdisplay_ring.transform) c.gameObject.layer = 26;
            }

            if (tweaks_time != null) tweaks_time.layer = 26;

            if (Plugin.CountersPlusInstalled) HideCountersPlus();
            
            Console.WriteLine("[NoHUDPlus] Applied hidden layer (26) to game objects.");
        }

        private void HideCountersPlus()
        {
            foreach (GameObject g in CountersController.loadedCounters)
            {
                g.layer = 26;
                for (int i = 0; i < g.transform.childCount; i++)
                    g.transform.GetChild(i).gameObject.layer = 26;
            }
        }
    }
}

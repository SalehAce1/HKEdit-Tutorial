using System;
using System.Diagnostics;
using System.Reflection;
using Modding;
using JetBrains.Annotations;
using MonoMod.RuntimeDetour;
using UnityEngine.SceneManagement;
using UnityEngine;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UObject = UnityEngine.Object;
using System.Collections.Generic;
using System.IO;

namespace SceneTutorial
{
    [UsedImplicitly]
    public class SceneTutorial : Mod, ITogglableMod
    {
        private static AssetBundle Bundle;
        public static Dictionary<string, GameObject> ReplaceAssets;
        public static GameObject blur;
        public static GameObject sm;
        public override string GetVersion() => "0.0.0.0";

        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("Abyss_09","BlurPlane"),
                ("Abyss_06_Core","_SceneManager"),
                ("Abyss_09","Ruins Fossil"),
                ("Abyss_09","tinker lite"),
                ("Abyss_09","Surface Water Region"),
                ("Abyss_09", "abyss_black-water")
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initalizing.");
            ReplaceAssets = new Dictionary<string, GameObject>();
            blur = preloadedObjects["Abyss_09"]["BlurPlane"];
            sm = preloadedObjects["Abyss_06_Core"]["_SceneManager"];
            ReplaceAssets["water_region"] = preloadedObjects["Abyss_09"]["Surface Water Region"];
            ReplaceAssets["shell"] = preloadedObjects["Abyss_09"]["Ruins Fossil"];
            ReplaceAssets["tink"] = preloadedObjects["Abyss_09"]["tinker lite"];
            ReplaceAssets["water"] = preloadedObjects["Abyss_09"]["abyss_black-water"];
            Unload();
            ModHooks.Instance.AfterSavegameLoadHook += AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook += AddComponent;

            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string res in asm.GetManifestResourceNames())
            {
                using (Stream s = asm.GetManifestResourceStream(res))
                {
                    if (s == null) continue;
                    string bundleName = Path.GetExtension(res).Substring(1);
                    if (bundleName != "newbund") continue;
                    Bundle = AssetBundle.LoadFromStream(s);  
                }
            }
        }

        private void AfterSaveGameLoad(SaveGameData data) => AddComponent();

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<LoadScene>();
        }

        public void Unload()
        {
            ModHooks.Instance.AfterSavegameLoadHook -= AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook -= AddComponent;

            // ReSharper disable once Unity.NoNullPropogation
            var x = GameManager.instance?.gameObject.GetComponent<LoadScene>();
            if (x == null) return;
            UObject.Destroy(x);
        }
    }
}
using System.Collections;
using System.Linq;
using System.Reflection;
using GlobalEnums;
using UnityEngine;
using ModCommon;
using ModCommon.Util;
using Modding;

namespace SceneTutorial
{
    public class LoadScene : MonoBehaviour
    {
        private void Start()
        {
            // When we get to our scene, replace the SceneManager and other gameobjects.
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (arg0, scene1) =>
            {
                if (scene1.name == "TestAbyss")
                {
                    Destroy(GameObject.Find("_SceneManager"));
                    GameObject s = Instantiate(SceneTutorial.sm);
                    s.name = "_SceneManager";
                    s.SetActive(true);
                    ReplaceGameObjects();
                }
            };
            
            // Runs whenever the player transitions to a different scene
            On.GameManager.BeginSceneTransitionRoutine += GameManagerOnBeginSceneTransitionRoutine;
        }

        private IEnumerator GameManagerOnBeginSceneTransitionRoutine(On.GameManager.orig_BeginSceneTransitionRoutine orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            // If going to Abyss_09, go to TestAbyss instead.
            info.SceneName = info.SceneName == "Abyss_09" ? "TestAbyss" : info.SceneName;
            yield return orig(self, info);
            
            if (info.SceneName != "TestAbyss") yield break;
            
            GameObject blur = null;
            // Wait until BlurPlane is found
            yield return new WaitWhile(() => !(blur = GameObject.Find("BlurPlane")));
            // Destroy it and create a new BlurPlane from our Preloaded object
            Destroy(blur);
            blur = Instantiate(SceneTutorial.blur);
            blur.SetActive(true);
            blur.transform.position = new Vector3(111.5f, 65.3f, 10.9f);
            blur.transform.localRotation = Quaternion.Euler(-180f, -90f, 90f);
            blur.transform.localScale = new Vector3(11.48f, 11.48f, 25.01f);
        }

        private void ReplaceGameObjects()
        {
            // Replace breakable shells with original ones. 
            foreach (GameObject i in FindObjectsOfType<GameObject>()
                .Where(x => x.name.Contains("Ruins Fossil")))
            {
                GameObject shell = Instantiate(SceneTutorial.ReplaceAssets["shell"]);
                shell.transform.position = i.transform.position;
                shell.transform.localRotation = i.transform.localRotation;
                shell.transform.localScale = i.transform.localScale;
                shell.name = i.name;
                shell.SetActive(true);
                Destroy(i);
            }

            // Replace the tink effect
            TinkEffect orig = SceneTutorial.ReplaceAssets["tink"].GetComponent<TinkEffect>();
            foreach (TinkEffect i in FindObjectsOfType<TinkEffect>())
            {
                i.blockEffect = orig.blockEffect;
            }
            
            // Replace the water colliders
            foreach (GameObject go in FindObjectsOfType<GameObject>()
                .Where(x => x.name.Contains("Surface Water")))
            {
                GameObject surface = Instantiate(SceneTutorial.ReplaceAssets["water_region"]);
                surface.transform.position = go.transform.position;
                surface.transform.localScale = go.transform.localScale;
                surface.transform.rotation = go.transform.rotation;
                surface.SetActive(true);
                Destroy(go);
            }

            // Replace the void water to fix the splash effect
            GameObject water = Instantiate(SceneTutorial.ReplaceAssets["water"]);
            GameObject waterOrig = GameObject.Find("abyss_black-water");
            water.transform.position = waterOrig.transform.position;
            water.transform.rotation = waterOrig.transform.rotation;
            water.transform.localScale = waterOrig.transform.localScale;
            water.name = waterOrig.name;
            water.SetActive(true);
            Destroy(waterOrig);
        }
    }
}
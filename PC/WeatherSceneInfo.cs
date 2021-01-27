using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Weather
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public class WeatherSceneInfo : MonoBehaviour
    {
        public static Scene CurrentScene;
        public static int effects = 0;
        private static bool hasFullSetRefs = false;

        public async void SetRefs()
        {
            hasFullSetRefs = true;
            Plugin.Log.Info("SetRefs " + CurrentScene.name);
            WeatherDataRoot weatherInfo = await WeatherFinder.GetWeatherData();
            MeshRenderer[] mrs = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            //Plugin.Log.Info("SetRefs 1");
            if (CurrentScene.name == Plugin.menu) BundleLoader.WeatherPrefab.SetActive(PluginConfig.Instance.enabledInMenu);
            if (CurrentScene.name == Plugin.game) BundleLoader.WeatherPrefab.SetActive(PluginConfig.Instance.enabledInGameplay);
            BundleLoader.effects.Clear();
            for (int x = 0; x < weatherInfo.weather.Length; x++)
            {
                WeatherData data = weatherInfo.weather[x];
                EffectModel.EnableEffect(data.main, true);
            }
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                //Plugin.Log.Info(i.ToString());
                Transform Child = gameObject.transform.GetChild(i);
                Child.gameObject.SetActive(true);

                EffectDiscriptor efd = Child.gameObject.GetComponent<EffectDiscriptor>();
                string NameToUse = EffectModel.GetNameWithoutSceneName(efd.EffectName);
                Child.gameObject.GetComponentsInChildren<AudioSource>().ToList().ForEach((AudioSource s) => { s.volume = PluginConfig.Instance.audioSFXVolume; });
                efd.gameObject.SetActive(true);
                efd.transform.GetChild(0).gameObject.SetActive(true);
                Effect eff = new Effect(efd, Child.gameObject, PluginConfig.Instance.enabledEffects.Any((string str) => { return str == efd.EffectName; }));
                
                if (MiscConfig.hasObject(NameToUse))
                {
                    //Plugin.Log.Info("Misc Config has Object! " + NameToUse);
                    MiscConfigObject Object = MiscConfig.ReadObject(NameToUse);
                    eff.showInMenu = Object.showInMenu;
                    eff.showInGame = Object.showInGame;
                }
                else
                {
                    MiscConfig.Add(new MiscConfigObject(NameToUse, eff.showInMenu, eff.showInGame));
                    MiscConfig.Write();
                }
                eff.SetActiveRefs();
                BundleLoader.effects.Add(eff);
                //Plugin.Log.Info("Replacing " + mrs.Length.ToString    ());
                for (int x = 0; x < mrs.Length; x++)
                {
                    MeshRenderer mr = mrs[x];
                    if (mr.material.name.Contains("Note") || mr.gameObject.name.Contains("building") || mr.gameObject.name.Contains("speaker"))
                    {
                        eff.TrySetNoteMateral(mr);
                    }
                    else continue;
                }
            }
        }

        public void SetActiveRefs()
        {
            if (!hasFullSetRefs) { SetRefs(); return; }
            MeshRenderer[] mrs = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            for (int i = 0; i < BundleLoader.effects.Count; i++)
            {
                Effect eff = BundleLoader.effects[i];
                string NameToUse = EffectModel.GetNameWithoutSceneName(eff.Desc.EffectName);
                if (MiscConfig.hasObject(NameToUse))
                {
                    //Plugin.Log.Info("Misc Config has Object! " + NameToUse);
                    MiscConfigObject Object = MiscConfig.ReadObject(NameToUse);
                    eff.showInMenu = Object.showInMenu;
                    eff.showInGame = Object.showInGame;
                }
                else
                {
                    MiscConfig.Add(new MiscConfigObject(NameToUse, eff.showInMenu, eff.showInGame));
                }
                for (int x = 0; x < mrs.Length; x++)
                {
                    MeshRenderer mr = mrs[x];
                    if (mr.material.name.Contains("Note") || mr.gameObject.name.Contains("building") || mr.gameObject.name.Contains("speaker"))
                    {
                        eff.TrySetNoteMateral(mr);
                    }
                    else continue;
                }
                eff.SetActiveRefs();
            }
        }
    }
}
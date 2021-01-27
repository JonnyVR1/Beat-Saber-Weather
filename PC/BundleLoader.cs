using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
//Update this for each Descriptor update!

namespace Weather
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public static class BundleLoader
    {
        public static GameObject WeatherPrefab = null;
        public static List<Effect> effects = new List<Effect>();
        public static Sprite DefaultTex = null;
        
        public static void LoadDefaultCover()
        {
            DefaultTex = Utils.LoadSpriteFromResources("Weather.DefaultCover.png");
        }
        public static bool LoadFromFileAsync(string path, Action<AsyncOperation, string> callback)
        {
            if (File.Exists(path))
            {
                AssetBundleCreateRequest bundleAsync = AssetBundle.LoadFromFileAsync(path);
                bundleAsync.allowSceneActivation = true;
                Action<AsyncOperation> action = (AsyncOperation async) => { callback(async, path); };
                bundleAsync.completed += action;
            }
            else return false;
            return true;
        }
        public static void Load()
        {
            LoadDefaultCover();
            LoadWeatherFinder();
            WeatherPrefab = new GameObject("Weather", typeof(WeatherSceneInfo));    
            Object.DontDestroyOnLoad(WeatherPrefab);
            string EffectsPath = Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "Weather", "Effects");
            //Plugin.Log.Info(EffectsPath);

            if (!Directory.Exists(Path.GetDirectoryName(EffectsPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(EffectsPath));

            string[] Paths = Directory.GetFiles(EffectsPath, "*.effect");
            for (int i = 0; i < Paths.Length; i++)
            {
                string Path = Paths[i];
                //Plugin.Log.Info(Path);
                LoadFromFileAsync(Path, new Action<AsyncOperation, string>(LoadFromBundle));
            }
        }

        public static void LoadFromBundle(AsyncOperation bundleRequest, string filePath)
        {
            AssetBundle bundle = (bundleRequest as AssetBundleCreateRequest).assetBundle;
            TextAsset json = bundle.LoadAsset<TextAsset>("assets/effectJson.asset");
            Sprite Cover = null;
            string coverPath = "";
            foreach(string path in bundle.GetAllAssetNames())
            {
                if (path.StartsWith("assets/covers/"))
                    coverPath = path;
            }
            if(coverPath != "")
            {
                try
                {
                    Texture2D coverTex = bundle.LoadAsset<Texture2D>(coverPath);
                    Cover = Sprite.Create(coverTex, new Rect(0, 0, coverTex.width, coverTex.height), new Vector2(0, 0), 100f);
                }
                catch
                {
                    Cover = DefaultTex;
                }
            }
            else
                Cover = DefaultTex;
            
            GameObject Eff = Object.Instantiate(bundle.LoadAsset<GameObject>("assets/Effect.prefab"), WeatherPrefab.transform);
            Eff.SetActive(false);
            TempDesc_0_1_2 EfdTemp = null;
            try
            {
                EfdTemp = JsonUtility.FromJson<TempDesc_0_1_2>(json.text);
            }
            catch
            {
                try
                {
                    TempDesc_0_1_1 EfdOld1 = JsonUtility.FromJson<TempDesc_0_1_1>(json.text);
                    EfdTemp = new TempDesc_0_1_2(EfdOld1.Author, EfdOld1.EffectName, EfdOld1.WorksInMenu, true);
                }
                catch
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    Plugin.Log.Error(string.Format("{0} Failed To Load! Json: {1}", fileName, json.text));
                }
            }
            EffectDiscriptor Efd = Eff.AddComponent<EffectDiscriptor>();
            Efd.Author = EfdTemp.Author;
            Efd.EffectName = EfdTemp.EffectName;
            Efd.WorksInMenu = EfdTemp.WorksInMenu;
            Efd.WorksInGame = EfdTemp.WorksInGame;
            Efd.coverImage = Cover;
        }   

        async public static void LoadWeatherFinder()
        {
            WeatherDataRoot weatherData = await WeatherFinder.GetWeatherData();
        }
    }
}

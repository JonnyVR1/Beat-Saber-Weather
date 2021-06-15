using System;
using System.Collections.Generic;
using System.IO;
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
        public static GameObject WeatherPrefab;
        public static List<Effect> Effects = new List<Effect>();
        public static Sprite DefaultTex;
        
        public static void LoadDefaultCover()
        {
            DefaultTex = Utils.LoadSpriteFromResources("Weather.DefaultCover.png");
        }
        public static bool LoadFromFileAsync(string path, Action<AsyncOperation, string> callback)
        {
            if (File.Exists(path))
            {
                var bundleAsync = AssetBundle.LoadFromFileAsync(path);
                bundleAsync.allowSceneActivation = true;
                Action<AsyncOperation> action = async => { callback(async, path); };
                bundleAsync.completed += action;
            }
            else return false;
            return true;
        }
        public static void Load()
        {
            LoadDefaultCover();
            WeatherPrefab = new GameObject("Weather", typeof(WeatherSceneInfo));    
            Object.DontDestroyOnLoad(WeatherPrefab);
            var effectsPath = Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "Weather", "Effects");
            //Plugin.Log.Info(EffectsPath);

            if (!Directory.Exists(effectsPath))
                Directory.CreateDirectory(effectsPath);

            var paths = Directory.GetFiles(effectsPath, "*.effect");
            for (var i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                //Plugin.Log.Info(Path);
                LoadFromFileAsync(path, LoadFromBundle);
            }
        }

        public static void LoadFromBundle(AsyncOperation bundleRequest, string filePath)
        {
            var bundle = (bundleRequest as AssetBundleCreateRequest).assetBundle;
            var json = bundle.LoadAsset<TextAsset>("assets/effectJson.asset");
            Sprite cover = null;
            var coverPath = "";
            foreach(var path in bundle.GetAllAssetNames())
            {
                if (path.StartsWith("assets/covers/"))
                    coverPath = path;
            }
            if(coverPath != "")
            {
                try
                {
                    var coverTex = bundle.LoadAsset<Texture2D>(coverPath);
                    cover = Sprite.Create(coverTex, new Rect(0, 0, coverTex.width, coverTex.height), new Vector2(0, 0), 100f);
                }
                catch
                {
                    cover = DefaultTex;
                }
            }
            else
                cover = DefaultTex;
            
            var eff = Object.Instantiate(bundle.LoadAsset<GameObject>("assets/Effect.prefab"), WeatherPrefab.transform);
            eff.SetActive(false);
            TempDesc012 efdTemp = null;
            try
            {
                efdTemp = JsonUtility.FromJson<TempDesc012>(json.text);
            }
            catch
            {
                try
                {
                    var efdOld1 = JsonUtility.FromJson<TempDesc011>(json.text);
                    efdTemp = new TempDesc012(efdOld1.Author, efdOld1.EffectName, efdOld1.WorksInMenu, true);
                }
                catch
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    Plugin.Log.Error(string.Format("{0} Failed To Load! Json: {1}", fileName, json.text));
                }
            }
            var efd = eff.AddComponent<EffectDescriptor>();
            efd.author = efdTemp.Author;
            efd.effectName = efdTemp.EffectName;
            efd.worksInMenu = efdTemp.WorksInMenu;
            efd.worksInGame = efdTemp.WorksInGame;
            efd.coverImage = cover;
        }
    }
}

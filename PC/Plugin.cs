using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace Weather
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        private static bool hasEmptyTransitioned;

        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static ForecastFlowCoordinator _forecastFlowCoordinator { get; private set; }
        internal static string menu = "MenuViewControllers";
        internal static string game = "GameCore";
        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config)
        {
            Log = logger;
            Log.Info("Initializing");
            SceneManager.activeSceneChanged += SceneChanged;
            PluginConfig.Instance = config.Generated<PluginConfig>();
            Instance = this;
            MenuButtons.instance.RegisterButton(new MenuButton("Forecast", "See your Weather", new System.Action(LoadForCastUI)));
            PluginConfig.Instance.audioSFXVolume = Mathf.Clamp(PluginConfig.Instance.audioSFXVolume, 0f, 1f);
            MiscConfig.Read();
            var harmony = new Harmony("com.FutureMapper.Weather");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        void SceneChanged(Scene scene, Scene arg2)
        {
            Log.Info(scene.name + " " + arg2.name);
            if (arg2.name == "HealthWarning" && !hasEmptyTransitioned)
            {
                hasEmptyTransitioned = true;
                BundleLoader.Load();
            }
            if (!hasEmptyTransitioned && arg2.name == "EmptyTransition")
            {
                hasEmptyTransitioned = true;
                BundleLoader.Load();
            }
            if (arg2.name == menu)
            {
                Log.Debug(menu);
                MenuSceneActive();
            }
            if (arg2.name == game)
            {
                GameSceneActive();
            }
        }

        void MenuSceneActive()
        {
            if(PluginConfig.Instance.WeatherFinder.enabled)
                BundleLoader.LoadWeatherFinder();

            WeatherSceneInfo.CurrentScene = SceneManager.GetSceneByName(menu);
            BundleLoader.WeatherPrefab.GetComponent<WeatherSceneInfo>().SetActiveRefs();
        }
        void GameSceneActive()
        {
            if (PluginConfig.Instance.WeatherFinder.enabled)
                BundleLoader.LoadWeatherFinder();

            WeatherSceneInfo.CurrentScene = SceneManager.GetSceneByName(game);
            BundleLoader.WeatherPrefab.GetComponent<WeatherSceneInfo>().SetActiveRefs();
        }

        void LoadForCastUI()
        {
            if (_forecastFlowCoordinator == null)
            {
                _forecastFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ForecastFlowCoordinator>();
            }

            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_forecastFlowCoordinator);
        }
    }
}

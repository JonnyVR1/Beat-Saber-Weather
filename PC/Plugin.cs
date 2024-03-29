﻿using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using System.Reflection;
using JetBrains.Annotations;
using SiraUtil.Submissions;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace Weather
{
    [Plugin(RuntimeOptions.DynamicInit)]
    internal class Plugin
    {
        private static bool _hasEmptyTransitioned;
        internal static IPALogger Log { get; private set; }
        internal static Harmony Harmony { get; private set; }
        internal static IPA.Config.Config Config { get; private set; }
        internal static MenuButton MenuButton { get; private set; }
        private static ForecastFlowCoordinator ForecastFlowCoordinator { get; set; }
        private static readonly Submission _submission;
        internal const string Menu = "MainMenu";
        internal const string Game = "GameCore";

        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config)
        {
            Log = logger;
            Config = config;
            Log.Info("Initializing");
            MenuButton = new MenuButton("Forecast", "See your Weather", LoadForCastUI);
            Harmony = new Harmony("com.FutureMapper.Weather");
        }

        [UsedImplicitly]
        [OnEnable]
        public void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneChanged;
            PluginConfig.Instance = Config.Generated<PluginConfig>();
            MenuButtons.instance.RegisterButton(MenuButton);
            PluginConfig.Instance.AudioSfxVolume = Mathf.Clamp(PluginConfig.Instance.AudioSfxVolume, 0f, 1f);
            MiscConfig.Read();
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [UsedImplicitly]
        [OnDisable]
        public void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneChanged;
            MenuButtons.instance.UnregisterButton(MenuButton);
            Harmony.UnpatchSelf();
        }

        private void SceneChanged(Scene scene, Scene arg2)
        {
            Log.Info(scene.name + " " + arg2.name);
            if (arg2.name == "HealthWarning" && !_hasEmptyTransitioned)
            {
                _hasEmptyTransitioned = true;
                BundleLoader.Load();
            }

            if (!_hasEmptyTransitioned && arg2.name == "EmptyTransition")
            {
                _hasEmptyTransitioned = true;
                BundleLoader.Load();
            }

            switch (arg2.name)
            {
                case Menu:
                    Log.Debug(Menu);
                    MenuSceneActive();
                    break;
                case Game:
                    GameSceneActive();
                    break;
            }
        }

        private void MenuSceneActive()
        {
            WeatherSceneInfo.CurrentScene = SceneManager.GetSceneByName(Menu);
            BundleLoader.WeatherPrefab.GetComponent<WeatherSceneInfo>().SetActiveRefs();
        }

        private static void GameSceneActive()
        {
            WeatherSceneInfo.CurrentScene = SceneManager.GetSceneByName(Game);
            BundleLoader.WeatherPrefab.GetComponent<WeatherSceneInfo>().SetActiveRefs();
            
            var playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault();
            if (playerData.playerData.gameplayModifiers.ghostNotes)
            {
                DisableScore(true, "Ghost notes are enabled");
            }
        }

        private void LoadForCastUI()
        {
            if (ForecastFlowCoordinator == null)
            {
                ForecastFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ForecastFlowCoordinator>();
            }

            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(ForecastFlowCoordinator);
        }

        private static void DisableScore(bool disable, string reason)
        {
            if (!disable) return;
            _submission.DisableScoreSubmission("Weather", reason);
        }
    }
}

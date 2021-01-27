using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using IPA.Utilities;
using System.Collections.Generic;

namespace Weather
{
    internal class Forecast : BSMLResourceViewController
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);
        public ForecastFlowCoordinator flow = null;

        [UIComponent("Forecast")]
        public VerticalLayoutGroup forecast = null;
        public const bool showWeatherInfoMenu = false;

        [UIComponent("effectList")]
        public CustomListTableData customListTableData = null;
        List<Effect> effsInTable = new List<Effect>();
        [UIAction("effectSelect")]
        public void Select(TableView table, int row)
        {
            flow.ShowEffectSettings();
            flow._effectViewController.SetData(effsInTable[row]);
        }
        //Wasnt sure about how to go about this
        List<string> multiTypeAdded = new List<string>();
        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.tableView.ClearSelection();
            customListTableData.data.Clear();
            foreach (var effect in BundleLoader.effects)
            {   
                if(effect.IsEffectSeperateType())
                {
                    //Debug.Log(EffectModel.GetNameWithoutSceneName(effect.Desc.EffectName) + " is Seperate Type " + effect.Desc.EffectName);
                    if (multiTypeAdded.Contains(EffectModel.GetNameWithoutSceneName(effect.Desc.EffectName)))
                    {
                        //Debug.Log(EffectModel.GetNameWithoutSceneName(effect.Desc.EffectName) + " added... Skipping " + effect.Desc.EffectName);
                        continue;
                    }
                    else
                    {
                        //Debug.Log(EffectModel.GetNameWithoutSceneName(effect.Desc.EffectName) + " isnt added... Adding " + effect.Desc.EffectName);
                        multiTypeAdded.Add(EffectModel.GetNameWithoutSceneName(effect.Desc.EffectName));
                    }
                }
                effsInTable.Add(effect);
                var customCellInfo = new CustomListTableData.CustomCellInfo(EffectModel.GetNameWithoutSceneName(effect.Desc.EffectName), effect.Desc.Author, effect.Desc.coverImage);
                customListTableData.data.Add(customCellInfo);
                //if(effect.enabled)
                //{
                //    ReflectionUtil.GetField<HashSet<int>, TableView>(customListTableData.tableView, "_selectedCellIdxs").Add(customListTableData.data.Count-1);
                //    customListTableData.tableView.RefreshCells(true, false);
                //}
            }

            customListTableData.tableView.ReloadData();
            customListTableData.tableView.selectionType = TableViewSelectionType.Single;
        }

        public async void AddWeatherInfoMenu()
        {
            forecast.spacing = 0.01f;
            if (PluginConfig.Instance.showCityName)
            {
                var Header = BeatSaberUI.CreateText((RectTransform)forecast.transform, PluginConfig.Instance.WeatherFinder.cityName, Vector2.zero);
                Header.color = Color.cyan;
                Header.fontSize = 12f;
                Header.alignment = TMPro.TextAlignmentOptions.Center;
                ContentSizeFitter fitter = Header.gameObject.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            WeatherDataRoot weatherInfo = await WeatherFinder.GetWeatherData();
            for (int i = 0; i < weatherInfo.weather.Length; i++)
            {
                WeatherData data = weatherInfo.weather[i];
                GameObject forecastObj = new GameObject("ForecastObject");
                forecastObj.transform.SetParent(forecast.transform);
                HorizontalLayoutGroup horiz = forecastObj.AddComponent<HorizontalLayoutGroup>();
                horiz.childAlignment = TextAnchor.MiddleCenter;
                var text = BeatSaberUI.CreateText((RectTransform)forecastObj.transform, data.main + "    -    " + data.description, Vector2.zero);
                text.fontSize = 0.2f;
                text.alignment = TMPro.TextAlignmentOptions.Center;
                forecastObj.transform.localPosition = Vector3.zero;
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            customListTableData.tableView.ClearSelection();
            customListTableData.tableView.selectionType = TableViewSelectionType.Single;
            if (firstActivation)
            {
                //Visual Studio doesn't like me using an if statement with a const bool set to false (or true) "Unreachable code detected"
                #pragma warning disable
                if (showWeatherInfoMenu)
                    AddWeatherInfoMenu();
                #pragma warning restore
            }
        }
    }
}
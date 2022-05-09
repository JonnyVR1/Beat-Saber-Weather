using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Weather
{
    internal class Forecast : BSMLResourceViewController
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);
        public ForecastFlowCoordinator flow;

        [UIComponent("Forecast")]
        public VerticalLayoutGroup forecast;

        [UIComponent("effectList")]
        public CustomListTableData customListTableData;

        private readonly List<Effect> _effsInTable = new();
        [UIAction("effectSelect")]
        public void Select(TableView table, int row)
        {
            flow.ShowEffectSettings();
            flow.EffectViewController.SetData(_effsInTable[row]);
        }

        //Wasn't sure about how to go about this
        private readonly List<string> _multiTypeAdded = new();

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.tableView.ClearSelection();
            customListTableData.data.Clear();
            foreach (var effect in BundleLoader.Effects)
            {
                if (effect.IsEffectSeparateType())
                {
                    if (_multiTypeAdded.Contains(EffectModel.GetNameWithoutSceneName(effect.Desc.effectName)))
                    {
                        continue;
                    }

                    _multiTypeAdded.Add(EffectModel.GetNameWithoutSceneName(effect.Desc.effectName));
                }
                _effsInTable.Add(effect);
                var customCellInfo = new CustomListTableData.CustomCellInfo(EffectModel.GetNameWithoutSceneName(effect.Desc.effectName), effect.Desc.author, effect.Desc.coverImage);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();
            customListTableData.tableView.selectionType = TableViewSelectionType.Single;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            customListTableData.tableView.ClearSelection();
            customListTableData.tableView.selectionType = TableViewSelectionType.Single;
        }
    }
}
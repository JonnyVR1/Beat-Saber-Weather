using System;
using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Weather
{
    internal class EffectSettings : BSMLResourceViewController
    {
        // For this method of setting the ResourceName, this class must be the first class in the file.
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

        [UIComponent("effectName")]
        private TextMeshProUGUI text = null;

        [UIObject("Enabled")]
        private GameObject enabledToggle = null;
        [UIObject("ShowMenu")]
        private GameObject menuToggle = null;
        [UIObject("ShowGame")]
        private GameObject gameToggle = null;

        private Effect currectEff = null;

        private void EnabledToggleClicked(bool value)
        {
            EffectModel.EnableEffect(currectEff.Desc.EffectName, enabledToggle.GetComponentInChildren<Toggle>().isOn);
            if (value) currectEff.SetSceneMaterials();
            if (!value) currectEff.RemoveSceneMaterials();
        }
        private void GameToggleClicked(bool value)
        {
            Plugin.Log.Info("GameToggleClicked");
            currectEff.showInGame = value;
            currectEff.SetActiveRefs();
            MiscConfigObject configObject = MiscConfig.ReadObject(EffectModel.GetNameWithoutSceneName(currectEff.Desc.EffectName));
            configObject.showInGame = value;
            MiscConfig.WriteToObject(configObject);
        }
        private void MenuToggleClicked(bool value)
        {
            Plugin.Log.Info("MenuToggleClicked");
            currectEff.showInMenu = value;
            currectEff.SetActiveRefs();
            MiscConfigObject configObject = MiscConfig.ReadObject(EffectModel.GetNameWithoutSceneName(currectEff.Desc.EffectName));
            configObject.showInMenu = value;
            MiscConfig.WriteToObject(configObject);
            if (value) currectEff.SetSceneMaterials();
            if (!value) currectEff.RemoveSceneMaterials();
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if(firstActivation)
            {
                UnityAction<bool> enabledOnClicked = new UnityAction<bool>(EnabledToggleClicked);
                UnityAction<bool> gameOnClicked = new UnityAction<bool>(GameToggleClicked);
                UnityAction<bool> menuOnClicked = new UnityAction<bool>(MenuToggleClicked);
                enabledToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(enabledOnClicked);
                menuToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(menuOnClicked);
                gameToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(gameOnClicked);
            }
        }

        public void SetData(Effect eff)
        {
            currectEff = eff;
            string IndependentName = eff.GetNameWithoutSceneName();
            string Game = IndependentName + "Game";
            string Menu = IndependentName + "Menu";
            bool showGame = false;
            bool showMenu = false;
            if (eff.IsEffectSeperateType())
            {
                Plugin.Log.Info(IndependentName + " Is Independent Type");
                Effect effGame = EffectModel.GetEffectByName(Game);
                Effect effMenu = EffectModel.GetEffectByName(Menu);
                bool val = EffectModel.GetEffectEnabledByName(IndependentName);
                effGame.enabled = val;
                effMenu.enabled = val;
                showGame = effGame.Desc.WorksInGame;
                showMenu = effMenu.Desc.WorksInMenu;
            }
            else
            {
                Plugin.Log.Info(IndependentName + " Is Not Independent Type");
                eff.enabled = EffectModel.GetEffectEnabledByName(eff.Desc.EffectName);
                showGame = eff.Desc.WorksInGame;
                showMenu = eff.Desc.WorksInMenu;
            }
            text.text = IndependentName;    
            enabledToggle.GetComponentInChildren<Toggle>().isOn = EffectModel.GetEffectEnabledByName(eff.Desc.EffectName);
            Toggle menu = menuToggle.GetComponentInChildren<Toggle>();
            Toggle game = gameToggle.GetComponentInChildren<Toggle>();
            menu.interactable = showMenu;
            game.interactable = showGame;
            menu.isOn = MiscConfig.ReadObject(IndependentName).showInMenu;
            game.isOn = MiscConfig.ReadObject(IndependentName).showInGame;
        }
    }
}

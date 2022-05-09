using BeatSaberMarkupLanguage.Attributes;
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
        private readonly TextMeshProUGUI text = null;

        [UIObject("Enabled")]
        private readonly GameObject enabledToggle = null;
        [UIObject("ShowMenu")]
        private readonly GameObject menuToggle = null;
        [UIObject("ShowGame")]
        private readonly GameObject gameToggle = null;

        private Effect currentEffect;

        private void EnabledToggleClicked(bool value)
        {
            EffectModel.EnableEffect(currentEffect.Desc.effectName, enabledToggle.GetComponentInChildren<Toggle>().isOn);
            if (value) currentEffect.SetSceneMaterials();
            if (!value) currentEffect.RemoveSceneMaterials();
        }

        private void GameToggleClicked(bool value)
        {
            Plugin.Log.Info("GameToggleClicked");
            currentEffect.ShowInGame = value;
            currentEffect.SetActiveRefs();
            var configObject = MiscConfig.ReadObject(EffectModel.GetNameWithoutSceneName(currentEffect.Desc.effectName));
            configObject.ShowInGame = value;
            MiscConfig.WriteToObject(configObject);
        }

        private void MenuToggleClicked(bool value)
        {
            Plugin.Log.Info("MenuToggleClicked");
            currentEffect.ShowInMenu = value;
            currentEffect.SetActiveRefs();
            var configObject = MiscConfig.ReadObject(EffectModel.GetNameWithoutSceneName(currentEffect.Desc.effectName));
            configObject.ShowInMenu = value;
            MiscConfig.WriteToObject(configObject);
            if (value) currentEffect.SetSceneMaterials();
            if (!value) currentEffect.RemoveSceneMaterials();
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if(firstActivation)
            {
                var enabledOnClicked = new UnityAction<bool>(EnabledToggleClicked);
                var gameOnClicked = new UnityAction<bool>(GameToggleClicked);
                var menuOnClicked = new UnityAction<bool>(MenuToggleClicked);
                enabledToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(enabledOnClicked);
                menuToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(menuOnClicked);
                gameToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(gameOnClicked);
            }
        }

        public void SetData(Effect eff)
        {
            currentEffect = eff;
            var independentName = eff.GetNameWithoutSceneName();
            var game = independentName + "Game";
            var menu = independentName + "Menu";
            bool showGame;
            bool showMenu;
            if (eff.IsEffectSeparateType())
            {
                Plugin.Log.Info(independentName + " Is Independent Type");
                var effGame = EffectModel.GetEffectByName(game);
                var effMenu = EffectModel.GetEffectByName(menu);
                var val = EffectModel.GetEffectEnabledByName(independentName);
                effGame.Enabled = val;
                effMenu.Enabled = val;
                showGame = effGame.Desc.worksInGame;
                showMenu = effMenu.Desc.worksInMenu;
            }
            else
            {
                Plugin.Log.Info(independentName + " Is Not Independent Type");
                eff.Enabled = EffectModel.GetEffectEnabledByName(eff.Desc.effectName);
                showGame = eff.Desc.worksInGame;
                showMenu = eff.Desc.worksInMenu;
            }

            text.text = independentName;    
            enabledToggle.GetComponentInChildren<Toggle>().isOn = EffectModel.GetEffectEnabledByName(eff.Desc.effectName);
            var menuToggleComponent = menuToggle.GetComponentInChildren<Toggle>();
            var gameToggleComponent = gameToggle.GetComponentInChildren<Toggle>();
            menuToggleComponent.interactable = showMenu;
            gameToggleComponent.interactable = showGame;
            menuToggleComponent.isOn = MiscConfig.ReadObject(independentName).ShowInMenu;
            gameToggleComponent.isOn = MiscConfig.ReadObject(independentName).ShowInGame;
        }
    }
}

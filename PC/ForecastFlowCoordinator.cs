using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Weather
{
	internal class ForecastFlowCoordinator : FlowCoordinator
	{
		internal Forecast _forecastViewController;
		internal EffectSettings _effectViewController;

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			_forecastViewController = BeatSaberUI.CreateViewController<Forecast>();
			_effectViewController = BeatSaberUI.CreateViewController<EffectSettings>();
			try
			{
				if (firstActivation)
				{
					SetTitle("Forecast");
					showBackButton = true;
					_forecastViewController.flow = this;
					ProvideInitialViewControllers(_forecastViewController);
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.Error(ex);
			}
		}

		protected override void BackButtonWasPressed(ViewController _)
		{
			// Dismiss ourselves
			BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
		}

		public void ShowEffectSettings()
		{
			SetRightScreenViewController(_effectViewController, ViewController.AnimationType.In);
		}
	}
}

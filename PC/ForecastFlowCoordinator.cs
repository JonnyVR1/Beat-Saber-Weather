using BeatSaberMarkupLanguage;
using HMUI;
using System;

namespace Weather
{
	internal class ForecastFlowCoordinator : FlowCoordinator
	{
		private Forecast forecastViewController;
		internal EffectSettings EffectViewController;

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			forecastViewController = BeatSaberUI.CreateViewController<Forecast>();
			EffectViewController = BeatSaberUI.CreateViewController<EffectSettings>();

			try
			{
				if (!firstActivation) return;

				SetTitle("Forecast");
				showBackButton = true;
				forecastViewController.flow = this;
				ProvideInitialViewControllers(forecastViewController);
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
			SetRightScreenViewController(EffectViewController, ViewController.AnimationType.In);
		}
	}
}

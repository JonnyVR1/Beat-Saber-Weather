using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;

namespace Weather
{
    public class WeatherFinderConfig
    {
        
        public virtual bool enabled { get; set; } = false;
       
        public virtual string apiKey { get; set; } = "None";
        public virtual string cityName { get; set; } = "MyCity";
        public WeatherFinderConfig()
        {
        }
    }
    public class PluginConfig
    {
        
        public static PluginConfig Instance = null;
        [NonNullable, UseConverter(typeof(ListConverter<string>))]
        public virtual List<string> enabledEffects { get; set; } = new List<string>();
        public virtual bool showCityName { get; set; } = true;
        
        public virtual float audioSFXVolume { get; set; } = 1f;
        public virtual bool enabledInMenu { get; set; } = true;
        public virtual bool enabledInGameplay { get; set; } = true;

        public virtual WeatherFinderConfig WeatherFinder { get; set; } = new WeatherFinderConfig();

        public void AddEffect(string name)
        {
            if(!enabledEffects.Contains(name))
            {
                enabledEffects.Add(name);
            }
        }
    }
}

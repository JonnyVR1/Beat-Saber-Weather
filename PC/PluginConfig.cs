using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace Weather
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class PluginConfig
    {
        
        public static PluginConfig Instance = null;
        [NonNullable, UseConverter(typeof(ListConverter<string>))]
        public virtual List<string> EnabledEffects { get; set; } = new List<string>();
        public virtual bool ShowCityName { get; set; } = true;
        
        public virtual float AudioSfxVolume { get; set; } = 1f;
        public virtual bool EnabledInMenu { get; set; } = true;
        public virtual bool EnabledInGameplay { get; set; } = true;

        public void AddEffect(string name)
        {
            if(!EnabledEffects.Contains(name))
            {
                EnabledEffects.Add(name);
            }
        }
    }
}

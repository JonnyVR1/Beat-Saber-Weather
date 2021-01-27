using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
    static class EffectModel
    {
        public static Effect GetEffectByName(string name)
        {
            Effect Out = null;
            foreach (Effect e in BundleLoader.effects)
            {
                if (e.Desc.EffectName == name) Out = e;
            }
            return Out;
        }
        public static bool GetEffectEnabledByName(string name)
        {
            return PluginConfig.Instance.enabledEffects.Contains(GetNameWithoutSceneName(name));
        }
        //Idk what to name this but basically this says if it should be seperated and have Game/Menu removed
        public static bool IsEffectSeperateType(string name)
        {
            Plugin.Log.Debug("{IsEffectSeperateType} " + name + (name.EndsWith("Menu") || name.EndsWith("Game")).ToString() );
            return name.EndsWith("Menu") || name.EndsWith("Game");
        }
        public static bool IsEffectSeperateType(this Effect eff) => IsEffectSeperateType(eff.Desc.EffectName);
        //Idk what to name this either but it gets the name without scene seperation so RainMenu->Rain or RainGame->Rain
        public static string GetNameWithoutSceneName(string name)
        {
            if(IsEffectSeperateType(name))
            {
                //Game
                string New = name.Substring(0, name.Length-4);
                return New;
            }
            return name;
        }
        public static string GetNameWithoutSceneName(this Effect eff) => GetNameWithoutSceneName(eff.Desc.EffectName);

        public static void EnableEffect(string name, bool Value)
        {
            string NewName = GetNameWithoutSceneName(name);
            //Plugin.Log.Info(NewName + " " + name);
            string Game = NewName + "Game";
            string Menu = NewName + "Menu";
            if(IsEffectSeperateType(name))
            {
                Plugin.Log.Info(NewName + " Is Independent " + name);
                Effect effGame = GetEffectByName(Game);
                Effect effMenu = GetEffectByName(Menu);
                effGame.enabled = Value;
                effMenu.enabled = Value;
                
                if (Value)
                    PluginConfig.Instance.AddEffect(NewName);
                else
                    PluginConfig.Instance.enabledEffects.Remove(NewName);
                
                effGame.SetActiveRefs();
                effMenu.SetActiveRefs();

                return;
            }
            Effect eff = GetEffectByName(name);
            if (eff == null) return;
            eff.enabled = Value;
            
            if (Value)
                PluginConfig.Instance.AddEffect(name);
            else
                PluginConfig.Instance.enabledEffects.Remove(name);
            eff.SetActiveRefs();
        }
    }
}

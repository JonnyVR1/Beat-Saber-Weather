namespace Weather
{
    internal static class EffectModel
    {
        public static Effect GetEffectByName(string name)
        {
            Effect @out = null;
            foreach (var e in BundleLoader.Effects)
            {
                if (e.Desc.effectName == name) @out = e;
            }

            return @out;
        }

        public static bool GetEffectEnabledByName(string name)
        {
            return PluginConfig.Instance.EnabledEffects.Contains(GetNameWithoutSceneName(name));
        }

        //Idk what to name this but basically this says if it should be seperated and have Game/Menu removed
        private static bool IsEffectSeparateType(string name)
        {
            if (name == null) return false;
            Plugin.Log.Debug("{IsEffectSeparateType} " + name + (name.EndsWith("Menu") || name.EndsWith("Game")) );
            return name.EndsWith("Menu") || name.EndsWith("Game");
        }

        public static bool IsEffectSeparateType(this Effect eff) => IsEffectSeparateType(eff.Desc.effectName);

        //Idk what to name this either but it gets the name without scene separation so RainMenu->Rain or RainGame->Rain
        public static string GetNameWithoutSceneName(string name)
        {
            if (!IsEffectSeparateType(name)) return name;

            //Game
            var @new = name.Substring(0, name.Length-4);
            return @new;
        }

        public static string GetNameWithoutSceneName(this Effect eff) => GetNameWithoutSceneName(eff.Desc.effectName);

        public static void EnableEffect(string name, bool value)
        {
            var newName = GetNameWithoutSceneName(name);
            var game = newName + "Game";
            var menu = newName + "Menu";

            if (IsEffectSeparateType(name))
            {
                Plugin.Log.Info(newName + " Is Independent " + name);
                var effGame = GetEffectByName(game);
                var effMenu = GetEffectByName(menu);
                effGame.Enabled = value;
                effMenu.Enabled = value;

                if (value)
                {
                    PluginConfig.Instance.AddEffect(newName);
                }
                else
                {
                    PluginConfig.Instance.EnabledEffects.Remove(newName);
                }
                
                effGame.SetActiveRefs();
                effMenu.SetActiveRefs();

                return;
            }

            var eff = GetEffectByName(name);
            if (eff == null) return;
            eff.Enabled = value;

            if (value)
            {
                PluginConfig.Instance.AddEffect(name);
            }
            else
            {
                PluginConfig.Instance.EnabledEffects.Remove(name);
            }

            eff.SetActiveRefs();
        }
    }
}

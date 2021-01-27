#include "include/EffectModel.hpp"
#include "include/Logger.hpp"
#include "include/Config.hpp"

bool Weather::EffectModel::GetEffectEnabledByName(Il2CppString* name)
{
    std::string key = GetNameWithoutSceneName(name);
    bool found = false;
    for (auto const &e: getConfig().config["enabledEffects"].GetArray()) {
        GenericLogger::Log( e.GetString());
        if ( e.GetString() == key) {
            found = true;
            std::string e2 = e.GetString();
            GenericLogger::Log("Found! " + e2 + " " + key);
            break;
        }
    }
    return found;
}
bool hasEnding (std::string fullString, std::string ending) {
    if (fullString.length() >= ending.length()) {
        return (0 == fullString.compare (fullString.length() - ending.length(), ending.length(), ending));
    } else {
        return false;
    }
}

bool Weather::EffectModel::IsEffectSeperateType(std::string name)
{
    return hasEnding(name, "Menu") || hasEnding(name, "Game");
}

bool Weather::EffectModel::IsEffectSeperateType(Il2CppString* name)
{
    std::string str = to_utf8(csstrtostr(name));
    return IsEffectSeperateType(str);
}

std::string Weather::EffectModel::GetNameWithoutSceneName(std::string name)
{
    if(IsEffectSeperateType(name))
    {
        return name.substr(0, name.length()-4);
    }
    return name;
}

std::string Weather::EffectModel::GetNameWithoutSceneName(Il2CppString* name)
{
    std::string convert = to_utf8(csstrtostr(name));
    return GetNameWithoutSceneName(convert);
}

/*void Weather::EffectModel::EnableEffect(Il2CppString* name, bool Value)
{
    std::string NewName = GetNameWithoutSceneName(name);
    std::string Game = NewName + "Game";
    std::string Menu = NewName + "Menu";
    if(IsEffectSeperateType(name))
    {
        Weather::Effect* effGame = GetEffectByName(Game);
        Weather::Effect* effMenu = GetEffectByName(Menu);
        effGame->enabled = Value;
        effMenu->enabled = Value;
        
        if (Value)
        {
            Config.enabledEffects.insert(NewName);
            getConfig().Write();
        }
        else
        {
            Config.enabledEffects.erase(NewName);
            getConfig().Write();
        }
        
        effGame.SetActiveRefs();
        effMenu.SetActiveRefs();

        return;
    }

}*/
#include "beatsaber-hook/shared/utils/utils.h"
#include "beatsaber-hook/shared/utils/il2cpp-utils.hpp"
#include "beatsaber-hook/shared/config/config-utils.hpp"

#include "custom-types/shared/register.hpp"
#include "custom-types/shared/types.hpp"

#include "Logger.hpp"
#include "Config.hpp"
#include "EffectDescriptor.hpp"
#include "BundleLoader.hpp"
#include "Effect.hpp"
#include "WeatherSceneInfo.hpp"
#include "codegen/include/UnityEngine/SceneManagement/Scene.hpp"
#include "codegen/include/UnityEngine/GameObject.hpp"
#include "codegen/include/UnityEngine/Transform.hpp"
#include "codegen/include/UnityEngine/SceneManagement/SceneManager.hpp"
#include "codegen/include/UnityEngine/SceneManagement/LoadSceneMode.hpp"
#include "codegen/include/GlobalNamespace/GameNoteController.hpp"
#include "codegen/include/GlobalNamespace/SaberTrail.hpp"
#include "codegen/include/UnityEngine/MeshRenderer.hpp"
#include "codegen/include/UnityEngine/Material.hpp"
#include "codegen/include/GlobalNamespace/SaberTrailRenderer.hpp"

#include <iomanip>
#include <sstream>
#include <unordered_set>

using namespace UnityEngine::SceneManagement;
using namespace UnityEngine;
using namespace GlobalNamespace;

Configuration& getConfig() {
    static Configuration configuration(Weather::GenericLogger::modInfo);
    return configuration;
}

void SceneActive(UnityEngine::SceneManagement::Scene scene)
{
    Weather::WeatherSceneInfo::CurrentScene = scene;

    Weather::BundleLoader::SceneInfoRef->SetRefs();
}
MAKE_HOOK_OFFSETLESS(sceneLoaded, void, UnityEngine::SceneManagement::Scene oldScene, UnityEngine::SceneManagement::Scene scene) {
    sceneLoaded(oldScene, scene);
    
    std::string name = to_utf8(csstrtostr(scene.get_name()));
    Weather::GenericLogger::Log(name);
    if (name == "ShaderWarmup")
    {
        Weather::BundleLoader::Load();
        return;
    }
    if (name == "HealthWarning")
    {
        Weather::BundleLoader::LoadAssets();
    }
    if (name == "MenuViewControllers")
    {
        SceneActive(scene);
    }
    if (name == "GameCore")
    {
        SceneActive(scene);
    }
}

extern "C" void setup(ModInfo& info) {
    info.id = ID;
    info.version = VERSION;
    getConfig().Load();
}

void SaveConfig() {
    ConfigDocument& configDoc = getConfig().config;
    configDoc.RemoveAllMembers();
    configDoc.SetObject();
    auto& allocator = configDoc.GetAllocator();

    ConfigValue enabledEffects(rapidjson::kArrayType);
    for (auto& name : Config.enabledEffects) {
        ConfigValue value(name.c_str(), name.size());
        enabledEffects.PushBack(value, allocator);
    }
    configDoc.AddMember("enabledEffects", enabledEffects, allocator);
    configDoc.AddMember("enabledInMenu", Config.enabledInMenu, allocator);
    configDoc.AddMember("enabledInGameplay", Config.enabledInGameplay, allocator);
    configDoc.AddMember("enableShaderAdditions", Config.enableShaderAdditions, allocator);
    configDoc.AddMember("audioSFXVolume", Config.audioSFXVolume, allocator);

    getConfig().Write();
}

bool LoadConfig() {
    getConfig().Load();
    ConfigDocument& configDoc = getConfig().config;
    bool foundEverything = true;

    Config.enabledEffects.clear();
    if (configDoc.HasMember("enabledEffects") && configDoc["enabledEffects"].IsArray()) {
        for (rapidjson::SizeType i = 0; i < configDoc["enabledEffects"].Size(); i++) {
            if (configDoc["enabledEffects"][i].IsString()) {
                Config.enabledEffects.insert(configDoc["enabledEffects"][i].GetString());
                Weather::GenericLogger::Log(((std::string)configDoc["enabledEffects"][i].GetString()) + " Is Enabled");
            }
        }
    } else {
        foundEverything = false;
    }
    if (configDoc.HasMember("enabledInMenu") && configDoc["enabledInMenu"].IsBool()) {
        Config.enabledInMenu = configDoc["enabledInMenu"].GetBool();
    } else {
        foundEverything = false;
    }
    if (configDoc.HasMember("enabledInGameplay") && configDoc["enabledInGameplay"].IsBool()) {
        Config.enabledInGameplay = configDoc["enabledInGameplay"].GetBool();
    } else {
        foundEverything = false;
    }
    if (configDoc.HasMember("audioSFXVolume") && configDoc["audioSFXVolume"].IsFloat()) {
        Config.audioSFXVolume = configDoc["audioSFXVolume"].GetFloat();
    } else {
        foundEverything = false;
    }
    if (configDoc.HasMember("enableShaderAdditions") && configDoc["enableShaderAdditions"].IsBool()) {
        Config.enableShaderAdditions = configDoc["enableShaderAdditions"].GetBool();
    } else {
        foundEverything = false;
    }
    return foundEverything;
}
void ReloadConfig()
{
    LoadConfig();
}
extern "C" void load() {
    //Modloader::requireMod("Qosmetics");
    il2cpp_functions::Init();
    if (!LoadConfig()) SaveConfig();
    custom_types::Register::RegisterType<Weather::EffectDescTemp>();
    custom_types::Register::RegisterType<Weather::EffectDescriptor>();
    custom_types::Register::RegisterType<Weather::Effect>();
    custom_types::Register::RegisterType<Weather::WeatherSceneInfo>();
    INSTALL_HOOK_OFFSETLESS(Weather::GenericLogger::getLogger(), sceneLoaded, il2cpp_utils::FindMethodUnsafe("UnityEngine.SceneManagement", "SceneManager", "Internal_ActiveSceneChanged", 2));
}
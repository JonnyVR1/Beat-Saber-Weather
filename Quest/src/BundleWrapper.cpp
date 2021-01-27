#include "include/BundleWrapper.hpp"
#include "include/BundleLoader.hpp"
#include "include/EffectModel.hpp"
#include "include/EffectDescriptor.hpp"
#include "beatsaber-hook/shared/rapidjson/include/rapidjson/document.h"
#include "include/Logger.hpp"
#include "UnityEngine/Transform.hpp"
#include "UnityEngine/GameObject.hpp"
#include "UnityEngine/Object.hpp"
#include "UnityEngine/TextAsset.hpp"
#include "UnityEngine/Resources.hpp"

static Weather::EffectDescTemp* ParseJSON(std::string json)
{
    Weather::GenericLogger::Log("BundleWrapper::ParseJSON");
    Weather::GenericLogger::Log("Json: " + json);
    rapidjson::Document d;
    d.Parse(json.c_str());
    std::string author = d["Author"].GetString();
    std::string effectName = d["EffectName"].GetString();
    bool worksInMenu = d["WorksInMenu"].GetBool();
    bool worksInGame = d.HasMember("WorksInGame") ? d["WorksInGame"].GetBool() : true;
    Weather::GenericLogger::Log(author + ": " + effectName + " Loaded");
    Weather::EffectDescTemp* desc = CRASH_UNLESS(il2cpp_utils::New<Weather::EffectDescTemp*>());
    desc->WorksInGame = worksInGame;
    desc->WorksInMenu = worksInMenu;
    desc->EffectName = il2cpp_utils::createcsstr(effectName);
    desc->Author = il2cpp_utils::createcsstr(author);
    return desc;
}

void Weather::BundleWrapper::JSONLoaded()
{
    Weather::GenericLogger::Log("BundleWrapper::JSONLoaded");
    Weather::GenericLogger::Log("Json Null: " + std::to_string(this->json == nullptr));
    this->desc = ParseJSON(to_utf8(csstrtostr(this->json->get_text())));
    static auto Unload = reinterpret_cast<function_ptr_t<void, UnityEngine::Object*>>(il2cpp_functions::resolve_icall("UnityEngine.Resources::UnloadAsset"));
    Unload(this->json);
}

void Weather::BundleWrapper::PrefabLoaded(bs_utils::AssetBundle* b)
{
    if(b == nullptr)
    {
        Weather::GenericLogger::Log("Bundle Null!");
        return;
    }
    Weather::GenericLogger::Log("BundleWrapper::PrefabLoaded");
    UnityEngine::GameObject* effectObj = UnityEngine::Object::Instantiate<UnityEngine::GameObject*>(this->prefab, Weather::BundleLoader::WeatherPrefab->get_transform());
    Weather::EffectDescriptor* desc = effectObj->AddComponent<Weather::EffectDescriptor*>();
    desc->Author = this->desc->Author;
    desc->EffectName = this->desc->EffectName;
    desc->WorksInMenu = this->desc->WorksInMenu;
    desc->WorksInGame = this->desc->WorksInGame;
    this->descMain = desc;
    effectObj->SetActive(false);
    static auto Unload = reinterpret_cast<function_ptr_t<void, UnityEngine::Object*>>(il2cpp_functions::resolve_icall("UnityEngine.Resources::UnloadAsset"));
    Unload(this->prefab);
    ((UnityEngine::AssetBundle*)b)->Unload(false);
    FreeMe(this);
}

void Weather::BundleWrapper::LoadAssets()
{
    
    Weather::GenericLogger::Log("BundleWrapper::LoadAssets");
    auto b = this->bundle;
    if(b == nullptr)
    {
        Weather::GenericLogger::Log("Bundle Null!");
        return;
    }
    this->bundle->LoadAssetAsync("assets/effectJson.asset", [this](bs_utils::Asset* a){this->json   = reinterpret_cast<UnityEngine::TextAsset*>(a);  Weather::GenericLogger::Log("Loaded JSON"); this->JSONLoaded();}, il2cpp_utils::GetSystemType("UnityEngine", "TextAsset"));
    this->bundle->LoadAssetAsync("assets/Effect.prefab",    [this, b](bs_utils::Asset* a){this->prefab = reinterpret_cast<UnityEngine::GameObject*>(a); Weather::GenericLogger::Log("Loaded prefab"); this->PrefabLoaded(b);}, il2cpp_utils::GetSystemType("UnityEngine", "GameObject"));
}

void Weather::BundleWrapper::LoadFromBundle(bs_utils::AssetBundle* b)
{
    loaded = true;
    if(b == nullptr)
    {
        Weather::GenericLogger::Log("Bundle Null!");
        return;
    }
    Weather::GenericLogger::Log("Loaded A Bundle");
    this->bundle = b;   
}

void Weather::BundleWrapper::Load(Il2CppString* path)
{
    loaded = false;
    bs_utils::AssetBundle::LoadFromFileAsync(to_utf8(csstrtostr(path)), [this](bs_utils::AssetBundle* b){this->LoadFromBundle(b);});
}
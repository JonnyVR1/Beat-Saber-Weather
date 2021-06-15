#include "include/BundleLoader.hpp"
#include "include/BundleWrapper.hpp"
#include "include/EffectModel.hpp"
#include "include/Logger.hpp"
#include "include/WeatherSceneInfo.hpp"
#include "extern/bs-utils/shared/AssetBundle.hpp"
#include "codegen/include/System/Collections/Generic/List_1.hpp"
#include "System/IO/Directory.hpp"
#include "System/IO/Path.hpp"
#include "include/Logger.hpp"
#include "UnityEngine/AssetBundle.hpp"
#include "bs-utils/shared/utils.hpp"
#include <unistd.h>

static const std::string cpath = "Effects/";

using namespace UnityEngine;
using namespace System::IO;

UnityEngine::GameObject* Weather::BundleLoader::WeatherPrefab; 
bool Weather::BundleLoader::isInitialized; 
Weather::WeatherSceneInfo* Weather::BundleLoader::SceneInfoRef; 
std::vector<Weather::Effect*> Weather::BundleLoader::effects; 
std::vector<Weather::BundleWrapper*> Weather::BundleLoader::bundles; 

Weather::BundleWrapper* lastWrapper = nullptr;
Array<Il2CppString*>* paths;
int i = 0;
void BundleThread()
{
    if(lastWrapper != nullptr)
    {
        while(!lastWrapper->loaded)
        {
            usleep(1000);
        } 
    }
    Il2CppString* path = reinterpret_cast<Il2CppString*>(paths->values[i]);
    Weather::GenericLogger::Log("Bundle Index: " + std::to_string(i));
            
    Weather::BundleWrapper* wrapper = new Weather::BundleWrapper();
    lastWrapper = wrapper;
    wrapper->Load(path);
    Weather::BundleLoader::bundles.emplace_back(wrapper);
    i++;
}

void Weather::BundleLoader::Load()
{
    Weather::GenericLogger::Log("BundleLoader::Load");
    //LoadWeatherFinder();
    ModInfo info = ModInfo();
    info.id = ID;
    info.version = VERSION;
    std::string dataPath = bs_utils::getDataDir(info);
    WeatherPrefab = GameObject::New_ctor(il2cpp_utils::createcsstr("Weather"));
    Object::DontDestroyOnLoad(WeatherPrefab);
    SceneInfoRef = WeatherPrefab->AddComponent<WeatherSceneInfo*>();

    Il2CppString* searchPath = il2cpp_utils::createcsstr(dataPath + cpath);
    Il2CppString* search = il2cpp_utils::createcsstr("*.qeffect");
    searchPath = Path::GetDirectoryName(searchPath);

    if (!Directory::Exists(searchPath))
    {
        Weather::GenericLogger::Log("Creating Path: " + dataPath + cpath);
        Directory::CreateDirectory(searchPath);
    }

    paths = Directory::GetFiles(searchPath, search);
    Weather::GenericLogger::Log("Bundle Paths: " + std::to_string(paths->Length()));
    std::thread assetLoad(BundleThread);

    assetLoad.detach();
} 

void Weather::BundleLoader::LoadAssets()
{
    Weather::GenericLogger::Log("BundleLoader::LoadAssets");
    for(auto wrapper : bundles)
    {
        if(wrapper == nullptr)
        {
            Weather::GenericLogger::Log("Json Null!");
            return;
        }
        Weather::GenericLogger::Log("Loading Asset");
        wrapper->LoadAssets();
    }
} 
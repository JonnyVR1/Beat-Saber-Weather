#pragma once
#include "./BundleWrapper.hpp"
#include "./EffectDescriptor.hpp"
#include "./WeatherSceneInfo.hpp"
#include "./Effect.hpp"
#include "codegen/include/UnityEngine/GameObject.hpp"
#include "codegen/include/System/Collections/Generic/List_1.hpp"
#include <vector>
namespace Weather
{
    class BundleLoader
    {
        public:
        static UnityEngine::GameObject* WeatherPrefab; 
        static bool isInitialized; 
        static Weather::WeatherSceneInfo* SceneInfoRef;
        static std::vector<Weather::Effect*> effects; 
        static std::vector<Weather::BundleWrapper*> bundles; 
        static void Load();
        static void LoadAssets();
    };
}
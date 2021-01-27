#pragma once

#include "include/EffectModel.hpp"
#include "include/EffectDescriptor.hpp"
#include "include/WeatherSceneInfo.hpp"
#include "extern/bs-utils/shared/AssetBundle.hpp"
#include "codegen/include/System/Collections/Generic/List_1.hpp"
#include "System/IO/Directory.hpp"
#include "System/IO/Path.hpp"
#include "include/Logger.hpp"
#include "UnityEngine/AssetBundle.hpp"
#include "UnityEngine/TextAsset.hpp"

namespace Weather
{
    class BundleWrapper
    {
        public:
        UnityEngine::TextAsset* json;
        Weather::EffectDescTemp* desc;
        Weather::EffectDescriptor* descMain;
        UnityEngine::GameObject* prefab;
        bool loaded;
        bs_utils::AssetBundle* bundle;

        void LoadAssetFromBundle(bs_utils::AssetBundle* b);

        void LoadFromBundle(bs_utils::AssetBundle* b);

        void Load(Il2CppString* path);
        void LoadAssets();

        void JSONLoaded();

        void PrefabLoaded(bs_utils::AssetBundle* );
    };

    static void FreeMe(Weather::BundleWrapper* wrapper)
    {
        delete wrapper;
    }
}
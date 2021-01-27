#pragma once

#include "Effect.hpp"
#include "Config.hpp"
#include "codegen/include/UnityEngine/GameObject.hpp"
#include "codegen/include/System/Collections/Generic/List_1.hpp"
namespace Weather
{
    class EffectModel
    {
        public:
        static bool GetEffectEnabledByName(Il2CppString* name);
        //static void EnableEffect(Il2CppString* name, bool Value);
        static bool IsEffectSeperateType(Il2CppString* name);
        static std::string GetNameWithoutSceneName(Il2CppString* name);
        static bool IsEffectSeperateType(std::string name);
        static std::string GetNameWithoutSceneName(std::string name);
    };
}

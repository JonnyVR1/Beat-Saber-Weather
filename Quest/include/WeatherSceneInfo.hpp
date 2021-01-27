#pragma once

#include "extern/codegen/include/UnityEngine/MonoBehaviour.hpp"
#include "extern/codegen/include/UnityEngine/SceneManagement/Scene.hpp"
#include "extern/codegen/include/System/Object.hpp"
#include "extern/custom-types/shared/macros.hpp"
#include "extern/custom-types/shared/register.hpp"
#include "extern/custom-types/shared/types.hpp"
#include "codegen/include/UnityEngine/SceneManagement/Scene.hpp"
#include "codegen/include/UnityEngine/SceneManagement/SceneManager.hpp"

using namespace UnityEngine::SceneManagement;

DECLARE_CLASS_CODEGEN(Weather, WeatherSceneInfo, UnityEngine::MonoBehaviour,
    DECLARE_STATIC_FIELD(UnityEngine::SceneManagement::Scene, CurrentScene);
    DECLARE_STATIC_FIELD(bool, hasFullSetRefs);
    DECLARE_METHOD(void, SetRefs);
    DECLARE_METHOD(void, SetActiveRefs);
    REGISTER_FUNCTION(WeatherSceneInfo,
        REGISTER_FIELD(CurrentScene);
        REGISTER_FIELD(hasFullSetRefs);
        REGISTER_METHOD(SetRefs);
        REGISTER_METHOD(SetActiveRefs);
    )
)
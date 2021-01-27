#pragma once

#include "extern/codegen/include/UnityEngine/MonoBehaviour.hpp"
#include "extern/codegen/include/System/Object.hpp"
#include "extern/custom-types/shared/macros.hpp"
#include "extern/custom-types/shared/register.hpp"
#include "extern/custom-types/shared/types.hpp"

DECLARE_CLASS_CODEGEN(Weather, EffectDescriptor, UnityEngine::MonoBehaviour,
    DECLARE_INSTANCE_FIELD(Il2CppString*, Author);
    DECLARE_INSTANCE_FIELD(Il2CppString*, EffectName);
    DECLARE_INSTANCE_FIELD(bool, WorksInMenu);
    DECLARE_INSTANCE_FIELD(bool, WorksInGame);
    REGISTER_FUNCTION(EffectDescriptor,
        REGISTER_FIELD(Author);
        REGISTER_FIELD(EffectName);
        REGISTER_FIELD(WorksInMenu);
        REGISTER_FIELD(WorksInGame);
    )
)

DECLARE_CLASS_CODEGEN(Weather, EffectDescTemp, System::Object,
    DECLARE_INSTANCE_FIELD(Il2CppString*, Author);
    DECLARE_INSTANCE_FIELD(Il2CppString*, EffectName);
    DECLARE_INSTANCE_FIELD(bool, WorksInMenu);
    DECLARE_INSTANCE_FIELD(bool, WorksInGame);
    DECLARE_CTOR(New, Il2CppString* _Author, Il2CppString* _EffectName, bool _WorksInMenu, bool _WorksInGame);
    REGISTER_FUNCTION(EffectDescTemp,
        REGISTER_FIELD(Author);
        REGISTER_FIELD(EffectName);
        REGISTER_FIELD(WorksInMenu);
        REGISTER_FIELD(WorksInGame);
    )
)

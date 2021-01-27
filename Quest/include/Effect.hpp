#pragma once
#include "extern/codegen/include/System/Object.hpp"
#include "extern/codegen/include/UnityEngine/GameObject.hpp"
#include "extern/codegen/include/UnityEngine/MeshRenderer.hpp"
#include "extern/codegen/include/UnityEngine/Transform.hpp"
#include "./EffectDescriptor.hpp"

DECLARE_CLASS_CODEGEN(Weather, Effect, System::Object,
    DECLARE_INSTANCE_FIELD(Weather::EffectDescriptor*, Desc);
    DECLARE_INSTANCE_FIELD(UnityEngine::GameObject*, gameObject);
    DECLARE_INSTANCE_FIELD(bool, enabled);
    DECLARE_INSTANCE_FIELD(bool, showInMenu);
    DECLARE_INSTANCE_FIELD(bool, showInGame);
    DECLARE_INSTANCE_FIELD(UnityEngine::Transform*, Grab);

    DECLARE_CTOR(ctor, Weather::EffectDescriptor* _effectDescriptor, UnityEngine::GameObject* _gameObject, bool _enabled);
    DECLARE_METHOD(void, SetActiveRefs, bool force = false);
    DECLARE_METHOD(void, SetSceneMaterials);
    DECLARE_METHOD(void, RemoveSceneMaterials);
    DECLARE_METHOD(void, TrySetNoteMateral, UnityEngine::MeshRenderer* mr);
    REGISTER_FUNCTION(Effect,
        REGISTER_FIELD(Desc);
        REGISTER_FIELD(gameObject);
        REGISTER_FIELD(enabled);
        REGISTER_FIELD(showInMenu);
        REGISTER_FIELD(showInGame);
        REGISTER_FIELD(Grab);

        REGISTER_METHOD(ctor);
        REGISTER_METHOD(SetActiveRefs);
        REGISTER_METHOD(SetSceneMaterials);
        REGISTER_METHOD(RemoveSceneMaterials);
        REGISTER_METHOD(TrySetNoteMateral);
    )
)
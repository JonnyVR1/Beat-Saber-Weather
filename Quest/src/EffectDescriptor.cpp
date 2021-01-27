#include "Effect.hpp"
#include "EffectDescriptor.hpp"
#include "beatsaber-hook/shared/utils/il2cpp-utils.hpp"

using namespace UnityEngine;

DEFINE_CLASS(Weather::EffectDescriptor);
DEFINE_CLASS(Weather::EffectDescTemp);

void Weather::EffectDescTemp::New(Il2CppString* _Author, Il2CppString* _EffectName, bool _WorksInMenu, bool _WorksInGame) {
    this->Author = _Author;
    this->EffectName = _EffectName;
    this->WorksInMenu = _WorksInMenu;
    this->WorksInGame = _WorksInGame;
}
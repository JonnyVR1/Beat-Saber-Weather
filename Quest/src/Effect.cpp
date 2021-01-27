#include "Effect.hpp"
#include "EffectDescriptor.hpp"
#include "WeatherSceneInfo.hpp"
#include "EffectModel.hpp"
#include "include/Logger.hpp"
#include "UnityEngine/Material.hpp"
#include "UnityEngine/Transform.hpp"
#include "System/Collections/Generic/List_1.hpp"
#include "System/Linq/Enumerable.hpp"
#include "Config.hpp"

#include "beatsaber-hook/shared/utils/il2cpp-utils.hpp"
using namespace UnityEngine;
using namespace System::Collections::Generic;
using namespace System::Linq;

DEFINE_CLASS(Weather::Effect);

void Weather::Effect::ctor(Weather::EffectDescriptor* _effectDescriptor, GameObject* _gameObject, bool _enabled) {
    this->enabled = _enabled;
    this->Desc = _effectDescriptor;
    this->gameObject = _gameObject;
}

void Weather::Effect::SetActiveRefs(bool force) {
    Weather::GenericLogger::Log("SetActiveRefs");
    this->Grab = gameObject->get_transform()->GetChild(0)->Find(il2cpp_utils::createcsstr("NotesShader"));
    enabled = EffectModel::GetEffectEnabledByName(Desc->EffectName);
    if(force) enabled = force;
    if(!enabled)
    {
        Weather::GenericLogger::Log("NotFound");
        gameObject->SetActive(false);
        return;
    }
    if(to_utf8(csstrtostr(WeatherSceneInfo::CurrentScene.get_name())) == "MenuViewControllers")
    {
        gameObject->SetActive((Desc->WorksInMenu && showInMenu));
    }
    if(to_utf8(csstrtostr(WeatherSceneInfo::CurrentScene.get_name())) == "GameCore")
    {
        gameObject->SetActive((Desc->WorksInGame && showInGame));
    }
}
//Unfilled since Ingame UI isnt implemented yet
void Weather::Effect::SetSceneMaterials()
{

}

void Weather::Effect::RemoveSceneMaterials()
{
    
}
void Weather::Effect::TrySetNoteMateral(MeshRenderer* mr) {
    if(!Grab || !Grab->GetComponent<MeshRenderer*>()) return;
    if(!getConfig().config["enableShaderAdditions"].GetBool()) return;
    if(!enabled) return;
    Material* notemat = Grab->GetComponent<MeshRenderer*>()->get_material();
    Array<Material*>* mats = mr->get_materials();
    Array<Material*>* newMats = mats->NewLength(2);
    newMats->values[0] = mats->values[0];
    newMats->values[1] = notemat;
    mr->set_materials(newMats);
}
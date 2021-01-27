#include "Effect.hpp"
#include "EffectDescriptor.hpp"
#include "EffectModel.hpp"
#include "BundleLoader.hpp"
#include "WeatherSceneInfo.hpp"
#include "Logger.hpp"
#include "Config.hpp"

#include "beatsaber-hook/shared/utils/il2cpp-utils.hpp"
#include "codegen/include/UnityEngine/MeshRenderer.hpp"
#include "codegen/include/UnityEngine/Transform.hpp"
#include "codegen/include/UnityEngine/GameObject.hpp"
#include "codegen/include/UnityEngine/Material.hpp"
#include "codegen/include/UnityEngine/Resources.hpp"
#include "codegen/include/UnityEngine/AudioSource.hpp"
#include "codegen/include/System/Linq/Enumerable.hpp"
#include "codegen/include/System/Collections/Generic/List_1.hpp"

using namespace UnityEngine;
using namespace System::Collections::Generic;
using namespace System::Linq;
DEFINE_CLASS(Weather::WeatherSceneInfo);

UnityEngine::SceneManagement::Scene Weather::WeatherSceneInfo::CurrentScene;
bool Weather::WeatherSceneInfo::hasFullSetRefs;

bool NameContains(std::string str, std::string substr)
{
    return str.find(substr) != std::string::npos;
}

bool shouldApplyAdditionalShader(MeshRenderer* mr)
{
    std::string matName = to_utf8(csstrtostr(mr->get_material()->get_name()->ToLower()));
    std::string objName = to_utf8(csstrtostr(mr->get_gameObject()->get_name()->ToLower()));
    //std::string parentObjName = to_utf8(csstrtostr(mr->get_transform()->get_parent()->get_gameObject()->get_name()->ToLower()));
    return NameContains(matName, "note") || NameContains(objName, "building") || NameContains(objName, "speaker") || NameContains(objName, "spear") || NameContains(objName, "trail") || NameContains(objName, "arena");//* && NameContains(parentObjName, "tentacle"))*/ || NameContains(objName, "underground") || NameContains(objName, "hall") || NameContains(objName, "vconstruction") || NameContains(objName, "topvones") || NameContains(objName, "car") || (objName == "construction") || (objName == "trackconstruction") || NameContains(objName, "pillar");
}

void Weather::WeatherSceneInfo::SetRefs() {
    ReloadConfig();
    hasFullSetRefs = true;
    Array<MeshRenderer*>* mrs = Resources::FindObjectsOfTypeAll<MeshRenderer*>(); 
    std::string sceneName = to_utf8(csstrtostr(CurrentScene.get_name()));
    BundleLoader::WeatherPrefab->SetActive(true);
    if(sceneName == "MenuViewControllers") 
    {
        if(!getConfig().config["enabledInMenu"].GetBool())
        {
            BundleLoader::WeatherPrefab->SetActive(false);
            return;
        }
    }
    if(sceneName == "GameCore") 
    {
        if(!getConfig().config["enabledInGameplay"].GetBool())
        {
            BundleLoader::WeatherPrefab->SetActive(false);
            return;
        }
    }
    
    for (int i = 0; i < this->get_transform()->get_childCount(); i++)
    {   
        Transform* Child = this->get_transform()->GetChild(i);
        if(!Child) continue;
        
        Child->get_gameObject()->SetActive(true);

        auto sources = Child->get_gameObject()->GetComponentsInChildren<AudioSource*>();
        for(int i = 0; i < sources->Length(); i++)
            sources->values[i]->set_volume(getConfig().config["audioSFXVolume"].GetFloat());
        
        Weather::EffectDescriptor* efd = Child->get_gameObject()->GetComponent<EffectDescriptor*>();
        if(!efd) continue;
        efd->get_gameObject()->SetActive(true);
        efd->get_transform()->GetChild(0)->get_gameObject()->SetActive(true);
        
        Effect* eff = new Weather::Effect();
        eff->Desc = efd;
        eff->gameObject = Child->get_gameObject();
        eff->enabled = EffectModel::GetEffectEnabledByName(efd->EffectName);
        eff->showInGame = true;
        eff->showInMenu = true; 
        eff->SetActiveRefs();
        if(!eff) continue;
        for (int x = 0; x < mrs->Length(); x++)
        {
            MeshRenderer* mr = mrs->values[x];
            if(!mr) continue;
            if (shouldApplyAdditionalShader(mr)) 
            {
                Weather::GenericLogger::Log(to_utf8(csstrtostr(mr->get_material()->get_name())));
                eff->TrySetNoteMateral(mr);
            }
        }
    }
}

void Weather::WeatherSceneInfo::SetActiveRefs() {
    if(!hasFullSetRefs)
    {
        SetRefs();
        return;
    }
    Array<MeshRenderer*>* mrs = Resources::FindObjectsOfTypeAll<MeshRenderer*>(); 
    for (Effect* eff : BundleLoader::effects)
    {
        eff->enabled = EffectModel::GetEffectEnabledByName(eff->Desc->EffectName);
        eff->showInGame = true;
        eff->showInMenu = true; 
        eff->SetActiveRefs();
        for (int x = 0; x < mrs->Length(); x++)
        {
            MeshRenderer* mr = mrs->values[x];
            if (shouldApplyAdditionalShader(mr)) 
            {
                Weather::GenericLogger::Log(to_utf8(csstrtostr(mr->get_material()->get_name())));
                eff->TrySetNoteMateral(mr);
            }
        }
    }
    
}
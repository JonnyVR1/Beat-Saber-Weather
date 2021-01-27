using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Weather
{
    public class Effect
    {
        public EffectDiscriptor Desc { get; private set; } = null;
        public GameObject gameObject { get; private set; } = null;
        public bool enabled { get; set; } = false;

        public bool showInMenu { get; set; } = true;
        public bool showInGame { get; set; } = true;

        public Effect(EffectDiscriptor _effectDiscriptor, GameObject _gameObject, bool _enabled)
        {
            Desc = _effectDiscriptor;
            gameObject = _gameObject;
            enabled = _enabled;
            
        }
        public void SetActiveRefs(bool force = false)
        {
            Plugin.Log.Info("Setting Active Refs " + Desc.EffectName + " " + WeatherSceneInfo.CurrentScene.name);
            enabled = EffectModel.GetEffectEnabledByName(Desc.EffectName);
            if (force) enabled = true;
            if(!enabled)
            {
                gameObject.SetActive(false);
                return;
            }         
            
            if (WeatherSceneInfo.CurrentScene.name == Plugin.menu)
            {   
                Plugin.Log.Info(Desc.EffectName + (Desc.WorksInMenu && showInMenu).ToString());
                gameObject.SetActive((Desc.WorksInMenu && showInMenu));
            }
            if (WeatherSceneInfo.CurrentScene.name == Plugin.game)
            {
                Plugin.Log.Info(Desc.EffectName + (Desc.WorksInGame && showInGame).ToString());
                gameObject.SetActive((Desc.WorksInGame && showInGame));
            }
        }
        public void SetSceneMaterials()
        {
            Transform Grab = gameObject.transform.GetChild(0).Find("NotesShader");
            if (Grab == null) return;
            MeshRenderer[] mrs = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            for (int x = 0; x < mrs.Length; x++)
            {
                MeshRenderer mr = mrs[x];
                if (mr.material.name.Contains("Note") || mr.gameObject.name.Contains("building") || mr.gameObject.name.Contains("speaker"))
                {
                    TrySetNoteMateral(mr);
                }
                else continue;
            }
        }
        public void RemoveSceneMaterials()
        {
            Transform Grab = gameObject.transform.GetChild(0).Find("NotesShader");
            if (Grab == null) return;
            MeshRenderer[] mrs = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            for (int x = 0; x < mrs.Length; x++)
            {
                MeshRenderer mr = mrs[x];
                if (mr.material.name.Contains("Note") || mr.gameObject.name.Contains("building") || mr.gameObject.name.Contains("speaker"))
                {
                    List<Material> mats = mr.sharedMaterials.ToList();
                    mats.RemoveAt(1);
                    mr.materials = mats.ToArray();
                }
                else continue;
            }
        }
        public void TrySetNoteMateral(MeshRenderer mr)
        {
            if (!enabled) return;
            Transform Grab = gameObject.transform.GetChild(0).Find("NotesShader");
            if (Grab == null) return;
            Material notemat = Grab.GetComponent<MeshRenderer>().material;
            List<Material> mats = mr.sharedMaterials.ToList();
            if(mats.Count == 1)
                mats.Add(notemat);
            mr.materials = mats.ToArray();
        }
    }
}

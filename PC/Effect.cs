using System.Linq;
using UnityEngine;

namespace Weather
{
    public class Effect
    {
        public EffectDescriptor Desc { get; private set; }
        private GameObject GameObject { get; set; }
        public bool Enabled { get; set; }

        public bool ShowInMenu { get; set; } = true;
        public bool ShowInGame { get; set; } = true;

        public Effect(EffectDescriptor effectDescriptor, GameObject gameObject, bool enabled)
        {
            Desc = effectDescriptor;
            GameObject = gameObject;
            Enabled = enabled;
            
        }
        public void SetActiveRefs(bool force = false)
        {
            Plugin.Log.Info("Setting Active Refs " + Desc.effectName + " " + WeatherSceneInfo.CurrentScene.name);
            Enabled = EffectModel.GetEffectEnabledByName(Desc.effectName) || force;
            if(!Enabled)
            {
                GameObject.SetActive(false);
                return;
            }         
            
            switch (WeatherSceneInfo.CurrentScene.name)
            {
                case Plugin.Menu:
                    Plugin.Log.Info(Desc.effectName + (Desc.worksInMenu && ShowInMenu));
                    GameObject.SetActive((Desc.worksInMenu && ShowInMenu));
                    break;
                case Plugin.Game:
                    Plugin.Log.Info(Desc.effectName + (Desc.worksInGame && ShowInGame));
                    GameObject.SetActive((Desc.worksInGame && ShowInGame));
                    break;
            }
        }
        public void SetSceneMaterials()
        {
            var grab = GameObject.transform.GetChild(0).Find("NotesShader");
            if (grab == null) return;
            var mrs = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            foreach (var mr in mrs)
            {
                if (mr.material.name.Contains("Note") || mr.gameObject.name.Contains("building") || mr.gameObject.name.Contains("speaker"))
                {
                    TrySetNoteMaterial(mr);
                }
            }
        }
        public void RemoveSceneMaterials()
        {
            var grab = GameObject.transform.GetChild(0).Find("NotesShader");
            if (grab == null) return;
            var mrs = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            foreach (var mr in mrs)
            {
                if (!mr.material.name.Contains("Note") && !mr.gameObject.name.Contains("building") &&
                    !mr.gameObject.name.Contains("speaker")) continue;
                var mats = mr.sharedMaterials.ToList();
                mats.RemoveAt(1);
                mr.materials = mats.ToArray();
            }
        }
        public void TrySetNoteMaterial(MeshRenderer mr)
        {
            if (!Enabled) return;
            var grab = GameObject.transform.GetChild(0).Find("NotesShader");
            if (grab == null) return;
            var noteMaterial = grab.GetComponent<MeshRenderer>().material;
            var mats = mr.sharedMaterials.ToList();
            if(mats.Count == 1)
                mats.Add(noteMaterial);
            mr.materials = mats.ToArray();
        }
    }
}

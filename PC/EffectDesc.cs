using UnityEngine;
using UnityEngine.Serialization;

namespace Weather
{
    public class EffectDescriptor : MonoBehaviour
    {
        [FormerlySerializedAs("Author")] [SerializeField] public string author = "MyName";
        [FormerlySerializedAs("EffectName")] [SerializeField] public string effectName = "MyEffect";
        [FormerlySerializedAs("WorksInMenu")] [SerializeField] public bool worksInMenu = true;
        [FormerlySerializedAs("WorksInGame")] [SerializeField] public bool worksInGame = true;
        [SerializeField] public Sprite coverImage;
    }   

    [System.Serializable]
    public class TempDesc011
    {
        public string Author;
        public string EffectName;
        public bool WorksInMenu;

        protected TempDesc011(string author, string effectName, bool worksInMenu)
        {
            Author = author;
            EffectName = effectName;
            WorksInMenu = worksInMenu;
        }
    }

    [System.Serializable]
    public class TempDesc012
    {
        public string Author;
        public string EffectName;
        public bool WorksInMenu;
        public bool WorksInGame;
        public Texture2D CoverImage;

        public TempDesc012(string author, string effectName, bool worksInMenu, bool worksInGame)
        {
            Author = author;
            EffectName = effectName;
            WorksInMenu = worksInMenu;
            WorksInGame = worksInGame;
        }
    }
}
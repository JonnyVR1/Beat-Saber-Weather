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

    public abstract class TempDesc011
    {
        public readonly string Author;
        public readonly string EffectName;
        public readonly bool WorksInMenu;

        protected TempDesc011(string author, string effectName, bool worksInMenu)
        {
            Author = author;
            EffectName = effectName;
            WorksInMenu = worksInMenu;
        }
    }

    public class TempDesc012
    {
        public readonly string Author;
        public readonly string EffectName;
        public readonly bool WorksInMenu;
        public readonly bool WorksInGame;
        public Texture2D CoverImage = null;

        public TempDesc012(string author, string effectName, bool worksInMenu, bool worksInGame)
        {
            Author = author;
            EffectName = effectName;
            WorksInMenu = worksInMenu;
            WorksInGame = worksInGame;
        }
    }
}
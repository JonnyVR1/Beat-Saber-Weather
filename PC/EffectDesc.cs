using UnityEngine;
public class EffectDiscriptor : MonoBehaviour
{
    [SerializeField] public string Author = "MyName";
    [SerializeField] public string EffectName = "MyEffect";
    [SerializeField] public bool WorksInMenu = true;
    [SerializeField] public bool WorksInGame = true;
    [SerializeField] public Sprite coverImage = null;
}   

public class TempDesc_0_1_1
{
    public string Author = "MyName";
    public string EffectName = "MyEffect";
    public bool WorksInMenu = true;

    public TempDesc_0_1_1(string _Author, string _EffectName, bool _WorksInMenu)
    {
        Author = _Author;
        EffectName = _EffectName;
        WorksInMenu = _WorksInMenu;
    }
}

public class TempDesc_0_1_2
{
    public string Author = "MyName";
    public string EffectName = "MyEffect";
    public bool WorksInMenu = true;
    public bool WorksInGame = true;
    public Texture2D coverImage = null;

    public TempDesc_0_1_2(string _Author, string _EffectName, bool _WorksInMenu, bool _WorksInGame)
    {
        Author = _Author;
        EffectName = _EffectName;
        WorksInMenu = _WorksInMenu;
        WorksInGame = _WorksInGame;
    }
}
using UnityEngine;
public class EffectDiscriptor : MonoBehaviour
{
    public string Author = "MyName";
    public string EffectName = "MyEffect";
    public bool WorksInMenu = true;
    public bool WorksInGame = true;
    public Texture2D coverImage = null;

    public JSON GetJSON()
    {
        return new JSON(Author, EffectName, WorksInMenu, WorksInGame);
    }
}

public class JSON
{
    public string Author = "MyName";
    public string EffectName = "MyEffect";
    public bool WorksInMenu = true;
    public bool WorksInGame = true;

    public JSON(string author, string effectName, bool worksInMenu, bool worksInGame)
    {
        Author = author;
        EffectName = effectName;
        WorksInMenu = worksInMenu;
        WorksInGame = worksInGame;
    }
}

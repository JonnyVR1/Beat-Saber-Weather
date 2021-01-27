using System.IO;
using UnityEngine;

public class Prefs
{
    public static string PreferredAuthorName;
    public static string BeatSaberPath;
    public static string GetPrefsFolder() { return Path.Combine(Application.dataPath, "Exporters", "Prefs"); }
    public static void CreatePrefsFolder()
    { 
        Directory.CreateDirectory(GetPrefsFolder()); 
    }
    
    public static void WritePrefs()
    {
        string path = Path.Combine(GetPrefsFolder(), "Prefs.txt");
        if (!Directory.Exists(GetPrefsFolder())) CreatePrefsFolder();
        File.WriteAllText(path, PreferredAuthorName + "~" + BeatSaberPath);
    }   

    public static void ReadPrefs()
    {
        string path = Path.Combine(GetPrefsFolder(), "Prefs.txt");
        if (!Directory.Exists(GetPrefsFolder())) CreatePrefsFolder();
        if (!File.Exists(path)) WritePrefs();
        string FileStr = File.ReadAllText(path);
        PreferredAuthorName = FileStr.Split('~')[0];
        BeatSaberPath = FileStr.Split('~')[1];
    }
}
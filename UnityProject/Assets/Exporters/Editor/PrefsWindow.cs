using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class PrefsEditor : EditorWindow
{
    [MenuItem("Weather/Exporter Prefs")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PrefsEditor), false, "Exporter Prefs");
    }

    public void OnGUI()
    {
        Prefs.PreferredAuthorName = EditorGUILayout.TextField("Default Author Name", Prefs.PreferredAuthorName);
        Prefs.BeatSaberPath = EditorGUILayout.TextField("BeatSaber Path", Prefs.BeatSaberPath);
        if (GUILayout.Button("Save Prefs"))
        {
            Prefs.WritePrefs();
        }
    }
    void OnInspectorUpdate()
    {
        Repaint();
    }
}
#endif
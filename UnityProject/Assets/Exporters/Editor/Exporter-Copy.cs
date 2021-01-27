using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(EffectDiscriptor))]
public class CreateEffectAssetBundles : Editor
{
    private static bool Quest;
    private static bool PC = true;
    private static bool CopyToBeatSaberFolder = true;
    private static bool LogConfig = false;
    private static bool autoGrabCover = true;
    private static Camera screenshotCamera = null;
    private static RenderTexture rt;

    public static void DeleteDirectory(string target_dir)
    {
        string[] files = Directory.GetFiles(target_dir);
        string[] dirs = Directory.GetDirectories(target_dir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(target_dir, false);
    }
    public static Texture2D getTexture2D(Camera cam, int width = 1024, int height = 512)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        if(rt == null) rt = Resources.FindObjectsOfTypeAll<RenderTexture>().First((RenderTexture rt) => { return rt.name == "DoNotTouch"; });
        if (screenshotCamera == null) screenshotCamera = Camera.main;

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        return tex;
    }
    private void BuildAssetBundle(BuildTarget Target, string Ext)
    {
        EffectDiscriptor effectDiscriptor = (EffectDiscriptor)serializedObject.targetObject;
        string name = Regex.Replace(effectDiscriptor.EffectName, @"\s+", "");
        if (autoGrabCover)
        {
            effectDiscriptor.coverImage = getTexture2D(screenshotCamera, 1024, 1024);
        }
        GameObject prefab = new GameObject("Effect");
        Selection.activeGameObject.transform.SetParent(prefab.transform);
        PrefabUtility.SaveAsPrefabAsset(prefab, "Assets/Effect.prefab");

        TextAsset effectJson = new TextAsset(JsonUtility.ToJson(effectDiscriptor.GetJSON(), true));
        AssetDatabase.CreateAsset(effectJson, "Assets/effectJson.asset");
        string coverPath = Path.Combine(Application.dataPath, "Covers", name + "Cover.png");
        if (!Directory.Exists(Path.GetDirectoryName(coverPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(coverPath));
        File.WriteAllBytes(coverPath, effectDiscriptor.coverImage.EncodeToPNG());

        AssetDatabase.CreateAsset(effectDiscriptor.coverImage, "Assets/Covers/" + name + "Cover.asset");
        
        AssetBundleBuild assetBundleBuild = default;
        assetBundleBuild.assetNames = new string[]
        {
            "Assets/Effect.prefab",
            "Assets/effectJson.asset",
            "Assets/Covers/" + name + "Cover.asset",
        };
        string path = Path.Combine(Application.dataPath, "EffectResult", effectDiscriptor.EffectName + Ext);
        if (!Directory.Exists(Path.GetDirectoryName(path)))
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        //if (string.IsNullOrEmpty(path)) return;
        assetBundleBuild.assetBundleName = Path.GetFileName(path);
        if (File.Exists(path))
            File.Delete(path);
        BuildPipeline.BuildAssetBundles(Path.GetDirectoryName(path), new AssetBundleBuild[] { assetBundleBuild }, 0, Target);
        EditorPrefs.SetString("currentBuildingAssetBundlePath", Path.GetDirectoryName(path));
        //Cleanup
        
        AssetDatabase.DeleteAsset("Assets/Effect.prefab");
        AssetDatabase.DeleteAsset("Assets/effectJson.asset");
        AssetDatabase.DeleteAsset("Assets/Covers/" + name + "Cover.asset");


        if (CopyToBeatSaberFolder)
        {
            if(!Directory.Exists(Path.Combine(Prefs.BeatSaberPath, "UserData", "Weather", "Effects")))
            {
                Directory.CreateDirectory(Path.Combine(Prefs.BeatSaberPath, "UserData", "Weather", "Effects"));
            }
            string copyPath = Path.Combine(Prefs.BeatSaberPath, "UserData", "Weather", "Effects", effectDiscriptor.EffectName + Ext);
            if (File.Exists(copyPath))
                File.Delete(copyPath);
            File.Copy(path, copyPath); 
        }
        effectDiscriptor.transform.parent = null;
        DestroyImmediate(prefab);
        DeleteDirectory(Path.GetDirectoryName(path));
        File.Delete(Path.GetDirectoryName(path) + ".meta");
        AssetDatabase.Refresh();
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        EffectDiscriptor effectDiscriptor = (EffectDiscriptor)serializedObject.targetObject;
        ResetPrefs(effectDiscriptor);
        GUILayout.Space(15f);
        EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);
        if (GUILayout.Button("Build " + effectDiscriptor.EffectName))
        {
            if (PC)
                BuildAssetBundle(BuildTarget.StandaloneWindows, ".effect");

            if (Quest)
                BuildAssetBundle(BuildTarget.Android, ".qeffect");
        }
        PC = GUILayout.Toggle(PC, "Include PC Export");
        Quest = GUILayout.Toggle(Quest, "Include Quest Export");
        CopyToBeatSaberFolder = GUILayout.Toggle(CopyToBeatSaberFolder, "Copy Result To BeatSaber Folder");
        autoGrabCover = GUILayout.Toggle(autoGrabCover, "Automatically Grab Cover");

        if (CopyToBeatSaberFolder)
        {
            if(Prefs.BeatSaberPath == "")  
                EditorGUILayout.LabelField("    Prefs Beatsaber path is empty! Please set it in Weather/Exporter Prefs or your effect won't be copied", EditorStyles.boldLabel);

            if (!PC)
                EditorGUILayout.LabelField("    Won't export for PC, result Won't be copied!", EditorStyles.boldLabel);
        }
        else
        {
            EditorGUILayout.LabelField(string.Format("  Not Copying, result will be in, {0} and .QEffect", Path.GetFullPath(Path.Combine(Application.dataPath, "EffectResult", effectDiscriptor.EffectName + ".Effect"))), EditorStyles.boldLabel);
        }

        if(autoGrabCover)
        {
            if(Camera.main == null)
                EditorGUILayout.LabelField("    Wants to grab cover automatically but there is no camera!", EditorStyles.boldLabel);
        }

        Transform NotesShader = effectDiscriptor.transform.Find("NotesShader");
        if (NotesShader)
        {
            MeshRenderer mr;
            if((mr = NotesShader.GetComponent<MeshRenderer>()) == null)
            {
                EditorGUILayout.LabelField("Has NotesShader Object but it doesn't have a MeshRenderer!", EditorStyles.boldLabel);
            }
            else if (mr.sharedMaterials.Length == 0 || mr.sharedMaterials[0] == null)
            {
                EditorGUILayout.LabelField("Has NotesShader Object with MeshRenderer but there are no materials assigned!", EditorStyles.boldLabel);
            }
        }
        
        LogConfig = GUILayout.Toggle(LogConfig, "Log Effect JSON");

        GUILayout.Space(15f);
        EditorGUILayout.LabelField("Prefs", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("You can change your prefs on the top right in Weather/Exporter Prefs", EditorStyles.label);
        if (GUILayout.Button("Reload Prefs"))
        {
            Prefs.ReadPrefs();
            ResetPrefs(effectDiscriptor);
        }

        GUILayout.Space(15f);
        EditorGUILayout.LabelField("Misc", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Note Shader Copy"))
        {
            if (NotesShader) return;
            GameObject NotesShaderNew = new GameObject("NotesShader", typeof(MeshRenderer));
            NotesShaderNew.transform.SetParent(effectDiscriptor.transform);
            
        }
        if(rt == null || screenshotCamera == null)
        {
            if (GUILayout.Button("Add Screen shot Camera"))
            {
                if (NotesShader) return;
                GameObject Camera = new GameObject("CoverCamera", typeof(Camera));
                Camera.tag = "MainCamera";
                Camera.transform.position = new Vector3(5f, 9.5f, -5.5f);
                Camera.transform.rotation = Quaternion.Euler(45f, -45f, 0f);
                Camera cam = Camera.GetComponent<Camera>();
                cam.clearFlags = CameraClearFlags.Color;
                cam.backgroundColor = Color.black;
                cam.targetTexture = Resources.FindObjectsOfTypeAll<RenderTexture>().First((RenderTexture rt) => { return rt.name == "DoNotTouch"; });
                rt = cam.targetTexture;
                screenshotCamera = cam;
            }
        }
    }
    public void ResetPrefs(EffectDiscriptor effectDiscriptor)
    {
        Prefs.ReadPrefs();
        if (effectDiscriptor.Author == "MyName" || string.IsNullOrEmpty(effectDiscriptor.Author))
        {
            effectDiscriptor.Author = Prefs.PreferredAuthorName;
        }
    }
}

#endif
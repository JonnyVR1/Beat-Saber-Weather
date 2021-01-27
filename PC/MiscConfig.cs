using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Weather
{
    class MiscConfigObject
    {
        public string name;
        public bool showInMenu;
        public bool showInGame;

        public MiscConfigObject(string name, bool showInMenu, bool showInGame)
        {
            this.name = name;
            this.showInMenu = showInMenu;
            this.showInGame = showInGame;
        }
    }
    class MiscConfigObjectRoot
    {
        public List<MiscConfigObject> miscConfigObjects = new List<MiscConfigObject>();
    }
    class MiscConfig
    {
        private static MiscConfigObjectRoot Root = new MiscConfigObjectRoot();
        private static string path = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "WeatherMisConfig.txt"));
        public static string Serialize(MiscConfigObject Object)
        {
            return Object.name + "_" + Object.showInMenu + "_" + Object.showInGame;
        }

        public static MiscConfigObject Deserialize(string Object)
        {
            string[] parts = Object.Split('_');
            return new MiscConfigObject(parts[0], bool.Parse(parts[1]), bool.Parse(parts[2]));
        }
        public static void Write()
        {
            Plugin.Log.Info(path);
            List<string> lines = new List<string>();
            foreach(MiscConfigObject Object in Root.miscConfigObjects)
            {
                string serialized = Serialize(Object);
                Plugin.Log.Info(serialized);
                lines.Add(serialized);
            }
            File.WriteAllLines(path, lines);
        }

        public static void Add(MiscConfigObject Object)
        {
            Root.miscConfigObjects.Add(Object);
        }

        public static void Read()
        {
            if (!File.Exists(path)) { File.WriteAllText(path, ""); return; }
            string[] lines = File.ReadAllLines(path);
            Root.miscConfigObjects.Clear();
            foreach (string line in lines)
            {
                string[] parts = line.Split('_');

                Root.miscConfigObjects.Add(new MiscConfigObject(parts[0], bool.Parse(parts[1]), bool.Parse(parts[2])));
            }
        }

        public static void WriteToObject(MiscConfigObject Object)
        {
            string[] arrLine = File.ReadAllLines(path);
            for(int i = 0; i < arrLine.Length; i++)
            {
                string line = arrLine[i];
                string[] parts = line.Split('_');
                if(parts[0] == Object.name)
                    arrLine[i] = Serialize(Object);
            }
            File.WriteAllLines(path, arrLine);
        }

        public static MiscConfigObject ReadObject(string name)
        {
            string[] arrLine = File.ReadAllLines(path);
            MiscConfigObject Out = new MiscConfigObject(name, true, true);
            for (int i = 0; i < arrLine.Length; i++)
            {
                string line = arrLine[i];
                Out = Deserialize(line);   
            }
            return Out;
        }

        public static bool hasObject(string name)
        {
            string[] arrLine = File.ReadAllLines(path);
            bool hasObject = false;
            for (int i = 0; i < arrLine.Length; i++)
            {
                string line = arrLine[i];
                string[] parts = line.Split('_');
                if (parts[0] == name) hasObject = true;
            }
            return hasObject;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Weather
{
    internal class MiscConfigObject
    {
        public readonly string Name;
        public bool ShowInMenu;
        public bool ShowInGame;

        public MiscConfigObject(string name, bool showInMenu, bool showInGame)
        {
            Name = name;
            ShowInMenu = showInMenu;
            ShowInGame = showInGame;
        }
    }

    internal class MiscConfigObjectRoot
    {
        public readonly List<MiscConfigObject> MiscConfigObjects = new();
    }

    internal static class MiscConfig
    {
        private static readonly MiscConfigObjectRoot Root = new();
        private static readonly string Path = System.IO.Path.GetFullPath(System.IO.Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "WeatherMisConfig.txt"));

        private static string Serialize(MiscConfigObject @object)
        {
            return @object.Name + "_" + @object.ShowInMenu + "_" + @object.ShowInGame;
        }

        private static MiscConfigObject Deserialize(string @object)
        {
            var parts = @object.Split('_');
            return new MiscConfigObject(parts[0], bool.Parse(parts[1]), bool.Parse(parts[2]));
        }

        public static void Write()
        {
            Plugin.Log.Info(Path);
            var lines = new List<string>();
            foreach (var serialized in Root.MiscConfigObjects.Select(Serialize))
            {
                Plugin.Log.Info(serialized);
                lines.Add(serialized);
            }

            File.WriteAllLines(Path, lines);
        }

        public static void Add(MiscConfigObject @object)
        {
            Root.MiscConfigObjects.Add(@object);
        }

        public static void Read()
        {
            if (!File.Exists(Path))
            {
                File.WriteAllText(Path, ""); return;
            }

            var lines = File.ReadAllLines(Path);
            Root.MiscConfigObjects.Clear();
            foreach (var line in lines)
            {
                var parts = line.Split('_');

                Root.MiscConfigObjects.Add(new MiscConfigObject(parts[0], bool.Parse(parts[1]), bool.Parse(parts[2])));
            }
        }

        public static void WriteToObject(MiscConfigObject @object)
        {
            var arrLine = File.ReadAllLines(Path);
            for(var i = 0; i < arrLine.Length; i++)
            {
                var line = arrLine[i];
                var parts = line.Split('_');
                if (parts[0] == @object.Name)
                {
                    arrLine[i] = Serialize(@object);
                }
            }

            File.WriteAllLines(Path, arrLine);
        }

        public static MiscConfigObject ReadObject(string name)
        {
            var arrLine = File.ReadAllLines(Path);
            var @out = new MiscConfigObject(name, true, true);
            foreach (var line in arrLine)
            {
                @out = Deserialize(line);
            }

            return @out;
        }

        public static bool HasObject(string name)
        {
            var arrLine = File.ReadAllLines(Path);
            foreach (var line in arrLine)
            {
                var parts = line.Split('_');
                if (parts[0] == name) return true;
            }

            return false;
        }
    }
}

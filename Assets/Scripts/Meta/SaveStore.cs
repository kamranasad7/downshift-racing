using System.IO;
using UnityEngine;

namespace Downshift
{
    public static class SaveStore
    {
        public static string DefaultPath => Application.persistentDataPath + "/save.json";

        public static SaveModel Load(string path)
        {
            try
            {
                if (!File.Exists(path)) return new SaveModel();
                var m = JsonUtility.FromJson<SaveModel>(File.ReadAllText(path));
                if (m == null || m.upgradeTiers == null || m.upgradeTiers.Length != 4) return new SaveModel();
                return m;
            }
            catch
            {
                return new SaveModel();
            }
        }

        public static void Save(SaveModel m, string path)
        {
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, JsonUtility.ToJson(m));
            if (File.Exists(path)) File.Replace(tmp, path, null);
            else File.Move(tmp, path);
        }
    }
}

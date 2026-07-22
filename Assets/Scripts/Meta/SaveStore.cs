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
            File.WriteAllText(path, JsonUtility.ToJson(m));
        }
    }
}

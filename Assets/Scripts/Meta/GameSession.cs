using UnityEngine;

namespace Downshift
{
    public static class GameSession
    {
        public static string PathOverride;
        static SaveModel _save;
        static EconomyConfig _economy;
        static bool _economyLoaded;

        public static SaveModel Save
        {
            get
            {
                if (_save == null) _save = SaveStore.Load(PathOverride ?? SaveStore.DefaultPath);
                return _save;
            }
        }

        public static EconomyConfig Economy
        {
            get
            {
                if (!_economyLoaded)
                {
                    _economy = Resources.Load<EconomyConfig>("Economy");
                    _economyLoaded = true;
                }
                return _economy;
            }
        }

        public static void Persist()
        {
            SaveStore.Save(Save, PathOverride ?? SaveStore.DefaultPath);
        }

        public static void Reset()
        {
            _save = null;
            _economyLoaded = false;
            _economy = null;
        }
    }
}

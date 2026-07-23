namespace Downshift
{
    [System.Serializable]
    public class SaveModel
    {
        public int coins;
        public float bestDistanceM;
        public int[] upgradeTiers = new int[4];
        // default-false means legacy saves (no field) load as unmuted, not muted
        public bool sfxMuted;
        public bool musicMuted;
        public int schemaVersion = 1;
    }
}

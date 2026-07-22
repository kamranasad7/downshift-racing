namespace Downshift
{
    [System.Serializable]
    public class SaveModel
    {
        public int coins;
        public float bestDistanceM;
        public int[] upgradeTiers = new int[4];
        public int schemaVersion = 1;
    }
}

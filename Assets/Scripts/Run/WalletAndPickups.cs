using UnityEngine;

namespace Downshift
{
    public static class Wallet
    {
        public static int Coins;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => Coins = 0;
    }
}

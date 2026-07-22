using UnityEngine;

namespace Downshift
{
    public class Pickup : MonoBehaviour
    {
        public PickupKind kind;

        void OnTriggerEnter2D(Collider2D other)
        {
            var vehicle = other.GetComponentInParent<VehicleController>();
            if (vehicle == null) return;
            switch (kind)
            {
                case PickupKind.Coin: Wallet.Coins++; GameAudio.PlayCoin(); break;
                case PickupKind.Coolant: vehicle.CoolPartial(30f, 30f); break;
                case PickupKind.Station: vehicle.CoolFull(); break;
            }
            gameObject.SetActive(false);
        }
    }
}

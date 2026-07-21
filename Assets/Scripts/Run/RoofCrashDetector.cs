using UnityEngine;

namespace Downshift
{
    public class RoofCrashDetector : MonoBehaviour
    {
        public RunManager runManager;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger) return;
            if (other.GetComponentInParent<VehicleController>() != null) return;
            if (runManager != null) runManager.NotifyCrash();
        }
    }
}

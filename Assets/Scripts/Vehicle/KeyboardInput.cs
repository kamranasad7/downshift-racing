using UnityEngine;

namespace Downshift
{
    public class KeyboardInput : MonoBehaviour
    {
        public VehicleController vehicle;

        void Update()
        {
            VehicleInput.KeyboardBrake = Input.GetKey(KeyCode.Space);
            if (vehicle == null) return;
            if (Input.GetKeyDown(KeyCode.UpArrow)) vehicle.GearUp();
            if (Input.GetKeyDown(KeyCode.DownArrow)) vehicle.GearDown();
        }
    }
}

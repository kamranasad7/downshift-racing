using UnityEngine;
using UnityEngine.EventSystems;

namespace Downshift
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool Held { get; private set; }
        public void OnPointerDown(PointerEventData e) { Held = true; VehicleInput.UiBrake = true; }
        public void OnPointerUp(PointerEventData e) { Held = false; VehicleInput.UiBrake = false; }
        void OnDisable() { Held = false; VehicleInput.UiBrake = false; }
    }
}

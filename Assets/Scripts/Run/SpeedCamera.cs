using Unity.Cinemachine;
using UnityEngine;

namespace Downshift
{
    public class SpeedCamera : MonoBehaviour
    {
        public VehicleController vehicle;
        public float minSize = 6f;
        public float maxSize = 12f;
        public float speedForMax = 30f;
        public float maxLookAhead = 4f;
        public float smoothing = 2f;

        CinemachineCamera _cam;
        CinemachinePositionComposer _composer;

        void Awake()
        {
            _cam = GetComponent<CinemachineCamera>();
            _composer = GetComponent<CinemachinePositionComposer>();
        }

        void LateUpdate()
        {
            float t = Mathf.Clamp01(vehicle.SpeedMs / speedForMax);
            var lens = _cam.Lens;
            lens.OrthographicSize = Mathf.Lerp(lens.OrthographicSize, Mathf.Lerp(minSize, maxSize, t), Time.deltaTime * smoothing);
            _cam.Lens = lens;
            var offset = _composer.TargetOffset;
            offset.x = Mathf.Lerp(offset.x, Mathf.Lerp(0f, maxLookAhead, t), Time.deltaTime * smoothing);
            _composer.TargetOffset = offset;
        }
    }
}

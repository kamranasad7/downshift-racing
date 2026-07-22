using UnityEngine;

namespace Downshift
{
    public class CoinSpin : MonoBehaviour
    {
        public float spinSpeed = 3f;
        public bool bob;
        public bool flip = true;

        float _baseX;
        float _startY;

        void Awake()
        {
            _baseX = transform.localScale.x;
            _startY = transform.localPosition.y;
        }

        void Update()
        {
            if (flip)
            {
                var scale = transform.localScale;
                scale.x = _baseX * Mathf.Abs(Mathf.Cos(Time.time * spinSpeed));
                transform.localScale = scale;
            }

            if (bob)
            {
                var pos = transform.localPosition;
                pos.y = _startY + Mathf.Sin(Time.time * spinSpeed) * 0.1f;
                transform.localPosition = pos;
            }
        }
    }
}

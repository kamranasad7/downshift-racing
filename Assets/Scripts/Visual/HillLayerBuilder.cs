using UnityEngine;

namespace Downshift
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HillLayerBuilder : MonoBehaviour
    {
        public Transform cam;
        public float parallaxFactor = 0.15f;
        public float verticalFactor = 0.5f;
        public float baseline = 8f;
        public float amplitude = 5f;
        public float noiseScale = 0.04f;
        public float width = 200f;
        public int segments = 48;
        public float skirtBottom = -40f;
        public Color color = Color.gray;
        public int sortingOrder = -90;
        public float baseY = 0f;
        public float verticalRecenterMargin = 3f;

        Mesh _mesh;
        float _shiftX;
        float _shiftY;
        float _noiseOffset;

        void Start()
        {
            var mr = GetComponent<MeshRenderer>();
            mr.sharedMaterial = new Material(Shader.Find("Sprites/Default")) { color = color };
            mr.sortingOrder = sortingOrder;
            _mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = _mesh;

            _noiseOffset = Random.Range(0f, 1000f);
            _shiftX = 0f;
            if (cam != null)
                transform.position = new Vector3(cam.position.x * parallaxFactor, baseY, 0f);
            Rebuild();
        }

        void Rebuild()
        {
            int n = segments + 1;
            var vertices = new Vector3[n * 2];
            var colors = new Color[n * 2];
            for (int i = 0; i < n; i++)
            {
                float localX = -width / 2f + i * (width / segments);
                float sampleX = (transform.position.x + localX) * noiseScale + _noiseOffset;
                float h = baseline + (Mathf.PerlinNoise(sampleX, 0f) - 0.5f) * 2f * amplitude;
                vertices[i] = new Vector3(localX, h - baseY, 0f);
                vertices[n + i] = new Vector3(localX, skirtBottom - baseY, 0f);
                colors[i] = color;
                colors[n + i] = color;
            }

            var triangles = new int[(n - 1) * 6];
            for (int i = 0; i < n - 1; i++)
            {
                int t = i * 6;
                triangles[t] = i;
                triangles[t + 1] = i + 1;
                triangles[t + 2] = n + i;
                triangles[t + 3] = i + 1;
                triangles[t + 4] = n + i + 1;
                triangles[t + 5] = n + i;
            }

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.colors = colors;
            _mesh.RecalculateBounds();
        }

        void LateUpdate()
        {
            if (cam == null || _mesh == null) return;

            float x = cam.position.x * parallaxFactor + _shiftX;
            float y = baseY + cam.position.y * verticalFactor + _shiftY;
            transform.position = new Vector3(x, y, transform.position.z);

            bool needsXRecenter = Mathf.Abs(cam.position.x - x) > width / 2f;
            // Track descends without bound, so the camera's y drifts away from this
            // layer's damped vertical follow just as unboundedly as x does; without this
            // the ridge silhouette drifts above frame and the flat skirt fills the screen.
            bool needsYRecenter = Mathf.Abs(cam.position.y - y) > verticalRecenterMargin;

            if (needsXRecenter)
                _shiftX += Mathf.Sign(cam.position.x - x) * (width / 2f);
            if (needsYRecenter)
                _shiftY += Mathf.Sign(cam.position.y - y) * verticalRecenterMargin;

            if (needsXRecenter || needsYRecenter)
            {
                x = cam.position.x * parallaxFactor + _shiftX;
                y = baseY + cam.position.y * verticalFactor + _shiftY;
                transform.position = new Vector3(x, y, transform.position.z);
                if (needsXRecenter) Rebuild();
            }
        }
    }
}

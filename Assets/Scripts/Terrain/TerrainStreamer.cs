using UnityEngine;

namespace Downshift
{
    public class TerrainStreamer : MonoBehaviour
    {
        public TerrainConfig config;
        public Transform target;
        public int activeChunks = 3;
        public float bottomDepth = 30f;
        public float stripDepth = 0.6f;
        // Defaults are palette sRGB (#7A6A55 / #4A3F33 / ~60%surface-40%cream) pre-converted to
        // linear space: mesh vertex colors bypass Unity's automatic sRGB->linear correction that
        // material.color gets, so raw sRGB floats here would render washed out under Linear color space.
        public Color surfaceColor = new Color(0.1946f, 0.1441f, 0.0908f);
        public Color deepColor = new Color(0.0685f, 0.0497f, 0.0331f);
        public Color stripColor = new Color(0.3814f, 0.3140f, 0.2269f);
        public GameObject coinPrefab;
        public GameObject coolantPrefab;
        public GameObject stationPrefab;
        public GameObject rocksPrefab;
        public GameObject ridgePrefab;
        public GameObject washboardPrefab;
        public GameObject signPrefab;

        GameObject[] _chunks;
        int[] _chunkIndices;
        Material _groundMaterial;

        public static int ChunkIndexAt(float x, TerrainConfig c)
        {
            return Mathf.FloorToInt(x / (c.pointsPerChunk * c.pointSpacing));
        }

        void Start()
        {
            _groundMaterial = new Material(Shader.Find("Sprites/Default")) { color = Color.white };
            _chunks = new GameObject[activeChunks];
            _chunkIndices = new int[activeChunks];
            for (int i = 0; i < activeChunks; i++)
            {
                _chunks[i] = new GameObject($"Chunk{i}");
                _chunks[i].transform.SetParent(transform, false);
                _chunkIndices[i] = int.MinValue;
            }
            UpdateChunks(true);
        }

        void Update() => UpdateChunks(false);

        void UpdateChunks(bool force)
        {
            int first = Mathf.Max(0, ChunkIndexAt(target.position.x, config) - 1);
            for (int wanted = first; wanted < first + activeChunks; wanted++)
            {
                int slot = wanted % activeChunks;
                if (_chunkIndices[slot] != wanted || force)
                {
                    BuildChunk(_chunks[slot], wanted);
                    _chunkIndices[slot] = wanted;
                }
            }
        }

        void BuildChunk(GameObject go, int chunkIndex)
        {
            var heights = TerrainProfile.ChunkHeights(chunkIndex, config);
            float startX = chunkIndex * config.pointsPerChunk * config.pointSpacing;

            var filter = go.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = go.AddComponent<MeshFilter>();
                var renderer = go.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = _groundMaterial;
                renderer.sortingOrder = -10;
                go.AddComponent<EdgeCollider2D>();
                filter.sharedMesh = new Mesh();
            }

            float minY = float.MaxValue;
            for (int i = 0; i < heights.Length; i++) minY = Mathf.Min(minY, heights[i]);
            float bottomY = minY - bottomDepth;

            int n = heights.Length;
            var vertices = new Vector3[n * 3];
            var colors = new Color[n * 3];
            var colPts = new Vector2[n];
            for (int i = 0; i < n; i++)
            {
                float x = startX + i * config.pointSpacing;
                vertices[i] = new Vector3(x, heights[i], 0f);
                vertices[n + i] = new Vector3(x, heights[i] - stripDepth, 0f);
                vertices[2 * n + i] = new Vector3(x, bottomY, 0f);
                colors[i] = stripColor;
                colors[n + i] = surfaceColor;
                colors[2 * n + i] = deepColor;
                colPts[i] = vertices[i];
            }

            var triangles = new int[(n - 1) * 12];
            for (int i = 0; i < n - 1; i++)
            {
                int t = i * 12;
                triangles[t] = i;
                triangles[t + 1] = i + 1;
                triangles[t + 2] = n + i;
                triangles[t + 3] = i + 1;
                triangles[t + 4] = n + i + 1;
                triangles[t + 5] = n + i;
                triangles[t + 6] = n + i;
                triangles[t + 7] = n + i + 1;
                triangles[t + 8] = 2 * n + i;
                triangles[t + 9] = n + i + 1;
                triangles[t + 10] = 2 * n + i + 1;
                triangles[t + 11] = 2 * n + i;
            }

            var mesh = filter.sharedMesh;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            go.GetComponent<EdgeCollider2D>().points = colPts;

            var old = go.transform.Find("Spawned");
            if (old != null) Destroy(old.gameObject);
            var parent = new GameObject("Spawned").transform;
            parent.SetParent(go.transform, false);
            foreach (var (x, kind) in PickupPlacer.PlacementsForChunk(chunkIndex, config))
            {
                var prefab = kind == PickupKind.Coin ? coinPrefab : kind == PickupKind.Coolant ? coolantPrefab : stationPrefab;
                if (prefab == null) continue;
                float y = TerrainProfile.Height(x, config) + (kind == PickupKind.Station ? 1.4f : 1.2f);
                var inst = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity, parent);
                inst.SetActive(true);
            }
            foreach (var (x, kind) in HazardPlacer.PlacementsForChunk(chunkIndex, config))
            {
                var prefab = kind == HazardKind.Rocks ? rocksPrefab : kind == HazardKind.Ridge ? ridgePrefab : washboardPrefab;
                if (prefab == null) continue;
                float y = TerrainProfile.Height(x, config);
                var inst = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity, parent);
                inst.SetActive(true);
            }
            foreach (var x in HazardPlacer.SignsForChunk(chunkIndex, config))
            {
                if (signPrefab == null) continue;
                float y = TerrainProfile.Height(x, config) + 1.6f;
                var inst = Instantiate(signPrefab, new Vector3(x, y, 0f), Quaternion.identity, parent);
                inst.SetActive(true);
            }
        }
    }
}

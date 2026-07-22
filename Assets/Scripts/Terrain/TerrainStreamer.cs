using UnityEngine;

namespace Downshift
{
    public class TerrainStreamer : MonoBehaviour
    {
        public TerrainConfig config;
        public Transform target;
        public int activeChunks = 3;
        public float bottomDepth = 30f;
        public Color groundColor = new Color(0.36f, 0.28f, 0.22f);
        public GameObject coinPrefab;
        public GameObject coolantPrefab;
        public GameObject stationPrefab;

        GameObject[] _chunks;
        int[] _chunkIndices;
        Material _groundMaterial;

        public static int ChunkIndexAt(float x, TerrainConfig c)
        {
            return Mathf.FloorToInt(x / (c.pointsPerChunk * c.pointSpacing));
        }

        void Start()
        {
            _groundMaterial = new Material(Shader.Find("Sprites/Default")) { color = groundColor };
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
                go.AddComponent<MeshRenderer>().sharedMaterial = _groundMaterial;
                go.AddComponent<EdgeCollider2D>();
                filter.sharedMesh = new Mesh();
            }

            float minY = float.MaxValue;
            for (int i = 0; i < heights.Length; i++) minY = Mathf.Min(minY, heights[i]);
            float bottomY = minY - bottomDepth;

            int n = heights.Length;
            var vertices = new Vector3[n * 2];
            var colPts = new Vector2[n];
            for (int i = 0; i < n; i++)
            {
                float x = startX + i * config.pointSpacing;
                vertices[i] = new Vector3(x, heights[i], 0f);
                vertices[n + i] = new Vector3(x, bottomY, 0f);
                colPts[i] = vertices[i];
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

            var mesh = filter.sharedMesh;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            go.GetComponent<EdgeCollider2D>().points = colPts;

            var old = go.transform.Find("Pickups");
            if (old != null) Destroy(old.gameObject);
            var parent = new GameObject("Pickups").transform;
            parent.SetParent(go.transform, false);
            foreach (var (x, kind) in PickupPlacer.PlacementsForChunk(chunkIndex, config))
            {
                var prefab = kind == PickupKind.Coin ? coinPrefab : kind == PickupKind.Coolant ? coolantPrefab : stationPrefab;
                if (prefab == null) continue;
                float y = TerrainProfile.Height(x, config) + (kind == PickupKind.Station ? 1.4f : 1.2f);
                var inst = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity, parent);
                inst.SetActive(true);
            }
        }
    }
}

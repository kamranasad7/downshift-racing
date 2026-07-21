using UnityEngine;
using UnityEngine.U2D;

namespace Downshift
{
    public class TerrainStreamer : MonoBehaviour
    {
        public TerrainConfig config;
        public Transform target;
        public int activeChunks = 3;
        public SpriteShape shapeProfile;
        public float bottomDepth = 30f;

        GameObject[] _chunks;
        int[] _chunkIndices;

        public static int ChunkIndexAt(float x, TerrainConfig c)
        {
            return Mathf.FloorToInt(x / (c.pointsPerChunk * c.pointSpacing));
        }

        void Start()
        {
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
            for (int slot = 0; slot < activeChunks; slot++)
            {
                int wanted = first + slot;
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

            var controller = go.GetComponent<SpriteShapeController>();
            if (controller == null)
            {
                controller = go.AddComponent<SpriteShapeController>();
                controller.spriteShape = shapeProfile;
                go.AddComponent<EdgeCollider2D>();
            }

            var spline = controller.spline;
            spline.Clear();
            float minY = float.MaxValue;
            for (int i = 0; i < heights.Length; i++) minY = Mathf.Min(minY, heights[i]);

            var colPts = new Vector2[heights.Length];
            for (int i = 0; i < heights.Length; i++)
            {
                var p = new Vector3(startX + i * config.pointSpacing, heights[i], 0f);
                spline.InsertPointAt(i, p);
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                colPts[i] = p;
            }
            spline.InsertPointAt(heights.Length, new Vector3(startX + config.pointsPerChunk * config.pointSpacing, minY - bottomDepth, 0f));
            spline.InsertPointAt(heights.Length + 1, new Vector3(startX, minY - bottomDepth, 0f));

            go.GetComponent<EdgeCollider2D>().points = colPts;
        }
    }
}

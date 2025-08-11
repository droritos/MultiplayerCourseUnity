using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class BlastZoneGrid : MonoBehaviour
    {
        [Header("Grid Blocks")] //
        [SerializeField] private GameObject solidBlockPrefab;
        [SerializeField] private GameObject breakableBlockPrefab;
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject borderBlockPrefab;

        [Header("Grid Data")] //
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private int width = 13;
        [SerializeField] private int height = 11;

        [Header("Grid Offsets")] //
        [SerializeField] private float spacing = 1f;
        [SerializeField] private float blockYOffset = 2f; // vertical offset of blocks above the floor

        // ReSharper disable once UnusedMember.Global
        public void GenerateGrid()
        {
            ClearGrid();

            // precalc some values
            float xOffset = (width - 1) * spacing * 0.5f;
            float zOffset = (height - 1) * spacing * 0.5f;
            Vector3 upOffset = Vector3.up * blockYOffset;

            // create subobjects for later
            GameObject spawnPositions = new GameObject
            {
                name = "Spawn Positions",
                transform = { parent = transform, position = transform.position, rotation = transform.rotation }
            };
            GameObject grid = new GameObject
            {
                name = "Grid",
                transform = { parent = transform, position = transform.position, rotation = transform.rotation }
            };

            var spawnPositionsTransform = spawnPositions.transform;
            var gridTransform = grid.transform;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 pos = startPosition + new Vector3(x * spacing - xOffset, z * 0, z * spacing - zOffset);

                    // create floor tile
                    InstantiatePrefabInstance(floorPrefab, pos, Quaternion.identity, grid.transform);

                    // create spawn zone markers and skip blocks
                    pos += upOffset;
                    if (IsSpawnZone(x, z))
                    {
                        var positionMarker = new GameObject("Spawn Marker");
                        positionMarker.transform.parent = spawnPositionsTransform;
                        positionMarker.transform.SetPositionAndRotation(pos, Quaternion.identity);
                        continue;
                    }

                    // create block
                    if (IsBorder(x, z))
                    {
                        InstantiatePrefabInstance(borderBlockPrefab, pos, Quaternion.identity, gridTransform);
                    }
                    else if (IsSolidBlock(x, z))
                    {
                        InstantiatePrefabInstance(solidBlockPrefab, pos, Quaternion.identity, gridTransform);
                    }
                    else if (IsBreakableBlock())
                    {
                        InstantiatePrefabInstance(breakableBlockPrefab, pos, Quaternion.identity, gridTransform);
                    }
                }
            }
        }

        public void ClearGrid()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private GameObject InstantiatePrefabInstance(GameObject original, Vector3 position, Quaternion rotation,
            Transform parent)
        {
#if UNITY_EDITOR
            var instance = Application.isPlaying
                ? Instantiate(original)
                : UnityEditor.PrefabUtility.InstantiatePrefab(original) as GameObject;
#else
        var instance = Instantiate(original);
#endif

            var instanceTransform = instance.transform;
            instanceTransform.SetParent(parent);
            instanceTransform.SetPositionAndRotation(position, rotation);

            return instance;
        }

        // ReSharper disable once UnusedMember.Global
        public void SetCurrentPositionToStartPosition() => startPosition = this.transform.position;

        private bool IsBorder(int x, int z) => x == 0 || x == width - 1 || z == 0 || z == height - 1;

        private bool IsSpawnZone(int x, int z) =>
            (x == 1 && z == 1) ||
            (x == width - 2 && z == height - 2) ||
            (x == 1 && z == height - 2) ||
            (x == width - 2 && z == 1);

        private static bool IsBreakableBlock() => Random.value < 0.7f;

        private static bool IsSolidBlock(int x, int z) => x % 2 == 0 && z % 2 == 0;
    }
}

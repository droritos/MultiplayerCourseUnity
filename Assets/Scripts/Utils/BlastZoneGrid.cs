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
        [SerializeField] private float seed = 1;

        [Header("Grid Objects")]
        [SerializeField] private Transform gridCollidersTransform;
        [SerializeField] private Transform spawnPositionsTransform;
        [SerializeField] private Transform gridBaseTransform;
        [SerializeField] private Transform gridTransform;

        // ReSharper disable once UnusedMember.Global
        public void GenerateGridBase()
        {
            ClearGridBase();

            // precalc some values
            float xOffset = (width - 1) * spacing * 0.5f;
            float zOffset = (height - 1) * spacing * 0.5f;
            Vector3 upOffset = Vector3.up * blockYOffset;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 pos = startPosition + new Vector3(x * spacing - xOffset, z * 0, z * spacing - zOffset);

                    // create floor tile
                    InstantiatePrefabInstance(floorPrefab, pos, Quaternion.identity, gridBaseTransform);

                    // create spawn zone markers and skip blocks
                    pos += upOffset;
                    if (IsSpawnZone(x, z))
                    {
                        var positionMarker = new GameObject("Spawn Marker")
                        {
                            transform =
                            {
                                parent = spawnPositionsTransform, position = pos, rotation = Quaternion.identity
                            }
                        };
                        continue;
                    }

                    // create block
                    if (IsBorder(x, z))
                    {
                        InstantiatePrefabInstance(borderBlockPrefab, pos, Quaternion.identity, gridBaseTransform);
                    }
                    else if (IsSolidBlock(x, z))
                    {
                        InstantiatePrefabInstance(solidBlockPrefab, pos, Quaternion.identity, gridBaseTransform);
                    }
                }
            }

            SpawnColliders();
        }

        private void SpawnColliders()
        {
            // spawn floor collider
            SpawnCollider("Floor", startPosition, new Vector3(width * 2, 2, height * 2));

            // spawn floor collider
            SpawnCollider("Left", startPosition + new Vector3(-width + 0.5f*2, blockYOffset, 0), new Vector3(2, 2, height * 2));
            SpawnCollider("Right", startPosition + new Vector3(width - 0.5f*2, blockYOffset, 0), new Vector3(2, 2, height * 2));
            SpawnCollider("Top", startPosition + new Vector3(0, blockYOffset, height - 0.5f*2), new Vector3(width * 2, 2, 2));
            SpawnCollider("Bottom", startPosition + new Vector3(0, blockYOffset, -height + 0.5f*2), new Vector3(width * 2, 2, 2));
        }

        private void SpawnCollider(string colliderName, Vector3 position, Vector3 scale)
        {
            var colliderGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderGameObject.name = colliderName;
            colliderGameObject.transform.SetParent(gridCollidersTransform);
            colliderGameObject.transform.position = position;
            colliderGameObject.transform.localScale = scale;
            DestroyImmediate(colliderGameObject.GetComponent<MeshFilter>());
            DestroyImmediate(colliderGameObject.GetComponent<MeshRenderer>());
        }

        public void ClearGridBase()
        {
            for (int i = gridCollidersTransform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridCollidersTransform.GetChild(i).gameObject);
            }

            for (int i = gridBaseTransform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridBaseTransform.GetChild(i).gameObject);
            }

            for (int i = spawnPositionsTransform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(spawnPositionsTransform.GetChild(i).gameObject);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public void GenerateGridContents()
        {
            ClearGridContents();

            // precalc some values
            float xOffset = (width - 1) * spacing * 0.5f;
            float zOffset = (height - 1) * spacing * 0.5f;
            for (int x = 1; x < width-1; x++)
            {
                for (int z = 1; z < height-1; z++)
                {
                    Vector3 pos = startPosition + new Vector3(x * spacing - xOffset, blockYOffset, z * spacing - zOffset);
                    if (IsBreakableBlock(x, z))
                    {
                        InstantiatePrefabInstance(breakableBlockPrefab, pos, Quaternion.identity, gridTransform);
                    }
                }
            }
        }

        public void ClearGridContents()
        {
            for (int i = gridTransform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridTransform.GetChild(i).gameObject);
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


        private bool IsAdjacentToSpawnZone(int x, int z) =>
            (Mathf.Abs(x - 1) <= 1 && Mathf.Abs(z - 1) <= 1) ||
            (Mathf.Abs(x - (width - 2)) <= 1 && Mathf.Abs(z - (height - 2)) <= 1) ||
            (Mathf.Abs(x - 1) <= 1 && Mathf.Abs(z - (height - 2)) <= 1) ||
            (Mathf.Abs(x - (width - 2)) <= 1 && Mathf.Abs(z - 1) <= 1);

        private bool IsBreakableBlock(int x, int z) =>
            !IsAdjacentToSpawnZone(x, z) && !IsSolidBlock(x, z) && !IsBorder(x,z) && (Random.value < 0.7);

        private bool IsSolidBlock(int x, int z) => x % 2 == 0 && z % 2 == 0;
    }
}

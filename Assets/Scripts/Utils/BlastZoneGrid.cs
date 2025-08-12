using Fusion;
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

        [SerializeField] private GridData gridData;

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
            float xOffset = (gridData.width - 1) * gridData.spacing * 0.5f;
            float zOffset = (gridData.height - 1) * gridData.spacing * 0.5f;
            Vector3 upOffset = Vector3.up * gridData.blockYOffset;

            for (int x = 0; x < gridData.width; x++)
            {
                for (int z = 0; z < gridData.height; z++)
                {
                    Vector3 pos = gridData.startPosition + new Vector3(x * gridData.spacing - xOffset, z * 0, z * gridData.spacing - zOffset);

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
            SpawnCollider("Floor", gridData.startPosition, new Vector3(gridData.width * 2, 2, gridData.height * 2));

            // spawn floor collider
            SpawnCollider("Left", gridData.startPosition + new Vector3(-gridData.width + 0.5f*2, gridData.blockYOffset, 0), new Vector3(2, 2, gridData.height * 2));
            SpawnCollider("Right", gridData.startPosition + new Vector3(gridData.width - 0.5f*2, gridData.blockYOffset, 0), new Vector3(2, 2, gridData.height * 2));
            SpawnCollider("Top", gridData.startPosition + new Vector3(0, gridData.blockYOffset, gridData.height - 0.5f*2), new Vector3(gridData.width * 2, 2, 2));
            SpawnCollider("Bottom", gridData.startPosition + new Vector3(0, gridData.blockYOffset, -gridData.height + 0.5f*2), new Vector3(gridData.width * 2, 2, 2));
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
            float xOffset = (gridData.width - 1) * gridData.spacing * 0.5f;
            float zOffset = (gridData.height - 1) * gridData.spacing * 0.5f;
            for (int x = 1; x < gridData.width-1; x++)
            {
                for (int z = 1; z < gridData.height-1; z++)
                {
                    Vector3 pos = gridData.startPosition + new Vector3(x * gridData.spacing - xOffset, gridData.blockYOffset, z * gridData.spacing - zOffset);
                    if (IsBreakableBlock(x, z))
                    {
                        var block = InstantiatePrefabInstance(breakableBlockPrefab, pos, Quaternion.identity, gridTransform);
                        block.AddComponent<NetworkObject>();
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
        public void SetCurrentPositionToStartPosition() => gridData.startPosition = this.transform.position;

        private bool IsBorder(int x, int z) => x == 0 || x == gridData.width - 1 || z == 0 || z == gridData.height - 1;

        private bool IsSpawnZone(int x, int z) =>
            (x == 1 && z == 1) ||
            (x == gridData.width - 2 && z == gridData.height - 2) ||
            (x == 1 && z == gridData.height - 2) ||
            (x == gridData.width - 2 && z == 1);


        private bool IsAdjacentToSpawnZone(int x, int z) =>
            (Mathf.Abs(x - 1) <= 1 && Mathf.Abs(z - 1) <= 1) ||
            (Mathf.Abs(x - (gridData.width - 2)) <= 1 && Mathf.Abs(z - (gridData.height - 2)) <= 1) ||
            (Mathf.Abs(x - 1) <= 1 && Mathf.Abs(z - (gridData.height - 2)) <= 1) ||
            (Mathf.Abs(x - (gridData.width - 2)) <= 1 && Mathf.Abs(z - 1) <= 1);

        private bool IsBreakableBlock(int x, int z) =>
            !IsAdjacentToSpawnZone(x, z) && !IsSolidBlock(x, z) && !IsBorder(x,z) && (Random.value < 0.7);

        private bool IsSolidBlock(int x, int z) => x % 2 == 0 && z % 2 == 0;
    }
}

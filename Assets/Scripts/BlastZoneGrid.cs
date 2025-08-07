using UnityEngine;

[ExecuteInEditMode]
public class BlastZoneGrid : MonoBehaviour
{
    [Header("Grid Blocks")]
    [SerializeField] GameObject solidBlockPrefab;
    [SerializeField] GameObject breakableBlockPrefab;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject borderBlockPrefab;

    [Header("Grid Data")]
    [SerializeField] Vector3 startPosition;
    [SerializeField] int width = 13;
    [SerializeField] int height = 11;

    [Header("Grid Offsets")]
    [SerializeField] float spacing = 1f;
    [SerializeField] float heightSpacing = 0f;       // vertical height change per row
    [SerializeField] float blockYOffset = 2f;      // vertical offset of blocks above the floor

    [Header("Other")]
    [SerializeField] bool clearOnGenerate = true;


    public void GenerateGrid()
    {
        if (clearOnGenerate)
            ClearGrid();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float xOffset = (width - 1) * spacing * 0.5f;
                float zOffset = (height - 1) * spacing * 0.5f;

                Vector3 pos = startPosition + new Vector3(x * spacing - xOffset, z * heightSpacing, z * spacing - zOffset);

                if (floorPrefab)
                    Instantiate(floorPrefab, pos, Quaternion.identity, transform);

                if (IsBorder(x, z))
                {
                    Instantiate(borderBlockPrefab, pos + Vector3.up * blockYOffset, Quaternion.identity, transform);
                }
                else if (x % 2 == 0 && z % 2 == 0)
                {
                    Instantiate(solidBlockPrefab, pos + Vector3.up * blockYOffset, Quaternion.identity, transform);
                }
                else
                {
                    if (IsSpawnZone(x, z)) continue;

                    if (Random.value < 0.7f)
                        Instantiate(breakableBlockPrefab, pos + Vector3.up * blockYOffset, Quaternion.identity, transform);
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

    public void SetCurrrentPositionToStartPosition()
    {
        startPosition = this.transform.position;
    }


    private void UpdateTransform()
    {
        this.transform.position = startPosition;
    }

    private bool IsBorder(int x, int z)
    {
        return x == 0 || x == width - 1 || z == 0 || z == height - 1;
    }

    private bool IsSpawnZone(int x, int z)
    {
        return (x <= 1 && z <= 1) ||
               (x >= width - 2 && z >= height - 2) ||
               (x <= 1 && z >= height - 2) ||
               (x >= width - 2 && z <= 1);
    }
}

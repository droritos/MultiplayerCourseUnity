using UnityEngine;

[ExecuteInEditMode]
public class BlastZoneGrid : MonoBehaviour
{
    [SerializeField] GameObject solidBlockPrefab;
    [SerializeField] GameObject breakableBlockPrefab;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject borderBlockPrefab;


    [SerializeField] float yStart = -13.5f;
    [SerializeField] int width = 13;
    [SerializeField] int height = 11;
    [SerializeField] float spacing = 1f;

    [SerializeField] float heightSpacing = 0f;       // vertical height change per row
    [SerializeField] float blockYOffset = 2f;      // vertical offset of blocks above the floor

    [SerializeField] bool clearOnGenerate = true;


    public void GenerateGrid()
    {
        if (clearOnGenerate)
            ClearGrid();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * spacing, yStart + z * heightSpacing, z * spacing);

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

    bool IsBorder(int x, int z)
    {
        return x == 0 || x == width - 1 || z == 0 || z == height - 1;
    }

    bool IsSpawnZone(int x, int z)
    {
        return (x <= 1 && z <= 1) ||
               (x >= width - 2 && z >= height - 2) ||
               (x <= 1 && z >= height - 2) ||
               (x >= width - 2 && z <= 1);
    }
}

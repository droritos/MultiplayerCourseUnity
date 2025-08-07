using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlastZoneGrid))]
public class BlastZoneGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BlastZoneGrid grid = (BlastZoneGrid)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Grid"))
        {
            grid.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            grid.ClearGrid();
        }

        if (GUILayout.Button("Update Start Position"))
        {
            grid.SetCurrrentPositionToStartPosition();
        }

    }
}

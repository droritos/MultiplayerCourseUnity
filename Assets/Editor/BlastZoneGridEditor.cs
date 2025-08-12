using UnityEngine;
using UnityEditor;

namespace Game.Editor
{
    [CustomEditor(typeof(BlastZoneGrid))]
    public class BlastZoneGridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BlastZoneGrid grid = (BlastZoneGrid)target;

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Grid Contents"))
            {
                grid.GenerateGridContents();
            }

            if (GUILayout.Button("Clear Grid Contents"))
            {
                grid.ClearGridContents();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Grid Base"))
            {
                grid.GenerateGridBase();
            }

            if (GUILayout.Button("Clear Grid Base"))
            {
                grid.ClearGridBase();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Update Start Position"))
            {
                grid.SetCurrentPositionToStartPosition();
            }
        }
    }
}

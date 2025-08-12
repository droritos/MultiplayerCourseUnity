using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Grid Data Object", menuName = "Game/Grid Data")]
    public class GridData : ScriptableObject
    {
        [Header("Grid Data")]
        public Vector3 startPosition;
        public int width = 13;
        public int height = 11;

        [Header("Grid Offsets")]
        public float spacing = 1f;
        public float blockYOffset = 2f;
    }
}

using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Практика 3.1 — дані будівлі в Scriptable Object.
    /// Create → Raycast Lesson → Building Data.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Raycast Lesson/Building Data")]
    public class BuildingData : ScriptableObject
    {
        public string buildingName;
        public int cost;
        public GameObject prefab;
        public Sprite icon;
    }
}

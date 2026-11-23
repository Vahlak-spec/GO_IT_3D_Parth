using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Практика 3.1 — Raycast + Scriptable Object.
    /// Клавіші 1/2/3 перемикають поточну будівлю, ЛКМ ставить її на поверхню
    /// (hit.point + орієнтація по hit.normal). Назва поточної будівлі виводиться в UI-текст.
    /// </summary>
    public class BuildingPlacer : MonoBehaviour, IRaycastDemo
    {
        public BuildingData[] buildings;
        public BuildingData currentBuilding;

        public LayerMask buildableLayers;
        public float maxDistance = 100f;

        private int _money = 500;

        [Header("UI")]
        public TMP_Text currentLabel;

        private readonly List<GameObject> placed = new List<GameObject>();
        private int totalSpent;
        private string lastMessage = "—";

        public string Title => "3.1 Будівництво: Raycast + Scriptable Object";
        public string Hint => "[1] House, [2] Tower, [3] Wall — вибрати будівлю (дані беруться з SO). ЛКМ — поставити на будь-яку поверхню. [C] — знести все.";
        public string Status => $"Поточна: {CurrentName}   Витрачено: {totalSpent}\n{lastMessage}";

        private string CurrentName => currentBuilding != null
            ? $"{currentBuilding.buildingName} ({currentBuilding.cost})"
            : "—";

        void OnEnable()
        {
            if (currentBuilding == null && buildings != null && buildings.Length > 0)
                currentBuilding = buildings[0];
            if (currentLabel != null) currentLabel.gameObject.SetActive(true);
            UpdateLabel();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Select(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) Select(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) Select(2);

            if (Input.GetMouseButtonDown(0))
                PlaceBuilding();

            if (Input.GetKeyDown(KeyCode.C))
                Clear();
        }

        void Select(int index)
        {
            if (buildings == null || index >= buildings.Length) return;
            currentBuilding = buildings[index];
            UpdateLabel();
        }

        void PlaceBuilding()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, buildableLayers))
            {


                Vector3 pos = hit.point;
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);

                GameObject gameObject = Instantiate(currentBuilding.prefab, pos, rot);

                placed.Add(gameObject);

                totalSpent += currentBuilding.cost;

                lastMessage = $"Збудовано {currentBuilding.buildingName} за {currentBuilding.cost}";
                Debug.Log(lastMessage);
            }
        }

        void UpdateLabel()
        {
            if (currentLabel != null)
                currentLabel.text = "Будівля: " + CurrentName;
        }

        void Clear()
        {
            foreach (var go in placed)
                if (go != null) Destroy(go);

            placed.Clear();
            totalSpent = 0;
            lastMessage = "Все знесено";
        }

        void OnDisable()
        {
            Clear();
            if (currentLabel != null) currentLabel.gameObject.SetActive(false);
        }
    }
}

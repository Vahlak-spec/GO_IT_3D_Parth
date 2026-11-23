using System.Collections.Generic; // в уроці цього using немає — без нього List<> не скомпілюється
using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Практика 2.1 — Спавн дерев по ландшафту.
    /// Розширений спавнер з уроку: ліміт, мінімальна відстань і попередній перегляд («привид»).
    /// Доповнення демо-сцени: привид зеленіє/червоніє залежно від того, чи можна тут садити.
    /// Важливо: у привида НЕ має бути колайдерів, інакше промінь влучатиме в нього самого.
    /// </summary>
    public class AdvancedSpawner : MonoBehaviour, IRaycastDemo
    {
        [Header("Префаб")]
        public GameObject prefabToSpawn;

        [Header("Обмеження")]
        public int maxObjects = 15;
        public float minDistanceBetween = 2f; // мінімальна відстань між об'єктами
        public LayerMask spawnableLayers;

        [Header("Попередній перегляд")]
        public GameObject previewObject;         // напівпрозорий "привид" об'єкта
        public Color previewOkColor = new Color(0.3f, 1f, 0.4f, 0.4f);
        public Color previewBlockedColor = new Color(1f, 0.25f, 0.2f, 0.4f);

        private int spawnedCount = 0;
        private List<Vector3> spawnedPositions = new List<Vector3>();
        private readonly List<GameObject> spawnedObjects = new List<GameObject>();
        private Renderer[] previewRenderers;
        private string lastMessage = "—";

        public string Title => "2.1 Посадка дерев (AdvancedSpawner)";
        public string Hint => "ЛКМ — посадити дерево. Ліміт 15, не ближче 2 м одне до одного. Привид показує, де з'явиться дерево; червоний — тут садити не можна. [C] — очистити.";
        public string Status => $"Дерев: {spawnedCount}/{maxObjects}\n{lastMessage}";

        void Awake()
        {
            if (previewObject != null)
                previewRenderers = previewObject.GetComponentsInChildren<Renderer>(true);
        }

        void Update()
        {
            HandlePreview();

            if (Input.GetMouseButtonDown(0))
                TrySpawn();

            if (Input.GetKeyDown(KeyCode.C))
                Clear();
        }

        void HandlePreview()
        {
            if (previewObject == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, spawnableLayers))
            {
                previewObject.SetActive(true);
                previewObject.transform.position = hit.point;
                previewObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                bool canPlace = spawnedCount < maxObjects && !IsTooClose(hit.point) && hit.normal.y >= 0.7f;

                if(canPlace)
                    TintPreview(previewOkColor);
                else
                    TintPreview(previewBlockedColor);
            }
            else
            {
                previewObject.SetActive(false);
            }
        }

        private void TrySpawn()
        {
            if (spawnedCount >= maxObjects)
            {
                lastMessage = "Досягнуто максимум об'єктів: " + maxObjects;
                Debug.Log(lastMessage);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, spawnableLayers))
            {
                if(hit.normal.y < 0.7f)
                {
                    lastMessage = "Занадто крутий нахил поверхні!";
                    Debug.Log(lastMessage);
                    return;
                }

                // Перевіряємо відстань до існуючих об'єктів
                if (IsTooClose(hit.point))
                {
                    lastMessage = "Занадто близько до іншого об'єкта!";
                    Debug.Log(lastMessage);
                    return;
                }

                Vector3 pos = hit.point;
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);

                spawnedObjects.Add(Instantiate(prefabToSpawn, pos, rot));
                spawnedPositions.Add(pos);
                spawnedCount++;

                lastMessage = $"Заспавнено {spawnedCount}/{maxObjects}";
                Debug.Log(lastMessage);
            }
        }

        private bool IsTooClose(Vector3 newPosition)
        {
            foreach (Vector3 existingPos in spawnedPositions)
            {
                if (Vector3.Distance(newPosition, existingPos) < minDistanceBetween)
                    return true;
            }
            return false;
        }

        private void TintPreview(Color color)
        {
            if (previewRenderers == null) return;
            foreach (var r in previewRenderers)
                r.material.color = color;
        }

        private void Clear()
        {
            foreach (var go in spawnedObjects)
                if (go != null) Destroy(go);
            spawnedObjects.Clear();
            spawnedPositions.Clear();
            spawnedCount = 0;
            lastMessage = "Очищено";
        }

        private void OnDisable()
        {
            Clear();
            if (previewObject != null) previewObject.SetActive(false);
        }
    }
}

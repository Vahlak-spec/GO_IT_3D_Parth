using System.Collections.Generic;
using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Блок «Спавн об'єктів по поверхні»: базовий спавн по кліку.
    /// alignToNormal = false — версія з уроку з Quaternion.identity (на схилі об'єкт «провалюється»);
    /// alignToNormal = true  — версія з орієнтацією по нормалі і зміщенням + hit.normal * 0.1f.
    /// Клавіша N перемикає режим, C — очистити.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour, IRaycastDemo
    {
        [Header("Що спавнити")]
        public GameObject prefabToSpawn;

        [Header("Налаштування")]
        public float maxDistance = 100f;
        public LayerMask spawnableLayers; // на яких поверхнях можна спавнити
        public bool alignToNormal = false;

        private readonly List<GameObject> spawned = new List<GameObject>();
        private string lastMessage = "—";

        public string Title => "Спавн по кліку: identity vs нормаль";
        public string Hint => "ЛКМ — поставити кубик. [N] — перемкнути режим, [C] — очистити. Порівняй, як кубики стоять на рампі й стіні в обох режимах.";
        public string Status => (alignToNormal
                                    ? "Режим: FromToRotation(Vector3.up, hit.normal) + зміщення"
                                    : "Режим: Quaternion.identity (як у першому прикладі)")
                                + "\n" + lastMessage;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N)) alignToNormal = !alignToNormal;
            if (Input.GetKeyDown(KeyCode.C)) Clear();

            if (Input.GetMouseButtonDown(0))
            {
                SpawnAtClick();
            }
        }

        void SpawnAtClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, spawnableLayers))
            {
                GameObject newObj;

                if (alignToNormal)
                {
                    // Обчислюємо поворот по нормалі поверхні
                    Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    // Трохи піднімаємо над поверхнею щоб не застрягав всередині
                    Vector3 spawnPosition = hit.point;
                    newObj = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
                }
                else
                {
                    // Спавнимо точно в точку зіткнення без повороту
                    newObj = Instantiate(prefabToSpawn, hit.point, Quaternion.identity);
                }

                spawned.Add(newObj);
                lastMessage = "Заспавнено " + prefabToSpawn.name + " на " + hit.transform.name;
                Debug.Log(lastMessage);
            }
        }

        void Clear()
        {
            foreach (var go in spawned)
                if (go != null) Destroy(go);

            spawned.Clear();
            lastMessage = "Очищено";
        }

        void OnDisable() => Clear();
    }
}

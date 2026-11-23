using System.Collections.Generic;
using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Сценарій 1 — Стрільба з перевіркою пробивання.
    /// Physics.RaycastAll знаходить УСІ об'єкти на шляху, сортуємо їх за відстанню
    /// і обробляємо не більше shotPower штук.
    /// Для Game View додано трасер від «ствола» і жовті мітки в кожній точці влучання.
    /// </summary>
    public class PenetratingShot : MonoBehaviour, IRaycastDemo
    {
        [Tooltip("Скільки об'єктів може пробити куля")]
        public int shotPower = 3;
        public int damage = 25;

        [Header("Візуалізація (демо)")]
        public Transform muzzle;
        public RayLine tracer;
        public Material impactMaterial;

        [Header("Мішені — відновлюються при вході в демку і по [R]")]
        public Health[] targets;

        private string lastShot = "—";

        public string Title => "Сценарій 1: пробивна куля (RaycastAll)";
        public string Hint => "ЛКМ — постріл. Куля пробиває до 3 об'єктів і знімає кожному по 25 HP. [↑]/[↓] — змінити shotPower, [R] — відновити мішені.";
        public string Status => $"shotPower: {shotPower}\n{lastShot}";

        void OnEnable() => ResetTargets();

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) shotPower = Mathf.Min(shotPower + 1, 10);
            if (Input.GetKeyDown(KeyCode.DownArrow)) shotPower = Mathf.Max(shotPower - 1, 1);
            if (Input.GetKeyDown(KeyCode.R)) ResetTargets();
        }

        void Shoot()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Знаходимо ВСІ об'єкти на шляху
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            // Сортуємо за відстанню (найближчий першим) — RaycastAll НЕ гарантує порядок
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            int penetrationsLeft = shotPower;
            var names = new List<string>();
            Vector3 lastPoint = ray.origin + ray.direction * 100f;

            foreach (RaycastHit hit in hits)
            {
                if (penetrationsLeft <= 0) break;

                Debug.Log($"Влучили в {hit.transform.name} на відстані {hit.distance:F1}м");
                names.Add(hit.transform.name);
                SpawnImpact(hit.point);
                lastPoint = hit.point;

                // Якщо є Health — наносимо шкоду

                if(hit.transform.TryGetComponent<Health>(out var health))
                {
                    health.TakeDamage(damage);
                }

                penetrationsLeft--;
            }

            if (tracer != null && muzzle != null)
                tracer.Show(muzzle.position, lastPoint, new Color(1f, 0.85f, 0.3f), 0.12f);

            lastShot = names.Count > 0
                ? $"Влучання ({names.Count}): " + string.Join(" → ", names)
                : "Промах";
        }

        void SpawnImpact(Vector3 point)
        {
            var mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(mark.GetComponent<Collider>()); // мітка не повинна ловити наступні промені
            mark.transform.position = point;
            mark.transform.localScale = Vector3.one * 0.15f;
            if (impactMaterial != null) mark.GetComponent<Renderer>().sharedMaterial = impactMaterial;
            Destroy(mark, 1.5f);
        }

        void ResetTargets()
        {
            if (targets == null) return;
            foreach (var t in targets)
                if (t != null) t.ResetHealth();
            lastShot = "Мішені відновлено";
        }
    }
}

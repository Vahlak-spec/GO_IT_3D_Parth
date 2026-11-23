using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Практика 1.1 — Бачимо нормаль.
    /// Зелена лінія — від камери до hit.point, синя — hit.normal (обидві в Scene View).
    /// У Game View нормаль показує стрілка-маркер, повернута через Quaternion.FromToRotation.
    /// ЛКМ — вивести hit.normal у Console.
    /// </summary>
    public class NormalVisualizer : MonoBehaviour, IRaycastDemo
    {
        public float maxDistance = 100f;
        [Tooltip("Стрілка, яка ставиться в hit.point і дивиться вздовж нормалі (без колайдерів!)")]
        public Transform normalMarker;

        public string Title => "1.1 hit.point і hit.normal";
        public string Hint => "Поводь мишею по підлозі, рампі, куполу і стіні: стрілка показує нормаль. ЛКМ — вивести hit.normal у Console. У Scene View видно зелений промінь і синю нормаль.";
        public string Status { get; private set; } = "—";

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                // Промінь від камери до точки зіткнення — зелений
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                // Нормаль поверхні в точці зіткнення — синій
                Debug.DrawRay(hit.point, hit.normal, Color.blue);

                if (normalMarker != null)
                {
                    normalMarker.gameObject.SetActive(true);
                    normalMarker.position = hit.point;
                    normalMarker.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }

                Status = $"{hit.transform.name}\npoint:  {hit.point:F2}\nnormal: {hit.normal:F3}";

                if (Input.GetMouseButtonDown(0))
                    Debug.Log($"{hit.transform.name}: hit.normal = {hit.normal:F3}");
            }
            else
            {
                if (normalMarker != null) normalMarker.gameObject.SetActive(false);
                Status = "Промінь нічого не знайшов";
            }
        }

        void OnDisable()
        {
            if (normalMarker != null) normalMarker.gameObject.SetActive(false);
            Status = "—";
        }
    }
}

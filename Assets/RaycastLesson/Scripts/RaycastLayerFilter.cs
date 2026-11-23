using UnityEngine;

namespace RaycastLesson
{

    public class RaycastLayerFilter : MonoBehaviour, IRaycastDemo
    {
        [Tooltip("Які шари бачить промінь. Якщо порожньо — береться шар \"Clickable\".")]
        public LayerMask mask;
        public float maxDistance = 100f;
        public Color hoverColor = Color.yellow;
        public Color clickColor = Color.red;

        private Renderer lastHovered;
        private Color lastOriginalColor;

        public string Title => "3.1 LayerMask і візуалізація";
        public string Hint => "Реагують тільки кубики на шарі Clickable. Сірий Obstacle, підлога і решта ігноруються — промінь пролітає крізь них. Відкрий Scene View, щоб побачити промінь.";
        public string Status { get; private set; } = "—";

        void Awake()
        {
            if (mask.value == 0)
                mask = LayerMask.GetMask("Clickable");
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, mask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                Debug.DrawRay(hit.point, hit.normal * 2f, Color.blue);

                Renderer rend = hit.transform.GetComponent<Renderer>();
                if (rend != null && rend != lastHovered)
                {
                    RestoreLastColor();
                    lastHovered = rend;
                    lastOriginalColor = rend.material.color;
                    rend.material.color = hoverColor;
                }

                if (Input.GetMouseButtonDown(0) && rend != null)
                {
                    rend.material.color = clickColor;
                    Debug.Log("Знайдено інтерактивний об'єкт: " + hit.transform.name);
                }

                Status = $"{hit.transform.name}   point: {hit.point:F1}   normal: {hit.normal:F1}";
            }
            else
            {
                // Промінь нічого не знайшов на потрібних шарах — малюємо його червоним на всю довжину
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
                RestoreLastColor();
                lastHovered = null;
                Status = "На шарі Clickable нічого немає";
            }
        }

        void RestoreLastColor()
        {
            if (lastHovered != null)
                lastHovered.material.color = lastOriginalColor;
        }

        void OnDisable()
        {
            RestoreLastColor();
            lastHovered = null;
            Status = "—";
        }
    }
}

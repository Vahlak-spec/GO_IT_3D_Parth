using UnityEngine;

namespace RaycastLesson
{
    /// <summary>
    /// Практика 2.1 — Підсвічування і клік.
    /// Наведення — жовтий, лівий клік — червоний, правий клік — оригінальний колір.
    /// Вішається на камеру. На об'єктах мають бути Collider і Renderer.
    /// </summary>
    public class RaycastColorChanger : MonoBehaviour, IRaycastDemo
    {
        [Header("Кольори")]
        public Color hoverColor = Color.yellow;   // при наведенні
        public Color clickColor = Color.red;      // при кліку

        private GameObject lastHovered;           // попередній об'єкт
        private Color lastOriginalColor;          // його оригінальний колір

        public string Title => "2.1 Підсвічування і клік";
        public string Hint => "Наведення — жовтий, ЛКМ — червоний, ПКМ — повернути оригінальний колір.";
        public string Status { get; private set; } = "—";

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Renderer rend = hit.transform.GetComponent<Renderer>();

                if (rend != null)
                {
                    // Новий об'єкт під курсором — відновлюємо попередній
                    if (lastHovered != hit.transform.gameObject)
                    {
                        RestoreLastColor();
                        lastHovered = hit.transform.gameObject;
                        lastOriginalColor = rend.material.color;
                        rend.material.color = hoverColor;
                        Status = "Наведено: " + hit.transform.name;
                    }

                    // Лівий клік — фарбуємо в clickColor
                    if (Input.GetMouseButtonDown(0))
                    {
                        rend.material.color = clickColor;
                        Debug.Log("Клік по: " + hit.transform.name);
                        Status = "Клік по: " + hit.transform.name;
                    }

                    // Правий клік — повертаємо оригінальний колір (доповнення з практики)
                    if (Input.GetMouseButtonDown(1))
                    {
                        rend.material.color = lastOriginalColor;
                        Debug.Log("Скинуто колір: " + hit.transform.name);
                        Status = "Скинуто колір: " + hit.transform.name;
                    }
                }
            }
            else
            {
                // Курсор не на жодному об'єкті
                RestoreLastColor();
                lastHovered = null;
                Status = "Курсор не на об'єкті";
            }
        }

        void RestoreLastColor()
        {
            if (lastHovered == null) return;

            Renderer rend = lastHovered.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = lastOriginalColor;
        }

        void OnDisable()
        {
            RestoreLastColor();
            lastHovered = null;
            Status = "—";
        }
    }
}

using UnityEngine;

namespace RaycastLesson
{
    /// <summary>
    /// Практика 1.1 — Перший Raycast.
    /// Кидаємо промінь від камери через позицію миші й виводимо ім'я та відстань до об'єкта.
    /// Вішається на камеру.
    /// </summary>
    public class RaycastBasic : MonoBehaviour, IRaycastDemo
    {
        public string Title => "1.1 Перший Raycast";
        public string Hint => "Поводь мишею по об'єктах. Сфера без Collider не реагує — промінь летить крізь неї.";
        public string Status { get; private set; } = "—";

        private Transform lastLogged;

        void Update()
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Status = $"Під курсором: {hit.transform.name}   Відстань: {hit.distance:F2} м";

                if (lastLogged != hit.transform)
                {
                    lastLogged = hit.transform;
                    Debug.Log("Під курсором: " + hit.transform.name);
                    Debug.Log("Відстань: " + hit.distance.ToString("F2") + "м");
                }
            }
            else
            {
                Status = "Промінь нічого не знайшов";
                lastLogged = null;
            }
        }

        void OnDisable()
        {
            lastLogged = null;
            Status = "—";
        }
    }
}

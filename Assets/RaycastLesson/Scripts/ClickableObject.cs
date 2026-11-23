using UnityEngine;

namespace RaycastLesson
{
    /// <summary>
    /// Блок «OnMouseDown — простіша альтернатива».
    /// Unity сама викликає OnMouseDown / OnMouseEnter / OnMouseExit при роботі мишею з колайдером —
    /// без жодного Raycast у нашому коді. Скрипт вішається на КОЖЕН об'єкт.
    /// Потрібні: Collider на об'єкті і камера з тегом MainCamera.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ClickableObject : MonoBehaviour
    {
        public static string LastEvent = "—";

        private Renderer rend;
        private Color originalColor;

        void Awake()
        {
            rend = GetComponent<Renderer>();
        }

        void OnMouseDown()
        {
            if (!enabled) return;
            Debug.Log(gameObject.name + " — натиснуто!");
            LastEvent = gameObject.name + " — натиснуто!";
        }

        void OnMouseEnter()
        {
            if (!enabled || rend == null) return;
            originalColor = rend.material.color;
            rend.material.color = Color.yellow;
            LastEvent = "Наведено: " + gameObject.name;
        }

        void OnMouseExit()
        {
            if (!enabled || rend == null) return;
            rend.material.color = originalColor;
        }
    }
}

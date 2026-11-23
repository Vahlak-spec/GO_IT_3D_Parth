using UnityEngine;

namespace RaycastLesson
{
    /// <summary>
    /// Обгортка для демо OnMouseDown: вмикає/вимикає ClickableObject на об'єктах сцени,
    /// щоб ця демка не заважала Raycast-демкам.
    /// </summary>
    public class MouseEventsDemo : MonoBehaviour, IRaycastDemo
    {
        public ClickableObject[] targets;

        public string Title => "OnMouseDown / Enter / Exit";
        public string Hint => "Без Raycast у коді: Unity сама викликає методи на об'єкті. Скрипт ClickableObject висить на кожному об'єкті окремо.";
        public string Status => ClickableObject.LastEvent;

        void OnEnable() => SetTargets(true);

        void OnDisable()
        {
            SetTargets(false);
            ClickableObject.LastEvent = "—";
        }

        void SetTargets(bool value)
        {
            if (targets == null) return;
            foreach (var t in targets)
                if (t != null) t.enabled = value;
        }
    }
}

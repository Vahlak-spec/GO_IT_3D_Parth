using System.Text;
using UnityEngine;

namespace SOPractice
{
    /// <summary>
    /// Обв'язка однієї демки: назва/підказка для HUD, вмикання потрібних компонентів на час демки
    /// (наприклад, ClickableItem на предметах), закриття панелі при виході з демки.
    /// </summary>
    public class DemoInfo : MonoBehaviour, IDemo
    {
        public string title;
        [TextArea(2, 5)] public string hint;

        [Tooltip("Компоненти поза цим об'єктом, які працюють лише в цій демці")]
        public Behaviour[] enableWhileActive;
        [Tooltip("UI-об'єкти, які видно лише в цій демці (кнопки тощо)")]
        public GameObject[] showWhileActive;
        public ItemInfoPanel panel;

        public string Title => title;
        public string Hint => hint;

        public string Status
        {
            get
            {
                var sb = new StringBuilder();
                if (panel != null)
                {
                    sb.Append(panel.CurrentItem != null
                        ? $"Панель показує: {panel.CurrentItem.itemName} ({panel.CurrentItem.name}.asset)"
                        : "Панель прихована");
                }
                foreach (var s in GetComponentsInChildren<IStatusSource>())
                {
                    string st = s.Status;
                    if (string.IsNullOrEmpty(st)) continue;
                    if (sb.Length > 0) sb.Append('\n');
                    sb.Append(st);
                }
                return sb.Length > 0 ? sb.ToString() : "—";
            }
        }

        void OnEnable() => Toggle(true);

        void OnDisable()
        {
            Toggle(false);
            if (panel != null && panel.gameObject.activeSelf) panel.HidePanel();
        }

        void Toggle(bool value)
        {
            if (enableWhileActive != null)
                foreach (var b in enableWhileActive)
                    if (b != null) b.enabled = value;
            if (showWhileActive != null)
                foreach (var go in showWhileActive)
                    if (go != null) go.SetActive(value);
        }
    }
}

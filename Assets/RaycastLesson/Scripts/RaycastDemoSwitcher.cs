using UnityEngine;

namespace RaycastLesson
{

    public class RaycastDemoSwitcher : MonoBehaviour
    {
        [Tooltip("Компоненти, що реалізують IRaycastDemo")]
        public MonoBehaviour[] demos;

        [Tooltip("Рендерери, яким повертаємо початковий матеріал при перемиканні")]
        public Renderer[] resettable;

        private Material[] originalMaterials;
        private int current;
        private GUIStyle boxStyle;

        void Start()
        {
            originalMaterials = new Material[resettable.Length];
            for (int i = 0; i < resettable.Length; i++)
                originalMaterials[i] = resettable[i] != null ? resettable[i].sharedMaterial : null;

            Select(0);
        }

        void Update()
        {
            for (int i = 0; i < demos.Length && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
                    Select(i);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) Select((current + 1) % demos.Length);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) Select((current - 1 + demos.Length) % demos.Length);
        }

        public void Select(int index)
        {
            if (demos == null || demos.Length == 0) return;
            current = Mathf.Clamp(index, 0, demos.Length - 1);

            foreach (var d in demos)
                if (d != null) d.enabled = false;

            for (int i = 0; i < resettable.Length; i++)
                if (resettable[i] != null) resettable[i].sharedMaterial = originalMaterials[i];

            if (demos[current] != null) demos[current].enabled = true;
        }

        void OnGUI()
        {
            if (demos == null || demos.Length == 0) return;

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.UpperLeft,
                    wordWrap = true,
                    richText = true,
                    fontSize = 15,
                    padding = new RectOffset(12, 12, 10, 10)
                };
            }

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < demos.Length; i++)
            {
                var demo = demos[i] as IRaycastDemo;
                string title = demo != null ? demo.Title : demos[i]?.GetType().Name;
                sb.AppendLine(i == current ? $"<b><color=#ffd54a>[{i + 1}] {title}</color></b>" : $"[{i + 1}] {title}");
            }

            var active = demos[current] as IRaycastDemo;
            if (active != null)
            {
                sb.AppendLine();
                sb.AppendLine(active.Hint);
                sb.AppendLine();
                sb.Append("<b>").Append(active.Status).Append("</b>");
            }

            GUI.Box(new Rect(10, 10, 520, 230), sb.ToString(), boxStyle);
        }
    }
}

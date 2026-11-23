using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Перемикач демок уроку «Raycast — розширений рівень».
    /// F1–F6 або [ / ] (а також PageUp / PageDown). Кожна демка — окремий GameObject,
    /// активний лише один. Камера плавно перелітає до станції потрібної демки.
    /// Цифри 1–3 і стрілки ↑/↓ зайняті самими демками, тому тут не використовуються.
    /// </summary>
    public class DemoSwitcher : MonoBehaviour
    {
        public GameObject[] demos;
        [Tooltip("Позиція камери для кожної демки (той самий індекс)")]
        public Transform[] cameraStations;
        public float moveSharpness = 4f;

        private int current;
        private Transform cam;
        private GUIStyle boxStyle;

        void Start()
        {
            cam = Camera.main.transform;
            Select(0, instant: true);
        }

        void Update()
        {
            for (int i = 0; i < demos.Length && i < 12; i++)
                if (Input.GetKeyDown(KeyCode.F1 + i)) Select(i);

            if (Input.GetKeyDown(KeyCode.RightBracket) || Input.GetKeyDown(KeyCode.PageDown))
                Select((current + 1) % demos.Length);
            if (Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetKeyDown(KeyCode.PageUp))
                Select((current - 1 + demos.Length) % demos.Length);

            Transform target = StationFor(current);
            if (target != null && cam != null)
            {
                // Експоненційне згладжування не залежить від FPS
                float k = 1f - Mathf.Exp(-moveSharpness * Time.deltaTime);
                cam.position = Vector3.Lerp(cam.position, target.position, k);
                cam.rotation = Quaternion.Slerp(cam.rotation, target.rotation, k);
            }
        }

        public void Select(int index, bool instant = false)
        {
            if (demos == null || demos.Length == 0) return;
            current = Mathf.Clamp(index, 0, demos.Length - 1);

            for (int i = 0; i < demos.Length; i++)
                if (demos[i] != null && i != current) demos[i].SetActive(false);
            if (demos[current] != null) demos[current].SetActive(true);

            Transform target = StationFor(current);
            if (instant && target != null && cam != null)
                cam.SetPositionAndRotation(target.position, target.rotation);
        }

        Transform StationFor(int index) =>
            cameraStations != null && index < cameraStations.Length ? cameraStations[index] : null;

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
            sb.AppendLine("<b>Raycast — розширений рівень</b>   (F1–F6 або [ / ])");
            for (int i = 0; i < demos.Length; i++)
            {
                var demo = demos[i] != null ? demos[i].GetComponent<IRaycastDemo>() : null;
                string title = demo != null ? demo.Title : demos[i]?.name;
                sb.AppendLine(i == current
                    ? $"<b><color=#ffd54a>[F{i + 1}] {title}</color></b>"
                    : $"[F{i + 1}] {title}");
            }

            var active = demos[current] != null ? demos[current].GetComponent<IRaycastDemo>() : null;
            if (active != null)
            {
                sb.AppendLine();
                sb.AppendLine(active.Hint);
                sb.AppendLine();
                sb.Append("<b>").Append(active.Status).Append("</b>");
            }

            GUI.Box(new Rect(10, 10, 580, 330), sb.ToString(), boxStyle);
        }
    }
}

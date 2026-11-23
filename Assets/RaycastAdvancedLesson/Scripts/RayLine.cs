using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Допоміжний скрипт демо-сцени: Debug.DrawLine видно лише в Scene View,
    /// тож для Game View малюємо ту саму лінію через LineRenderer.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class RayLine : MonoBehaviour
    {
        private LineRenderer line;
        private float hideAt = -1f;

        void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.enabled = false;
        }

        /// <param name="duration">0 — показувати, доки не викличуть Hide()</param>
        public void Show(Vector3 from, Vector3 to, Color color, float duration = 0f)
        {
            if (line == null) Awake();
            line.enabled = true;
            line.SetPosition(0, from);
            line.SetPosition(1, to);
            line.startColor = color;
            line.endColor = color;
            hideAt = duration > 0f ? Time.time + duration : -1f;
        }

        public void Hide()
        {
            if (line != null) line.enabled = false;
            hideAt = -1f;
        }

        void Update()
        {
            if (hideAt > 0f && Time.time >= hideAt) Hide();
        }

        void OnDisable() => Hide();
    }
}

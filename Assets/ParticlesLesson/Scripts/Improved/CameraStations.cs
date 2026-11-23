using System;
using TMPro;
using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>
    /// Допоміжний скрипт для демонстрації: камера плавно перелітає між «станціями» з ефектами.
    /// Клавіші 1–9 або стрілки ←/→, а також кнопки UI (Prev / Next).
    /// </summary>
    public class CameraStations : MonoBehaviour
    {
        [Serializable]
        public class Station
        {
            public string title;
            public Transform point;
        }

        public Station[] stations;
        public TMP_Text titleLabel;
        [Tooltip("Чим більше — тим швидше камера долітає")]
        public float moveSharpness = 4f;

        private int current;

        void Start()
        {
            Select(0, instant: true);
        }

        void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            for (int i = 0; i < stations.Length && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i)) Select(i);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) Next();
            if (Input.GetKeyDown(KeyCode.LeftArrow)) Prev();
#endif
            if (stations.Length == 0 || stations[current].point == null) return;

            // Експоненційне згладжування не залежить від FPS
            float k = 1f - Mathf.Exp(-moveSharpness * Time.deltaTime);
            Transform target = stations[current].point;
            transform.position = Vector3.Lerp(transform.position, target.position, k);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, k);
        }

        public void Next() => Select((current + 1) % stations.Length);
        public void Prev() => Select((current - 1 + stations.Length) % stations.Length);

        public void Select(int index, bool instant = false)
        {
            if (stations == null || stations.Length == 0) return;
            current = Mathf.Clamp(index, 0, stations.Length - 1);

            if (titleLabel != null)
            {
                titleLabel.text = $"{current + 1}/{stations.Length} — {stations[current].title}";
            }

            if (instant && stations[current].point != null)
            {
                transform.SetPositionAndRotation(stations[current].point.position, stations[current].point.rotation);
            }
        }
    }
}

using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>
    /// Рухає об'єкт по колу навколо його стартової позиції.
    /// Потрібен для демонстрацій, де емітер мусить рухатися: комета (Rate over Distance, Inherit Velocity),
    /// чорна діра (Force Field тягне частинки за собою), персонаж з аурою (Skinned Mesh у World Space).
    /// </summary>
    public class OrbitMover : MonoBehaviour
    {
        public float radius = 3f;
        [Tooltip("Радіан за секунду (6.28 ≈ один оберт за секунду)")]
        public float angularSpeed = 1.2f;
        [Tooltip("true — коло в площині XZ (по землі), false — у площині XY (обличчям до камери)")]
        public bool horizontal = false;
        [Tooltip("Повертати об'єкт обличчям у напрямку руху")]
        public bool faceDirection = false;

        [Header("Пауза (щоб показати, що буде, коли емітер стоїть)")]
        [Tooltip("Скільки секунд рухатися. 0 — рухатися завжди")]
        public float moveTime = 0f;
        public float pauseTime = 0f;

        private Vector3 center;
        private float angle;
        private float timer;
        private bool paused;

        void Awake()
        {
            center = transform.position;
            ApplyPosition();
        }

        void Update()
        {
            if (moveTime > 0f && pauseTime > 0f)
            {
                timer += Time.deltaTime;
                if (timer >= (paused ? pauseTime : moveTime))
                {
                    timer = 0f;
                    paused = !paused;
                }
            }

            if (!paused) angle += angularSpeed * Time.deltaTime;
            ApplyPosition();
        }

        private void ApplyPosition()
        {
            Vector3 offset = horizontal
                ? new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius
                : new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            transform.position = center + offset;

            if (faceDirection && radius > 0f)
            {
                Vector3 tangent = horizontal
                    ? new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle))
                    : new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0f);
                if (angularSpeed < 0f) tangent = -tangent;
                transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
            }
        }
    }
}

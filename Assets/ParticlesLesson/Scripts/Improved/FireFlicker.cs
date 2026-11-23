using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>
    /// Мерехтіння світла від вогню. Вогонь без світла — це «наклейка»:
    /// точкове світло, що тремтить у такт полум'ю, освітлює землю й об'єкти навколо.
    /// Perlin Noise дає плавне, а не «стробоскопне» мерехтіння, як у Random.Range.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class FireFlicker : MonoBehaviour
    {
        [Tooltip("Середня яскравість світла")]
        public float baseIntensity = 2.5f;

        [Tooltip("Наскільки сильно гуляє яскравість (0.3 = ±30%)")]
        [Range(0f, 1f)] public float intensityAmplitude = 0.35f;

        [Tooltip("Швидкість мерехтіння")]
        public float speed = 6f;

        [Tooltip("Легке тремтіння позиції світла — тіні «танцюють»")]
        public float positionJitter = 0.05f;

        private Light fireLight;
        private Vector3 basePosition;
        private float seed;

        void Awake()
        {
            fireLight = GetComponent<Light>();
            basePosition = transform.localPosition;
            seed = Random.value * 100f; // кожне багаття мерехтить по-своєму
        }

        void Update()
        {
            float t = Time.time * speed;

            // PerlinNoise повертає 0..1 → переводимо в -1..1
            float n = Mathf.PerlinNoise(seed, t) * 2f - 1f;
            fireLight.intensity = baseIntensity * (1f + n * intensityAmplitude);

            if (positionJitter > 0f)
            {
                Vector3 offset = new Vector3(
                    Mathf.PerlinNoise(seed + 10f, t) - 0.5f,
                    Mathf.PerlinNoise(seed + 20f, t) - 0.5f,
                    Mathf.PerlinNoise(seed + 30f, t) - 0.5f);
                transform.localPosition = basePosition + offset * (positionJitter * 2f);
            }
        }
    }
}

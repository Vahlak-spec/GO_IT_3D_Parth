using System.Collections;
using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>
    /// Покращене керування снігом:
    ///  • снігопад починається і закінчується ПЛАВНО (емісія росте/спадає за fadeDuration),
    ///    а не «вмикається рубильником»;
    ///  • керує кількома шарами снігу одразу (близькі пластівці + дрібний далекий сніг);
    ///  • (опційно) WindZone з поривами вітру — частинки реагують на нього через модуль External Forces.
    /// </summary>
    public class SnowWeather : MonoBehaviour
    {
        [Tooltip("Усі шари снігу, якими керуємо")]
        public ParticleSystem[] snowLayers;

        [Tooltip("За скільки секунд снігопад набирає / втрачає силу")]
        public float fadeDuration = 2.5f;

        [Header("Вітер (необов'язково)")]
        public WindZone wind;
        [Tooltip("Базова сила вітру")]
        public float windStrength = 0.4f;
        [Tooltip("Наскільки сильні пориви поверх базового вітру")]
        public float gustStrength = 0.6f;
        [Tooltip("Як часто міняються пориви")]
        public float gustSpeed = 0.25f;

        private float[] baseRates;   // початкова Rate over Time кожного шару
        private float intensity = 1f; // 0 = снігу немає, 1 = повний снігопад
        private Coroutine fadeRoutine;

        public bool IsSnowing { get; private set; } = true;

        void Awake()
        {
            baseRates = new float[snowLayers.Length];
            for (int i = 0; i < snowLayers.Length; i++)
            {
                baseRates[i] = snowLayers[i].emission.rateOverTimeMultiplier;
            }
        }

        void Update()
        {
            if (wind == null) return;

            // Пориви: плавний шум поверх базової сили. Коли снігу немає — вітер теж стихає.
            float gust = Mathf.PerlinNoise(Time.time * gustSpeed, 0.37f);
            wind.windMain = (windStrength + gust * gustStrength) * intensity;
        }

        /// <summary>Підключається до кнопки «Сніг+ увімкнути».</summary>
        public void StartSnow()
        {
            IsSnowing = true;
            foreach (var ps in snowLayers)
            {
                if (!ps.isEmitting) ps.Play();
            }
            FadeTo(1f);
        }

        /// <summary>Підключається до кнопки «Сніг+ вимкнути».</summary>
        public void StopSnow()
        {
            IsSnowing = false;
            FadeTo(0f);
        }

        public void ToggleSnow()
        {
            if (IsSnowing) StopSnow();
            else StartSnow();
        }

        private void FadeTo(float target)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeRoutine(target));
        }

        private IEnumerator FadeRoutine(float target)
        {
            float start = intensity;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                intensity = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, time / fadeDuration));
                ApplyIntensity();
                yield return null;
            }

            intensity = target;
            ApplyIntensity();

            if (Mathf.Approximately(target, 0f))
            {
                // Нові сніжинки вже не з'являються — зупиняємо систему, але ті, що летять, доживають.
                foreach (var ps in snowLayers)
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }

            fadeRoutine = null;
        }

        private void ApplyIntensity()
        {
            for (int i = 0; i < snowLayers.Length; i++)
            {
                var emission = snowLayers[i].emission;
                emission.rateOverTimeMultiplier = baseRates[i] * intensity;
            }
        }
    }
}

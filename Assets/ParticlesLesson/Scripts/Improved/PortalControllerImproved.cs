using System.Collections;
using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>
    /// Покращений портал. Що додано порівняно з уроком:
    ///  • активація — це ПОДІЯ: одноразовий спалах (Burst) + ударна хвиля + «поштовх» масштабу кільця;
    ///  • світіння кільця (HDR Emission + Bloom) плавно розгоряється і плавно гасне, а в активному стані «дихає»;
    ///  • точкове світло порталу підсвічує все навколо;
    ///  • окремий звук активації / деактивації + зациклений гул, гучність якого плавно наростає і спадає;
    ///  • керування і з клавіатури, і з кнопки UI (метод Toggle()).
    /// </summary>
    public class PortalControllerImproved : MonoBehaviour
    {
        [Header("Частинки")]
        [Tooltip("Вихор частинок, що закручуються всередину кільця (Looping)")]
        public ParticleSystem vortex;
        [Tooltip("Туманність у центрі порталу (Looping)")]
        public ParticleSystem coreGlow;
        [Tooltip("Одноразовий спалах при активації (Burst, не Looping) — дочірні системи граються разом з ним")]
        public ParticleSystem activationBurst;

        [Header("Кільце і світло")]
        public Renderer ringRenderer;
        [ColorUsage(false, true)] public Color emissionColor = new Color(0.55f, 0.25f, 1f);
        [Tooltip("Яскравість Emission в активному стані (>1, щоб спрацював Bloom)")]
        public float activeEmission = 4f;
        [Tooltip("Яскравість Emission у вимкненому стані — кільце ледь жевріє")]
        public float idleEmission = 0.15f;
        public Light portalLight;
        public float activeLightIntensity = 3f;

        [Header("Звук")]
        public AudioSource sfxSource;
        public AudioSource humSource;
        public AudioClip activateSound;
        public AudioClip deactivateSound;
        [Range(0f, 1f)] public float humVolume = 0.35f;

        [Header("Анімація")]
        public float transitionTime = 0.6f;
        [Tooltip("Наскільки кільце «підстрибує» при активації")]
        public float punchScale = 0.15f;
        public float pulseSpeed = 3f;
        [Range(0f, 0.5f)] public float pulseAmount = 0.15f;

        [Header("Керування")]
        public KeyCode toggleKey = KeyCode.O;

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private Material ringMaterial;
        private Vector3 ringBaseScale;
        private float level;          // 0 = вимкнено, 1 = повністю активно
        private bool isActive;
        private Coroutine transition;

        public bool IsActive => isActive;

        void Awake()
        {
            if (ringRenderer != null)
            {
                // .material створює копію матеріалу саме для цього об'єкта —
                // так ми не змінюємо asset на диску і не чіпаємо інші портали.
                ringMaterial = ringRenderer.material;
                ringBaseScale = ringRenderer.transform.localScale;
            }
            ApplyLevel(0f, 0f);
        }

        void OnDestroy()
        {
            if (ringMaterial != null) Destroy(ringMaterial);
        }

        void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            // У проєктах лише з новим Input System цей виклик кинув би виняток,
            // тому він під директивою. Кнопка UI (Toggle) працює завжди.
            if (Input.GetKeyDown(toggleKey))
            {
                Toggle();
            }
#endif
            if (isActive && transition == null)
            {
                ApplyLevel(1f, 0f);
            }
        }

        public void Toggle()
        {
            if (isActive) Deactivate();
            else Activate();
        }

        public void Activate()
        {
            if (isActive) return;
            isActive = true;

            if (activationBurst != null) activationBurst.Play(true);
            if (vortex != null) vortex.Play(true);
            if (coreGlow != null) coreGlow.Play(true);

            if (sfxSource != null && activateSound != null) sfxSource.PlayOneShot(activateSound);
            if (humSource != null && !humSource.isPlaying) humSource.Play();

            StartTransition(1f);
        }

        public void Deactivate()
        {
            if (!isActive) return;
            isActive = false;

            if (vortex != null) vortex.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            if (coreGlow != null) coreGlow.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            if (sfxSource != null && deactivateSound != null) sfxSource.PlayOneShot(deactivateSound);

            StartTransition(0f);
        }

        private void StartTransition(float target)
        {
            if (transition != null) StopCoroutine(transition);
            transition = StartCoroutine(TransitionRoutine(target));
        }

        private IEnumerator TransitionRoutine(float target)
        {
            float start = level;
            float time = 0f;

            while (time < transitionTime)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / transitionTime);
                level = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));

                // «Поштовх» тільки при вмиканні: sin(0..π) дає горб 0 → 1 → 0
                float punch = target > start ? Mathf.Sin(t * Mathf.PI) * punchScale : 0f;
                ApplyLevel(level, punch);
                yield return null;
            }

            level = target;
            ApplyLevel(level, 0f);

            if (Mathf.Approximately(target, 0f) && humSource != null)
            {
                humSource.Stop();
            }

            transition = null;
        }

        private void ApplyLevel(float value, float punch)
        {
            // В активному стані світіння «дихає»
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount * value;

            if (ringMaterial != null)
            {
                float intensity = Mathf.Lerp(idleEmission, activeEmission, value) * pulse;
                ringMaterial.SetColor(EmissionColorId, emissionColor * intensity);
                ringRenderer.transform.localScale = ringBaseScale * (1f + punch);
            }

            if (portalLight != null)
            {
                portalLight.intensity = activeLightIntensity * value * pulse;
                portalLight.enabled = value > 0.001f;
            }

            if (humSource != null)
            {
                humSource.volume = humVolume * value;
            }
        }
    }
}

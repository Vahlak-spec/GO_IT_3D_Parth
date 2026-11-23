using System.Collections.Generic;
using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>
    /// Модуль Triggers → Enter = Callback.
    /// Unity викликає OnParticleTrigger() на об'єкті з Particle System, коли частинки входять у колайдер.
    /// Ми забираємо ці частинки, змінюємо їм колір і повертаємо назад у систему.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class TriggerRecolor : MonoBehaviour
    {
        public Color enterColor = new Color(0.3f, 0.65f, 1f, 1f);

        private ParticleSystem ps;
        private readonly List<ParticleSystem.Particle> entered = new List<ParticleSystem.Particle>();

        void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        void OnParticleTrigger()
        {
            // 1) Отримуємо частинки, які щойно увійшли в тригер
            int count = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered);

            // 2) Змінюємо їх (Particle — struct, тому треба записати назад у список)
            for (int i = 0; i < count; i++)
            {
                ParticleSystem.Particle p = entered[i];
                p.startColor = enterColor;
                entered[i] = p;
            }

            // 3) Повертаємо змінені частинки в систему
            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered);
        }
    }
}

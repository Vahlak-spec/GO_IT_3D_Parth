using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>
    /// Практика 3.1 — керування Particle System зі скрипта (код з уроку).
    /// Методи EnterWinterZone / ExitWinterZone підключені до кнопок UI через On Click.
    /// </summary>
    public class EffectController : MonoBehaviour
    {
        // Перетягнемо наш Particle System сюди в Inspector
        public ParticleSystem fireEffect;
        public ParticleSystem snowEffect;

        // Запустити ефект вогню (наприклад, при взаємодії з об'єктом)
        public void StartFire()
        {
            fireEffect.Play();
        }

        // Зупинити вогонь
        public void StopFire()
        {
            // StopEmitting зупиняє нові частинки,
            // але вже існуючі доживають свій lifetime
            fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        // Увімкнути сніг при вході в зимову локацію
        public void EnterWinterZone()
        {
            if (!snowEffect.isPlaying)
            {
                snowEffect.Play();
            }
        }

        // Вимкнути сніг
        public void ExitWinterZone()
        {
            snowEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}

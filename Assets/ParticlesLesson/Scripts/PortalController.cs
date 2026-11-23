using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>
    /// «Портал активації» — код з уроку. Клавіша P вмикає / вимикає портал.
    /// Важливо: у PortalParticles має бути вимкнено Play On Awake,
    /// інакше на старті портал уже «працює», а isActive = false (перше натискання нічого не змінить).
    /// </summary>
    public class PortalController : MonoBehaviour
    {
        public ParticleSystem portalParticles;
        public AudioSource audioSource;
        public AudioClip activateSound;

        private bool isActive = false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!isActive)
                {
                    portalParticles.Play();
                    audioSource.PlayOneShot(activateSound);
                    isActive = true;
                }
                else
                {
                    portalParticles.Stop();
                    isActive = false;
                }
            }
        }
    }
}

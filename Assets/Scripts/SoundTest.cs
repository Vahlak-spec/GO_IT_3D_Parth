using UnityEngine;

public class SoundTest : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;

    public void PlaySound()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Play();
        }
    }

    public void StopSound()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Stop();
        }
    }

    public void PauseSound()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Pause();
        }
    }

    public void PlayOnShot()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
        }
    }
}

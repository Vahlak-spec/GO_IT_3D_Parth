using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Потрібно для доступу до компонента Slider

public class VolumeControl : MonoBehaviour
{

    // Слайдер, який перетягнемо з Hierarchy в Inspector

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    // AudioSource, гучністю якого керуємо
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private ParticleSystem particleSystem;

    private float _minValue = -40;
    private float _maxValue = 0;

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);


        particleSystem.Play();

    }

    // Цей метод підключимо до події On Value Changed слайдера
    // Він буде викликатися КОЖНОГО разу, коли гравець рухає повзунок
    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Lerp(_minValue, _maxValue, musicSlider.value));
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }
    public void ChangeSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Lerp(_minValue, _maxValue, sfxSlider.value));
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }
}
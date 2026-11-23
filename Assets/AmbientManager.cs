using UnityEngine;

using System.Collections;

public class AmbientManager : MonoBehaviour

{

    public AudioSource forestAmbient;
    public AudioSource dungeonAmbient;
    // Час плавного переходу в секундах
    public float crossfadeDuration = 2.0f;

    // Викликай цей метод при вході в підземелля
    public void TransitionToDungeon()
    {
        Debug.Log("Transitioning to Dungeon Ambient");
        StartCoroutine(Crossfade(forestAmbient, dungeonAmbient));
    }

    public void TransitionToForest()
    {
        Debug.Log("Transitioning to Forest Ambient");
        StartCoroutine(Crossfade(dungeonAmbient, forestAmbient));
    }

    // Корутина: одночасно зменшує гучність одного і збільшує іншого
    private IEnumerator Crossfade(AudioSource fadeOut, AudioSource fadeIn)
    {
        float startVolumeFadeOut = fadeOut.volume;
        float startVolumeFadeIn = 0;

        // Починаємо відтворення нового шару з нульової гучності
        fadeIn.volume = 0;
        fadeIn.Play();

        float time = 0;

        while (time < crossfadeDuration)
        {
            time += Time.deltaTime;
            float t = time / crossfadeDuration; // Від 0 до 1

            // Зменшуємо один, збільшуємо інший
            fadeOut.volume = Mathf.Lerp(startVolumeFadeOut, 0, t);
            fadeIn.volume = Mathf.Lerp(startVolumeFadeIn, 1, t);

            yield return null; // Чекаємо наступний кадр
        }

        // Зупиняємо попередній шар після завершення переходу
        fadeOut.Stop();
    }
}
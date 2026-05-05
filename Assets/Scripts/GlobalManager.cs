using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;

    [Header("Audio")]
    public AudioSource musicSource;
    public float fadeDuration = 1.5f;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    public SceneMusic[] sceneMusicList;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip newClip = GetClipForScene(scene.name);

        if (newClip != null && musicSource.clip != newClip)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeAndSwitch(newClip));
        }
    }

    AudioClip GetClipForScene(string sceneName)
    {
        foreach (var sm in sceneMusicList)
        {
            if (sm.sceneName == sceneName)
                return sm.musicClip;
        }
        return null;
    }

    IEnumerator FadeAndSwitch(AudioClip newClip)
    {
        // Fade out
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }
}
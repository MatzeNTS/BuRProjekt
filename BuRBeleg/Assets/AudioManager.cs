using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource gustSource;
    private Coroutine gustRoutine;

    [Header("Background Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip ambientMusic;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.35f;
    [SerializeField] private float musicFadeIn = 1.5f;

    [Header("Wind Gust SFX (random)")]
    [SerializeField] private AudioClip[] windGustClips; // 3 mp3s reinziehen
    [SerializeField, Range(0f, 1f)] private float gustVolume = 0.6f;

    [Tooltip("Fade-in Zeit für den Gust (Sekunden). Bei sehr kurzen Clips klein halten.")]
    [SerializeField] private float gustFadeIn = 0.03f;

    [Tooltip("Fade-out Zeit für den Gust (Sekunden). Bei sehr kurzen Clips klein halten.")]
    [SerializeField] private float gustFadeOut = 0.10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        gustSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f; // 2D
    }

    private void Start()
    {
        PlayAmbientMusic();
    }

    public void PlayAmbientMusic()
    {
        if (ambientMusic == null) return;

        musicSource.clip = ambientMusic;
        musicSource.volume = 0f;
        musicSource.Play();
        StartCoroutine(FadeVolume(musicSource, 0f, musicVolume, musicFadeIn));
    }

    public void StopAmbientMusic(float fadeOutSeconds = 1.0f)
    {
        if (!musicSource.isPlaying) return;
        StartCoroutine(StopWithFade(musicSource, fadeOutSeconds));
    }

    /// <summary>
    /// Spielt einen zufälligen Wind-Gust Clip mit Fade-In/Fade-Out.
    /// intensity01 kann optional die Lautstärke leicht beeinflussen.
    /// </summary>
    public void PlayWindGust(Vector3 worldPos, float intensity01 = 1f)
    {
        if (windGustClips == null || windGustClips.Length == 0) return;

        AudioClip clip = windGustClips[Random.Range(0, windGustClips.Length)];
        if (clip == null) return;

        // Position (für 2D egal, aber falls du später 3D willst)
        gustSource.transform.position = worldPos;

        // Vorherigen Gust sofort abbrechen
        if (gustRoutine != null) StopCoroutine(gustRoutine);
        gustSource.Stop();

        gustSource.clip = clip;
        gustSource.volume = 0f;
        gustSource.Play();

        float targetVol = gustVolume * Mathf.Lerp(0.8f, 1.15f, Mathf.Clamp01(intensity01));
        gustRoutine = StartCoroutine(PlayGustWithFade(gustSource, targetVol));
    }

    private IEnumerator PlayGustWithFade(AudioSource src, float targetVolume)
    {
        if (src == null || src.clip == null) yield break;

        float clipLen = src.clip.length;

        float fi = Mathf.Clamp(gustFadeIn, 0f, clipLen * 0.45f);
        float fo = Mathf.Clamp(gustFadeOut, 0f, clipLen * 0.45f);

        // Fade in
        if (fi > 0f)
            yield return FadeVolume(src, 0f, targetVolume, fi);
        else
            src.volume = targetVolume;

        // Hold
        float hold = Mathf.Max(0f, clipLen - fi - fo);
        if (hold > 0f)
            yield return new WaitForSeconds(hold);

        // Fade out
        if (fo > 0f)
            yield return FadeVolume(src, src.volume, 0f, fo);
        else
            src.volume = 0f;

        src.Stop();
        gustRoutine = null;
    }


    private IEnumerator PlayWithFadeAndDestroy(AudioSource src, float targetVolume)
    {
        if (src == null || src.clip == null) yield break;

        float clipLen = src.clip.length;

        // Safety: Fades dürfen bei <1s nicht länger als Clip sein
        float fi = Mathf.Clamp(gustFadeIn, 0f, clipLen * 0.45f);
        float fo = Mathf.Clamp(gustFadeOut, 0f, clipLen * 0.45f);

        // Fade in
        if (fi > 0f)
            yield return FadeVolume(src, 0f, targetVolume, fi);
        else
            src.volume = targetVolume;

        // Hold (bis kurz vor Ende)
        float hold = Mathf.Max(0f, clipLen - fi - fo);
        if (hold > 0f)
            yield return new WaitForSeconds(hold);

        // Fade out
        if (fo > 0f)
            yield return FadeVolume(src, src.volume, 0f, fo);
        else
            src.volume = 0f;

        if (src != null)
            Destroy(src.gameObject);
    }

    private IEnumerator StopWithFade(AudioSource src, float duration)
    {
        if (src == null) yield break;

        float start = src.volume;
        yield return FadeVolume(src, start, 0f, duration);
        src.Stop();
    }

    private IEnumerator FadeVolume(AudioSource src, float from, float to, float duration)
    {
        if (src == null) yield break;

        if (duration <= 0f)
        {
            src.volume = to;
            yield break;
        }

        float t = 0f;
        src.volume = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            src.volume = Mathf.Lerp(from, to, p);
            yield return null;
        }

        src.volume = to;
    }
}

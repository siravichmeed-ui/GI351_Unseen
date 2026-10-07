using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    // =========================
    // MUSIC
    // =========================

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null)
            return;

        if (clip == null)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
        musicSource.clip = null;
    }

    // =========================
    // SFX
    // =========================

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null)
            return;

        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    // =========================
    // VOLUME
    // =========================

    public void SetMusicVolume(float value)
    {
        musicVolume = value;

        if (musicSource != null)
        {
            musicSource.volume = value;
        }
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;

        if (sfxSource != null)
        {
            sfxSource.volume = value;
        }
    }
}
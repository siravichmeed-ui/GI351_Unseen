using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Header("SFX")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    // เล่นเพลง
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // เล่นเสียง Effect
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;
    
       sfxSource.PlayOneShot(clip);
    }

    // ปรับเสียงเพลง
    public void SetMusicVolume(float value)
    {
        Debug.Log("Slider Value = " + value);

        musicVolume = value;

        if (musicSource != null)
        {
            musicSource.volume = value;
        }
    }

    // ปรับเสียง Effect
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = value;
    }
}
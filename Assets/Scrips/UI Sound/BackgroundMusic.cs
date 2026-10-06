using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip music;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(music);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    
    public Slider musicVolumeSlider;
    //public Slider sfxVolumeSlider;

    public TMP_Dropdown resolutionDropdown;
    void Start()
    {
        LoadSettings();
    }
    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    //public void SetSFXVolume(float value)
    //{
    //   PlayerPrefs.SetFloat("SFXVolume", value);
    //}

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
    void LoadSettings()
    {
        musicVolumeSlider.value =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        //sfxVolumeSlider.value =
        //    PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
}

using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public AudioClip clickSound;
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    

    public void PlayClick() // เสียงคลิกเมื่อกดปุ่ม
    {
        AudioManager.Instance.PlaySFX(clickSound);
    }
    public void Back()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
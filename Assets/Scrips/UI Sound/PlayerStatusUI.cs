using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider expBar;
    [SerializeField] private Slider lightBar;

    [SerializeField] private Image lampIcon;

    [SerializeField] private TMP_Text levelText;


    [Header("Lamp Color")]
    [SerializeField] private Color lampFullColor = Color.white;

    [SerializeField]
    private Color lampEmptyColor =
        new Color(0.15f, 0.15f, 0.15f);


    [Header("Player")]
    [SerializeField] private PlayerController playerController;

    [SerializeField] private PlayerLevel playerLevel;

    [SerializeField] private LampManager lampManager;


    // =====================================================
    // FILL IMAGES
    // =====================================================

    private Image hpFillImage;

    private Image expFillImage;

    private Image lightFillImage;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        FindPlayerComponents();

        SetupBars();

        SetupFillImages();

        UpdateUI();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        UpdateUI();
    }


    // =====================================================
    // FIND PLAYER COMPONENTS
    // =====================================================

    private void FindPlayerComponents()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");


        if (player == null)
        {
            Debug.LogWarning(
                "PlayerStatusUI: หา Player ไม่เจอ"
            );

            return;
        }


        // Player Controller
        if (playerController == null)
        {
            playerController =
                player.GetComponent<PlayerController>();
        }


        // Player Level
        if (playerLevel == null)
        {
            playerLevel =
                player.GetComponent<PlayerLevel>();
        }


        // Lamp Manager
        if (lampManager == null)
        {
            lampManager =
                FindFirstObjectByType<LampManager>();
        }
    }


    // =====================================================
    // SETUP BARS
    // =====================================================

    private void SetupBars()
    {
        if (hpBar != null)
        {
            hpBar.minValue = 0f;
            hpBar.maxValue = 1f;
        }


        if (expBar != null)
        {
            expBar.minValue = 0f;
            expBar.maxValue = 1f;
        }


        if (lightBar != null)
        {
            lightBar.minValue = 0f;
            lightBar.maxValue = 1f;
        }


        if (lampIcon != null)
        {
            lampIcon.color =
                lampFullColor;
        }
    }


    // =====================================================
    // SETUP FILL IMAGES
    // =====================================================

    private void SetupFillImages()
    {
        // HP Fill
        if (hpBar != null &&
            hpBar.fillRect != null)
        {
            hpFillImage =
                hpBar.fillRect.GetComponent<Image>();
        }


        // EXP Fill
        if (expBar != null &&
            expBar.fillRect != null)
        {
            expFillImage =
                expBar.fillRect.GetComponent<Image>();
        }


        // LIGHT Fill
        if (lightBar != null &&
            lightBar.fillRect != null)
        {
            lightFillImage =
                lightBar.fillRect.GetComponent<Image>();
        }
    }


    // =====================================================
    // UPDATE UI
    // =====================================================

    private void UpdateUI()
    {
        UpdateHP();

        UpdateEXP();

        UpdateLight();

        UpdateLevel();
    }


    // =====================================================
    // HP
    // =====================================================

    private void UpdateHP()
    {
        if (hpBar == null)
            return;


        if (playerController == null)
            return;


        float maxHealth =
            playerController.MaxHealth;


        if (maxHealth <= 0f)
        {
            hpBar.value = 0f;

            if (hpFillImage != null)
            {
                hpFillImage.enabled = false;
            }

            return;
        }


        float currentHealth =
            playerController.CurrentHealth;


        float healthPercent =
            currentHealth /
            maxHealth;


        float healthValue =
            Mathf.Clamp01(
                healthPercent
            );


        // HP Slider
        hpBar.value =
            healthValue;


        // HP Fill
        if (hpFillImage != null)
        {
            hpFillImage.enabled =
                healthValue > 0.001f;
        }
    }


    // =====================================================
    // EXP
    // =====================================================

    private void UpdateEXP()
    {
        if (expBar == null)
            return;


        if (playerLevel == null)
            return;


        float expNeeded =
            playerLevel.ExpToNextLevel;


        if (expNeeded <= 0f)
        {
            expBar.value = 0f;

            if (expFillImage != null)
            {
                expFillImage.enabled = false;
            }

            return;
        }


        float currentExp =
            playerLevel.CurrentExp;


        float expPercent =
            currentExp /
            expNeeded;


        float expValue =
            Mathf.Clamp01(
                expPercent
            );


        // EXP Slider
        expBar.value =
            expValue;


        // EXP Fill
        if (expFillImage != null)
        {
            expFillImage.enabled =
                expValue > 0.001f;
        }
    }


    // =====================================================
    // LIGHT
    // =====================================================

    private void UpdateLight()
    {
        if (lampManager == null)
            return;


        float lightPercent =
            lampManager.GetRemainingPercent();


        float lightValue =
            Mathf.Clamp01(
                lightPercent / 100f
            );


        // Light Slider
        if (lightBar != null)
        {
            lightBar.value =
                lightValue;
        }


        // Light Fill
        if (lightFillImage != null)
        {
            lightFillImage.enabled =
                lightValue > 0.001f;
        }


        // Lamp Icon
        if (lampIcon != null)
        {
            lampIcon.color =
                Color.Lerp(
                    lampEmptyColor,
                    lampFullColor,
                    lightValue
                );
        }
    }


    // =====================================================
    // LEVEL
    // =====================================================

    private void UpdateLevel()
    {
        if (levelText == null)
            return;


        if (playerLevel == null)
            return;


        levelText.text =
            "Lv. " +
            playerLevel.Level;
    }
}
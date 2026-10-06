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
    // START
    // =====================================================

    private void Start()
    {
        FindPlayerComponents();

        SetupBars();

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

            return;
        }


        float currentHealth =
            playerController.CurrentHealth;


        float healthPercent =
            currentHealth / maxHealth;


        hpBar.value =
            Mathf.Clamp01(
                healthPercent
            );
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

            return;
        }


        float currentExp =
            playerLevel.CurrentExp;


        float expPercent =
            currentExp / expNeeded;


        expBar.value =
            Mathf.Clamp01(
                expPercent
            );
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
            "Lv. " + playerLevel.Level;
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


        // Light Bar
        if (lightBar != null)
        {
            lightBar.value =
                lightValue;
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
}
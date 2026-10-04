using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUpgradeUI : MonoBehaviour
{
    [Header("Upgrade Panel")]
    [SerializeField] private GameObject upgradePanel;


    [Header("Skill Point")]
    [SerializeField] private TMP_Text skillPointText;


    [Header("HP UI")]
    [SerializeField] private TMP_Text hpLevelText;
    [SerializeField] private TMP_Text hpValueText;


    [Header("Scan Range UI")]
    [SerializeField] private TMP_Text scanRangeLevelText;
    [SerializeField] private TMP_Text scanRangeValueText;


    [Header("Scan Cooldown UI")]
    [SerializeField] private TMP_Text scanCooldownLevelText;
    [SerializeField] private TMP_Text scanCooldownValueText;


    [Header("Player")]
    [SerializeField] private PlayerController playerController;

    [SerializeField] private PlayerLevel playerLevel;

    [SerializeField] private PlayerSkillUpgrade playerSkillUpgrade;

    [SerializeField] private PlayerScanSkill playerScanSkill;


    private bool isOpen = false;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }


        FindPlayerComponents();


        SetControlState(false);


        UpdateAllUI();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleUpgradePanel();
        }
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
                "SkillUpgradeUI: หา Player ไม่เจอ"
            );

            return;
        }


        // PlayerController
        if (playerController == null)
        {
            playerController =
                player.GetComponent<PlayerController>();
        }


        // PlayerLevel
        if (playerLevel == null)
        {
            playerLevel =
                player.GetComponent<PlayerLevel>();
        }


        // PlayerSkillUpgrade
        if (playerSkillUpgrade == null)
        {
            playerSkillUpgrade =
                player.GetComponent<PlayerSkillUpgrade>();
        }


        // PlayerScanSkill
        if (playerScanSkill == null)
        {
            playerScanSkill =
                player.GetComponent<PlayerScanSkill>();
        }
    }


    // =====================================================
    // TOGGLE PANEL
    // =====================================================

    private void ToggleUpgradePanel()
    {
        if (upgradePanel == null)
            return;


        isOpen = !isOpen;


        upgradePanel.SetActive(isOpen);


        SetControlState(isOpen);


        if (isOpen)
        {
            UpdateAllUI();
        }
    }


    // =====================================================
    // UPDATE ALL UI
    // =====================================================

    public void UpdateAllUI()
    {
        UpdateSkillPointUI();

        UpdateHealthUI();

        UpdateScanRangeUI();

        UpdateScanCooldownUI();
    }


    // =====================================================
    // SKILL POINT UI
    // =====================================================

    private void UpdateSkillPointUI()
    {
        if (skillPointText == null)
            return;


        if (playerLevel == null)
            return;


        skillPointText.text =
            "Skill Point: " +
            playerLevel.SkillPoints;
    }


    // =====================================================
    // HEALTH UI
    // =====================================================

    private void UpdateHealthUI()
    {
        if (playerSkillUpgrade == null)
            return;


        if (hpLevelText != null)
        {
            hpLevelText.text =
                "Lv. " +
                playerSkillUpgrade.HealthLevel;
        }


        if (hpValueText != null)
        {
            hpValueText.text =
                "+ " +
                playerSkillUpgrade.HealthIncrease +
                " Max HP";
        }
    }


    // =====================================================
    // SCAN RANGE UI
    // =====================================================

    private void UpdateScanRangeUI()
    {
        if (playerSkillUpgrade == null)
            return;


        if (scanRangeLevelText != null)
        {
            scanRangeLevelText.text =
                "Lv. " +
                playerSkillUpgrade.ScanRangeLevel;
        }


        if (scanRangeValueText != null)
        {
            if (playerScanSkill != null)
            {
                scanRangeValueText.text =
                    "Range: " +
                    playerScanSkill.GetScanRange()
                    .ToString("F0");
            }
            else
            {
                scanRangeValueText.text =
                    "Range: -";
            }
        }
    }


    // =====================================================
    // SCAN COOLDOWN UI
    // =====================================================

    private void UpdateScanCooldownUI()
    {
        if (playerSkillUpgrade == null)
            return;


        if (scanCooldownLevelText != null)
        {
            scanCooldownLevelText.text =
                "Lv. " +
                playerSkillUpgrade.ScanCooldownLevel;
        }


        if (scanCooldownValueText != null)
        {
            if (playerScanSkill != null)
            {
                scanCooldownValueText.text =
                    "Cooldown: " +
                    playerScanSkill.GetCooldown()
                    .ToString("F0") +
                    "s";
            }
            else
            {
                scanCooldownValueText.text =
                    "Cooldown: -";
            }
        }
    }


    // =====================================================
    // BUTTON - HEALTH
    // =====================================================

    public void OnHealthUpgradeButton()
    {
        if (playerSkillUpgrade == null)
            return;


        playerSkillUpgrade.UpgradeHealth();


        // อัปเดตทันที
        UpdateAllUI();
    }


    // =====================================================
    // BUTTON - SCAN RANGE
    // =====================================================

    public void OnScanRangeUpgradeButton()
    {
        if (playerSkillUpgrade == null)
            return;


        playerSkillUpgrade.UpgradeScanRange();


        // อัปเดตทันที
        UpdateAllUI();
    }


    // =====================================================
    // BUTTON - SCAN COOLDOWN
    // =====================================================

    public void OnScanCooldownUpgradeButton()
    {
        if (playerSkillUpgrade == null)
            return;


        playerSkillUpgrade.UpgradeScanCooldown();


        // อัปเดตทันที
        UpdateAllUI();
    }


    // =====================================================
    // SET CONTROL STATE
    // =====================================================

    private void SetControlState(bool uiOpen)
    {
        // Player
        if (playerController != null)
        {
            playerController.SetUIOpen(uiOpen);
        }


        // Gun
        Gun gun =
            playerController != null
                ? playerController.GetComponentInChildren<Gun>()
                : null;


        if (gun != null)
        {
            gun.SetCanShoot(!uiOpen);
        }


        // Gun Holder
        GunHolder gunHolder =
            playerController != null
                ? playerController.GetComponentInChildren<GunHolder>()
                : null;


        if (gunHolder != null)
        {
            gunHolder.SetCanControlGun(!uiOpen);
        }
    }


    // =====================================================
    // DESTROY
    // =====================================================

    private void OnDestroy()
    {
        SetControlState(false);
    }
}
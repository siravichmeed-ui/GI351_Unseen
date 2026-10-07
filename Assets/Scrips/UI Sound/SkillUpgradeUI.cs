using TMPro;
using UnityEngine;

public class SkillUpgradeUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject upgradePanel;

    [Header("Skill Point")]
    [SerializeField] private TMP_Text skillPointText;

    [Header("HP")]
    [SerializeField] private TMP_Text hpLevelText;
    [SerializeField] private TMP_Text hpValueText;

    [Header("Scan Range")]
    [SerializeField] private TMP_Text scanRangeLevelText;
    [SerializeField] private TMP_Text scanRangeValueText;

    [Header("Scan Cooldown")]
    [SerializeField] private TMP_Text scanCooldownLevelText;
    [SerializeField] private TMP_Text scanCooldownValueText;

    private PlayerController playerController;
    private PlayerLevel playerLevel;
    private PlayerSkillUpgrade playerSkillUpgrade;
    private PlayerScanSkill playerScanSkill;

    private Gun gun;
    private GunHolder gunHolder;

    private bool isOpen;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        playerLevel = FindFirstObjectByType<PlayerLevel>();
        playerSkillUpgrade = FindFirstObjectByType<PlayerSkillUpgrade>();
        playerScanSkill = FindFirstObjectByType<PlayerScanSkill>();

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        isOpen = false;

        SetControlState(false);

        UpdateAllUI();
    }

    private void Update()
    {
        // กด E เปิด/ปิด Skill Upgrade
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleUpgradePanel();
        }

        // อัปเดต HP แบบ Real-time
        if (isOpen)
        {
            UpdateHealthUI();
            UpdateSkillPointUI();
        }
    }

    // =========================================================
    // OPEN / CLOSE
    // =========================================================

    private void ToggleUpgradePanel()
    {
        isOpen = !isOpen;

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(isOpen);
        }

        SetControlState(isOpen);

        if (isOpen)
        {
            UpdateAllUI();
        }
    }

    // =========================================================
    // UPDATE ALL UI
    // =========================================================

    private void UpdateAllUI()
    {
        UpdateSkillPointUI();
        UpdateHealthUI();
        UpdateScanRangeUI();
        UpdateScanCooldownUI();
    }

    // =========================================================
    // SKILL POINT
    // =========================================================

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

    // =========================================================
    // HP
    // =========================================================

    private void UpdateHealthUI()
    {
        if (playerSkillUpgrade == null)
            return;

        if (playerController == null)
            return;

        // Level
        if (hpLevelText != null)
        {
            hpLevelText.text =
                "Lv. " +
                playerSkillUpgrade.HealthLevel;
        }

        // Current HP / Max HP
        if (hpValueText != null)
        {
            hpValueText.text =
                "HP: " +
                Mathf.CeilToInt(playerController.CurrentHealth) +
                " / " +
                Mathf.CeilToInt(playerController.MaxHealth) +
                "\n" +
                "+" +
                playerSkillUpgrade.HealthIncrease +
                " Max HP";
        }
    }

    // =========================================================
    // SCAN RANGE
    // =========================================================

    private void UpdateScanRangeUI()
    {
        if (playerSkillUpgrade == null)
            return;

        if (playerScanSkill == null)
            return;

        // Level
        if (scanRangeLevelText != null)
        {
            scanRangeLevelText.text =
                "Lv. " +
                playerSkillUpgrade.ScanRangeLevel;
        }

        // Current Range + Upgrade amount
        if (scanRangeValueText != null)
        {
            scanRangeValueText.text =
                "Range: " +
                playerScanSkill.GetScanRange().ToString("F0") +
                "\n+" +
                playerSkillUpgrade.ScanRangeIncrease +
                " Range";
        }
    }

    // =========================================================
    // SCAN COOLDOWN
    // =========================================================

    private void UpdateScanCooldownUI()
    {
        if (playerSkillUpgrade == null)
            return;

        if (playerScanSkill == null)
            return;

        // Level
        if (scanCooldownLevelText != null)
        {
            scanCooldownLevelText.text =
                "Lv. " +
                playerSkillUpgrade.ScanCooldownLevel;
        }

        // Current Cooldown + Upgrade amount
        if (scanCooldownValueText != null)
        {
            scanCooldownValueText.text =
                "Cooldown: " +
                playerScanSkill.GetCooldown().ToString("F0") +
                "s" +
                "\n-" +
                playerSkillUpgrade.ScanCooldownDecrease +
                "s Cooldown";
        }
    }

    // =========================================================
    // UPGRADE HP
    // =========================================================

    public void UpgradeHealth()
    {
        if (playerSkillUpgrade == null)
            return;

        playerSkillUpgrade.UpgradeHealth();

        UpdateAllUI();
    }

    // =========================================================
    // UPGRADE SCAN RANGE
    // =========================================================

    public void UpgradeScanRange()
    {
        if (playerSkillUpgrade == null)
            return;

        playerSkillUpgrade.UpgradeScanRange();

        UpdateAllUI();
    }

    // =========================================================
    // UPGRADE SCAN COOLDOWN
    // =========================================================

    public void UpgradeScanCooldown()
    {
        if (playerSkillUpgrade == null)
            return;

        playerSkillUpgrade.UpgradeScanCooldown();

        UpdateAllUI();
    }

    // =========================================================
    // PLAYER CONTROL
    // =========================================================

    private void SetControlState(bool uiOpen)
    {
        if (playerController != null)
        {
            playerController.SetUIOpen(uiOpen);
        }

        // หา Gun
        if (gun == null)
        {
            gun = FindFirstObjectByType<Gun>();
        }

        // หา GunHolder
        if (gunHolder == null)
        {
            gunHolder = FindFirstObjectByType<GunHolder>();
        }

        // ปิด/เปิดการยิง
        if (gun != null)
        {
            gun.SetCanShoot(!uiOpen);
        }

        // ปิด/เปิดการควบคุมปืน
        if (gunHolder != null)
        {
            gunHolder.SetCanControlGun(!uiOpen);
        }
    }

    // =========================================================
    // DESTROY
    // =========================================================

    private void OnDestroy()
    {
        SetControlState(false);
    }
}
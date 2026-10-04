using UnityEngine;

public class PlayerSkillUpgrade : MonoBehaviour
{
    [Header("Player Level")]
    [SerializeField] private PlayerLevel playerLevel;


    [Header("Player")]
    [SerializeField] private PlayerController playerController;


    [Header("Scan Skill")]
    [SerializeField] private PlayerScanSkill playerScanSkill;


    [Header("Upgrade Values")]
    [SerializeField] private float healthIncrease = 10f;

    [SerializeField] private float scanRangeIncrease = 1f;

    [SerializeField] private float scanCooldownDecrease = 1f;


    [Header("Current Upgrade Level")]
    [SerializeField] private int healthLevel = 0;

    [SerializeField] private int scanRangeLevel = 0;

    [SerializeField] private int scanCooldownLevel = 0;


    // =====================================================
    // GETTERS
    // =====================================================

    public int HealthLevel
    {
        get
        {
            return healthLevel;
        }
    }


    public int ScanRangeLevel
    {
        get
        {
            return scanRangeLevel;
        }
    }


    public int ScanCooldownLevel
    {
        get
        {
            return scanCooldownLevel;
        }
    }


    public float HealthIncrease
    {
        get
        {
            return healthIncrease;
        }
    }


    public float ScanRangeIncrease
    {
        get
        {
            return scanRangeIncrease;
        }
    }


    public float ScanCooldownDecrease
    {
        get
        {
            return scanCooldownDecrease;
        }
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (playerLevel == null)
        {
            playerLevel =
                GetComponent<PlayerLevel>();
        }


        if (playerController == null)
        {
            playerController =
                GetComponent<PlayerController>();
        }


        if (playerScanSkill == null)
        {
            playerScanSkill =
                GetComponent<PlayerScanSkill>();
        }


        if (playerLevel == null)
        {
            Debug.LogError(
                "PlayerSkillUpgrade: ไม่พบ PlayerLevel"
            );
        }


        if (playerController == null)
        {
            Debug.LogError(
                "PlayerSkillUpgrade: ไม่พบ PlayerController"
            );
        }


        if (playerScanSkill == null)
        {
            Debug.LogError(
                "PlayerSkillUpgrade: ไม่พบ PlayerScanSkill"
            );
        }
    }


    // =====================================================
    // UPGRADE HEALTH
    // =====================================================

    public void UpgradeHealth()
    {
        if (playerLevel == null)
        {
            return;
        }


        // ไม่มี Skill Point
        if (!playerLevel.UseSkillPoint())
        {
            Debug.Log(
                "Skill Point ไม่พอ"
            );

            return;
        }


        healthLevel++;


        if (playerController != null)
        {
            playerController.IncreaseMaxHealth(
                healthIncrease
            );
        }


        Debug.Log(
            "Upgrade HP!"
            + " | Level: "
            + healthLevel
            + " | Max HP +"
            + healthIncrease
        );
    }


    // =====================================================
    // UPGRADE SCAN RANGE
    // =====================================================

    public void UpgradeScanRange()
    {
        if (playerLevel == null)
        {
            return;
        }


        if (playerScanSkill == null)
        {
            Debug.LogWarning(
                "ไม่มี PlayerScanSkill"
            );

            return;
        }


        // ไม่มี Skill Point
        if (!playerLevel.UseSkillPoint())
        {
            Debug.Log(
                "Skill Point ไม่พอ"
            );

            return;
        }


        scanRangeLevel++;


        // เพิ่มระยะ Scan จริง
        playerScanSkill.IncreaseScanRange(
            scanRangeIncrease
        );


        Debug.Log(
            "Upgrade Scan Range!"
            + " | Level: "
            + scanRangeLevel
            + " | Range +"
            + scanRangeIncrease
        );
    }


    // =====================================================
    // UPGRADE SCAN COOLDOWN
    // =====================================================

    public void UpgradeScanCooldown()
    {
        if (playerLevel == null)
        {
            return;
        }


        if (playerScanSkill == null)
        {
            Debug.LogWarning(
                "ไม่มี PlayerScanSkill"
            );

            return;
        }


        // ไม่มี Skill Point
        if (!playerLevel.UseSkillPoint())
        {
            Debug.Log(
                "Skill Point ไม่พอ"
            );

            return;
        }


        scanCooldownLevel++;


        // ลด Cooldown จริง
        playerScanSkill.DecreaseCooldown(
            scanCooldownDecrease
        );


        Debug.Log(
            "Upgrade Scan Cooldown!"
            + " | Level: "
            + scanCooldownLevel
            + " | Cooldown -"
            + scanCooldownDecrease
            + " sec"
        );
    }
}
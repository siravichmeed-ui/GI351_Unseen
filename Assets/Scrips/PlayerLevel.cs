using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int level = 0;


    [Header("EXP")]
    [SerializeField] private float currentExp = 0f;
    [SerializeField] private float expToNextLevel = 100f;


    [Header("Level Up")]
    [SerializeField] private float expIncreasePerLevel = 50f;


    [Header("Skill Point")]
    [SerializeField] private int skillPoints = 0;


    // =====================================================
    // GETTERS
    // =====================================================

    public int Level => level;

    public float CurrentExp => currentExp;

    public float ExpToNextLevel => expToNextLevel;

    public int SkillPoints => skillPoints;


    // =====================================================
    // ADD EXP
    // =====================================================

    public void AddExp(float amount)
    {
        if (amount <= 0f)
            return;


        currentExp += amount;


        Debug.Log(
            "ได้รับ EXP +" +
            amount +
            " | EXP: " +
            currentExp +
            " / " +
            expToNextLevel
        );


        CheckLevelUp();
    }


    // =====================================================
    // CHECK LEVEL UP
    // =====================================================

    private void CheckLevelUp()
    {
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;

            LevelUp();
        }
    }


    // =====================================================
    // LEVEL UP
    // =====================================================

    private void LevelUp()
    {
        // เพิ่ม Level
        level++;


        // ได้ Skill Point 1 แต้ม
        skillPoints++;


        // เพิ่ม EXP ที่ต้องใช้ใน Level ต่อไป
        expToNextLevel +=
            expIncreasePerLevel;


        Debug.Log(
            "LEVEL UP! Player Level = " +
            level +
            " | Skill Point = " +
            skillPoints
        );
    }


    // =====================================================
    // USE SKILL POINT
    // =====================================================

    public bool UseSkillPoint()
    {
        if (skillPoints <= 0)
        {
            return false;
        }


        skillPoints--;


        Debug.Log(
            "ใช้ Skill Point 1 แต้ม" +
            " | เหลือ = " +
            skillPoints
        );


        return true;
    }
}
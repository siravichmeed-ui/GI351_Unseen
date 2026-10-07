using TMPro;
using UnityEngine;

public class ScanCooldownUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text cooldownText;

    [Header("Scan")]
    [SerializeField] private PlayerScanSkill playerScanSkill;

    private void Start()
    {
        if (playerScanSkill == null)
        {
            playerScanSkill =
                FindFirstObjectByType<PlayerScanSkill>();
        }

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (cooldownText == null)
        {
            return;
        }

        if (playerScanSkill == null)
        {
            cooldownText.text =
                "Skill Cooldown: Ready";
            return;
        }

        float remaining =
            playerScanSkill.GetCooldownRemaining();

        if (remaining <= 0f)
        {
            cooldownText.text =
                "Skill Cooldown: Ready";
            return;
        }

        cooldownText.text =
            "Skill Cooldown: " +
            remaining.ToString("F1") +
            "s";
    }
}
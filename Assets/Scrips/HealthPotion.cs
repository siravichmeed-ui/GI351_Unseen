using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [Header("Heal Amount")]
    [SerializeField] private float healAmount = 20f;


    // =====================================================
    // PLAYER PICKUP
    // =====================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจว่าเป็น Player หรือไม่
        if (!other.CompareTag("Player"))
        {
            return;
        }


        // =================================================
        // หา PlayerController
        // =================================================

        PlayerController playerController =
            other.GetComponent<PlayerController>();


        // ถ้า Collider อยู่ในลูกของ Player
        // ให้ลองหาใน Parent ด้วย
        if (playerController == null)
        {
            playerController =
                other.GetComponentInParent<PlayerController>();
        }


        // =================================================
        // ถ้าหา PlayerController ไม่เจอ
        // =================================================

        if (playerController == null)
        {
            Debug.LogWarning(
                "HealthPotion: หา PlayerController ไม่เจอ"
            );

            return;
        }


        // =================================================
        // HEAL PLAYER
        // =================================================

        playerController.Heal(
            healAmount
        );


        // =================================================
        // ลบ Potion
        // =================================================

        Destroy(
            gameObject
        );
    }
}
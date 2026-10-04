using UnityEngine;

public class LightItem : MonoBehaviour
{
    [Header("Light Recovery")]
    [SerializeField] private float lightTimeAmount = 10f;


    // =====================================================
    // PLAYER PICKUP
    // =====================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        // =================================================
        // ตรวจว่าเป็น Player
        // =================================================

        if (!other.CompareTag("Player"))
        {
            return;
        }


        // =================================================
        // หา LampManager
        // =================================================

        LampManager lampManager =
            FindFirstObjectByType<LampManager>();


        if (lampManager == null)
        {
            Debug.LogError(
                "LightItem: หา LampManager ไม่เจอ"
            );

            return;
        }


        // =================================================
        // เพิ่มเวลาไฟ
        // =================================================

        lampManager.AddLightTime(
            lightTimeAmount
        );


        // =================================================
        // ลบ Item
        // =================================================

        Destroy(
            gameObject
        );
    }
}
using UnityEngine;

public class GunHolder : MonoBehaviour
{
    [Header("Gun Position")]
    [SerializeField] private float distanceFromPlayer = 1f;

    [Header("Gun")]
    [SerializeField] private Transform gun;

    private Camera mainCamera;

    // =====================================================
    // UI LOCK
    // =====================================================

    private bool canControlGun = true;

    // =====================================================
    // PLAYER
    // =====================================================

    private PlayerController playerController;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        mainCamera =
            Camera.main;

        playerController =
            FindFirstObjectByType<PlayerController>();
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // ถ้า Player ตายแล้ว
        // ห้ามควบคุมปืน
        if (playerController != null &&
            playerController.IsDead)
        {
            return;
        }

        // ถ้าถูกล็อกโดย UI
        if (!canControlGun)
            return;

        RotateAroundPlayer();

        FlipGun();
    }

    // =====================================================
    // SET CONTROL
    // =====================================================

    public void SetCanControlGun(
        bool value)
    {
        canControlGun = value;
    }

    // =====================================================
    // ROTATE AROUND PLAYER
    // =====================================================

    private void RotateAroundPlayer()
    {
        if (mainCamera == null)
            return;

        Vector3 mousePosition =
            Input.mousePosition;

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                mousePosition
            );

        mouseWorldPosition.z =
            transform.parent.position.z;

        Vector2 direction =
            mouseWorldPosition -
            transform.parent.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        // ตำแหน่ง GunHolder
        transform.position =
            transform.parent.position +
            (Vector3)(
                direction *
                distanceFromPlayer
            );

        // มุมของปืน
        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }

    // =====================================================
    // FLIP GUN
    // =====================================================

    private void FlipGun()
    {
        if (gun == null)
            return;

        // ถ้า Gun อยู่ทางซ้ายของ Player
        if (
            transform.position.x <
            transform.parent.position.x
        )
        {
            gun.localScale =
                new Vector3(
                    1f,
                    -1f,
                    1f
                );
        }

        // ถ้า Gun อยู่ทางขวาของ Player
        else
        {
            gun.localScale =
                new Vector3(
                    1f,
                    1f,
                    1f
                );
        }
    }
}
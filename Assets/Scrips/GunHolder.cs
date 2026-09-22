using UnityEngine;

public class GunHolder : MonoBehaviour
{
    [Header("Gun Position")]
    [SerializeField] private float distanceFromPlayer = 1f;

    [Header("Gun")]
    [SerializeField] private Transform gun;

    private Camera mainCamera;


    private void Start()
    {
        mainCamera = Camera.main;
    }


    private void Update()
    {
        RotateAroundPlayer();
        FlipGun();
    }


    // =====================================================
    // ROTATE AROUND PLAYER
    // =====================================================

    private void RotateAroundPlayer()
    {
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
            ) * Mathf.Rad2Deg;


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
        if (transform.position.x <
            transform.parent.position.x)
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
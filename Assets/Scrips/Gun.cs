using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Fire Settings")]
    [SerializeField] private float fireRate = 0.2f;

    private float nextFireTime;

    // =====================================================
    // UI LOCK
    // =====================================================

    private bool canShoot = true;

    // =====================================================
    // PLAYER
    // =====================================================

    private PlayerController playerController;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        playerController =
            FindFirstObjectByType<PlayerController>();
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // ถ้า Player ตายแล้ว
        // ห้ามยิง
        if (playerController != null &&
            playerController.IsDead)
        {
            return;
        }

        // ถ้าถูกล็อกโดย UI
        // ไม่สามารถยิงได้
        if (!canShoot)
            return;

        // กดยิงค้าง
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    // =====================================================
    // SET SHOOT
    // =====================================================

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    // =====================================================
    // SHOOT
    // =====================================================

    private void Shoot()
    {
        // ป้องกันยิงเร็วเกิน Fire Rate
        if (Time.time < nextFireTime)
            return;

        // ถ้า Player ตาย
        // ป้องกันอีกชั้น
        if (playerController != null &&
            playerController.IsDead)
        {
            return;
        }

        // ไม่มี Bullet Prefab
        if (bulletPrefab == null)
            return;

        // ไม่มี Fire Point
        if (firePoint == null)
            return;

        // ตั้งเวลาในการยิงครั้งถัดไป
        nextFireTime =
            Time.time + fireRate;

        // สร้างกระสุน
        GameObject bullet =
            Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation
            );

        // เล่นเสียงยิง
        if (audioSource != null &&
            shootSound != null)
        {
            audioSource.PlayOneShot(
                shootSound
            );
        }
    }
}
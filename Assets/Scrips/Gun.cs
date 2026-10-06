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
    // UPDATE
    // =====================================================

    private void Update()
    {
        // ถ้าถูกล็อก ไม่สามารถยิงได้
        if (!canShoot)
            return;


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
        if (Time.time < nextFireTime)
            return;


        if (bulletPrefab == null)
            return;


        if (firePoint == null)
            return;


        nextFireTime =
            Time.time + fireRate;


        GameObject bullet =
            Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation
            );

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
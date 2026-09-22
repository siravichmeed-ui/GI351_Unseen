using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Fire Settings")]
    [SerializeField] private float fireRate = 0.2f;

    private float nextFireTime;


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }


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
    }
}
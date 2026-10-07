using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 10f;

    [Header("Hit Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    [Header("Hit Animation")]
    [SerializeField] private float hitAnimationTime = 0.1f;

    [Header("Wall")]
    [SerializeField] private LayerMask wallLayer;

    private Animator animator;

    private bool hasHit = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (hasHit)
            return;

        transform.position +=
            transform.right *
            speed *
            Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        // =================================================
        // ENEMY
        // =================================================

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            PlayHit();

            return;
        }

        // =================================================
        // WALL
        // =================================================

        if (((1 << other.gameObject.layer) & wallLayer) != 0)
        {
            PlayHit();

            return;
        }
    }

    // =====================================================
    // HIT
    // =====================================================

    private void PlayHit()
    {
        hasHit = true;

        if (audioSource != null &&
            hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        StartCoroutine(
            DestroyAfterHit()
        );
    }

    // =====================================================
    // DESTROY
    // =====================================================

    private IEnumerator DestroyAfterHit()
    {
        yield return new WaitForSeconds(
            hitAnimationTime
        );

        Destroy(gameObject);
    }
}
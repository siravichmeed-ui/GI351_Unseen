using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 10f;

    [Header("Hit Animation")]
    [SerializeField] private float hitAnimationTime = 0.1f;

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
        // ถ้าชนแล้ว หยุดการเคลื่อนที่
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

        Enemy enemy =
            other.GetComponent<Enemy>();

        if (enemy == null)
            return;


        // ==========================================
        // DAMAGE ENEMY
        // ==========================================

        enemy.TakeDamage(damage);


        // ==========================================
        // PLAY HIT ANIMATION
        // ==========================================

        hasHit = true;


        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }


        // ==========================================
        // DESTROY AFTER ANIMATION
        // ==========================================

        StartCoroutine(
            DestroyAfterHit()
        );
    }


    private IEnumerator DestroyAfterHit()
    {
        yield return new WaitForSeconds(
            hitAnimationTime
        );

        Destroy(gameObject);
    }
}
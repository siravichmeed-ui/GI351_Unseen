using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Hit Effect")]
    [SerializeField] private float hitFlashDuration = 0.08f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float hitInvincibilityTime = 0.5f;

    private float currentHealth;
    private float invincibilityTimer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    private Coroutine hitFlashCoroutine;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        animator =
            GetComponent<Animator>();

        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();

        currentHealth = maxHealth;
    }


    private void Update()
    {
        movement.x =
            Input.GetAxisRaw("Horizontal");

        movement.y =
            Input.GetAxisRaw("Vertical");

        movement =
            movement.normalized;

        UpdateAnimation();
        UpdateFacing();

        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -=
                Time.deltaTime;
        }
    }


    private void FixedUpdate()
    {
        if (invincibilityTimer <= 0f)
        {
            rb.MovePosition(
                rb.position +
                movement *
                moveSpeed *
                Time.fixedDeltaTime
            );
        }
    }


    // =====================================================
    // ANIMATION
    // =====================================================

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        bool isMoving =
            movement.sqrMagnitude > 0.01f;

        animator.SetBool(
            "IsMoving",
            isMoving
        );
    }


    // =====================================================
    // FACING
    // =====================================================

    private void UpdateFacing()
    {
        if (spriteRenderer == null)
            return;

        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }


    // =====================================================
    // DAMAGE
    // =====================================================

    public void TakeDamage(
        float damage,
        Vector2 hitDirection
    )
    {
        if (damage <= 0f)
            return;

        if (invincibilityTimer > 0f)
            return;

        currentHealth -= damage;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "Player HP: " +
            currentHealth +
            " / " +
            maxHealth
        );

        invincibilityTimer =
            hitInvincibilityTime;

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(
                hitFlashCoroutine
            );
        }

        hitFlashCoroutine =
            StartCoroutine(
                HitFlash()
            );

        ApplyKnockback(
            hitDirection
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    // =====================================================
    // KNOCKBACK
    // =====================================================

    private void ApplyKnockback(
        Vector2 direction
    )
    {
        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        rb.linearVelocity =
            Vector2.zero;

        rb.linearVelocity =
            direction *
            knockbackForce;
    }


    // =====================================================
    // HIT FLASH
    // =====================================================

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color =
            Color.red;

        yield return new WaitForSeconds(
            hitFlashDuration
        );

        spriteRenderer.color =
            Color.white;

        hitFlashCoroutine = null;
    }


    // =====================================================
    // DEATH
    // =====================================================

    private void Die()
    {
        Debug.Log("PLAYER DEAD");

        // Game Over ทำทีหลัง
    }
}
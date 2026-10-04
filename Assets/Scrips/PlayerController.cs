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


    [Header("Player Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;


    private float currentHealth;
    private float invincibilityTimer;


    private Rigidbody2D rb;
    private Animator animator;


    private Vector2 movement;


    private Coroutine hitFlashCoroutine;


    // =====================================================
    // UI LOCK
    // =====================================================

    private bool isUIOpen = false;


    // =====================================================
    // PUBLIC
    // =====================================================

    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // ถ้าเปิด UI อยู่
        if (isUIOpen)
        {
            movement = Vector2.zero;

            if (animator != null)
            {
                animator.SetBool(
                    "IsMoving",
                    false
                );
            }

            return;
        }


        GetInput();

        UpdateAnimation();

        UpdateInvincibility();
    }


    // =====================================================
    // FIXED UPDATE
    // =====================================================

    private void FixedUpdate()
    {
        // หยุดการเดินตอนเปิด UI
        if (isUIOpen)
        {
            if (rb != null)
            {
                rb.linearVelocity =
                    Vector2.zero;
            }

            return;
        }


        Move();
    }


    // =====================================================
    // UI LOCK
    // =====================================================

    public void SetUIOpen(bool value)
    {
        isUIOpen = value;


        if (isUIOpen)
        {
            movement = Vector2.zero;


            if (rb != null)
            {
                rb.linearVelocity =
                    Vector2.zero;
            }


            if (animator != null)
            {
                animator.SetBool(
                    "IsMoving",
                    false
                );
            }
        }
    }


    // =====================================================
    // INPUT
    // =====================================================

    private void GetInput()
    {
        movement.x =
            Input.GetAxisRaw("Horizontal");

        movement.y =
            Input.GetAxisRaw("Vertical");

        movement =
            movement.normalized;
    }


    // =====================================================
    // MOVEMENT
    // =====================================================

    private void Move()
    {
        if (rb == null)
            return;


        rb.MovePosition(
            rb.position +
            movement *
            moveSpeed *
            Time.fixedDeltaTime
        );
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


        animator.SetFloat(
            "MoveX",
            movement.x
        );


        animator.SetFloat(
            "MoveY",
            movement.y
        );
    }


    // =====================================================
    // INVINCIBILITY
    // =====================================================

    private void UpdateInvincibility()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -=
                Time.deltaTime;
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
    // HEAL
    // =====================================================

    public void Heal(float amount)
    {
        if (amount <= 0f)
            return;


        if (currentHealth <= 0f)
            return;


        currentHealth += amount;


        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );


        Debug.Log(
            "Player Heal: +" +
            amount +
            " | HP: " +
            currentHealth +
            " / " +
            maxHealth
        );
    }


    // =====================================================
    // KNOCKBACK
    // =====================================================

    private void ApplyKnockback(
        Vector2 direction
    )
    {
        if (rb == null)
            return;


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
        Debug.Log(
            "PLAYER DEAD"
        );
    }
    public void IncreaseMaxHealth(float amount)
    {
        if (amount <= 0f)
            return;

        maxHealth += amount;

        currentHealth += amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "Max Health เพิ่มเป็น: " +
            maxHealth
        );
    }
}
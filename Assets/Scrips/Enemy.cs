using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum SpriteFacing
    {
        Right,
        Left
    }

    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Sprite")]
    [SerializeField] private SpriteFacing spriteFacing = SpriteFacing.Right;

    [Header("Attack")]
    [SerializeField] private float contactDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Hit Flash")]
    [SerializeField] private float flashDuration = 0.08f;

    private float currentHealth;
    private float nextAttackTime;

    private Transform player;
    private SpriteRenderer spriteRenderer;

    private Coroutine hitFlashCoroutine;


    private void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }


    private void Update()
    {
        MoveTowardsPlayer();
        FacePlayer();
    }


    // =====================================================
    // MOVEMENT
    // =====================================================

    private void MoveTowardsPlayer()
    {
        if (player == null)
            return;

        Vector2 direction =
            player.position -
            transform.position;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        direction.Normalize();

        transform.position +=
            (Vector3)direction *
            moveSpeed *
            Time.deltaTime;
    }


    // =====================================================
    // FACE PLAYER
    // =====================================================

    private void FacePlayer()
    {
        if (player == null)
            return;

        if (spriteRenderer == null)
            return;

        float directionX =
            player.position.x -
            transform.position.x;

        if (Mathf.Abs(directionX) <= 0.01f)
            return;


        // Sprite ต้นฉบับหันขวา
        if (spriteFacing == SpriteFacing.Right)
        {
            if (directionX > 0f)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }


        // Sprite ต้นฉบับหันซ้าย
        else
        {
            if (directionX < 0f)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
    }


    // =====================================================
    // HEALTH
    // =====================================================

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
            return;

        currentHealth -= damage;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "Enemy HP: " +
            currentHealth +
            " / " +
            maxHealth
        );

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine =
            StartCoroutine(HitFlash());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    // =====================================================
    // HIT FLASH
    // =====================================================

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(
            flashDuration
        );

        spriteRenderer.color = Color.white;

        hitFlashCoroutine = null;
    }


    // =====================================================
    // ATTACK PLAYER
    // =====================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        TryAttack(collision.gameObject);
    }


    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        TryAttack(collision.gameObject);
    }


    private void TryAttack(GameObject target)
    {
        if (Time.time < nextAttackTime)
            return;

        PlayerController playerController =
            target.GetComponent<PlayerController>();

        if (playerController == null)
            return;

        Vector2 knockbackDirection =
            target.transform.position -
            transform.position;

        playerController.TakeDamage(
            contactDamage,
            knockbackDirection
        );

        nextAttackTime =
            Time.time + attackCooldown;
    }


    // =====================================================
    // DEATH
    // =====================================================

    private void Die()
    {
        Destroy(gameObject);
    }
}
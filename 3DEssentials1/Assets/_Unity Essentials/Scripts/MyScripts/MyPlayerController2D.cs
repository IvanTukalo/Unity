using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;               // Movement speed (same as original)
    public float moveDuration = 0.35f;     // How long (seconds) the player moves per input press

    public InputActionReference moveAction;

    [Header("Audio")]
    public AudioClip deathSound;           // Assign death sound in Inspector
    public AudioClip collectSound;         // Assign collect sound in Inspector

    [Header("Death VFX")]
    public GameObject deathEffect;         // Assign VFX_2D_Burst prefab in Inspector

    // Private variables
    private Rigidbody2D rb;
    private bool isDead = false;
    private AudioSource audioSource;

    // Step movement state
    private bool isMoving = false;         // True while locked into a direction
    private Vector2 lockedDirection;       // The direction locked for this step
    private float moveTimer = 0f;          // Time remaining in current step

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 0f; // Top-down 2D — no gravity

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isMoving)
        {
            // Count down the move timer
            moveTimer -= Time.deltaTime;

            if (moveTimer <= 0f)
            {
                // Step finished — stop
                isMoving = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            // Read input only when not in a step
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
            Vector2 direction = Vector2.zero;

            // Only 4 directions, no diagonals. Prioritize horizontal.
            if (Mathf.Abs(moveInput.x) > 0.5f)
            {
                direction = new Vector2(Mathf.Sign(moveInput.x), 0);
            }
            else if (Mathf.Abs(moveInput.y) > 0.5f)
            {
                direction = new Vector2(0, Mathf.Sign(moveInput.y));
            }

            if (direction != Vector2.zero)
            {
                // Rotate player to face movement direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);

                // Start a step: lock direction and set timer
                lockedDirection = direction;
                moveTimer = moveDuration;
                isMoving = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isMoving)
        {
            // Normal physics velocity — walls will block naturally
            rb.linearVelocity = lockedDirection * speed;
        }
        // When not moving, velocity is already zeroed in Update
    }

    /// <summary>
    /// Called when player collects a collectible. Plays the collect sound.
    /// </summary>
    public void PlayCollectSound()
    {
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
    }

    /// <summary>
    /// Called when player touches an enemy animal. Handles death.
    /// </summary>
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop all movement
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        // Play death sound (PlayClipAtPoint so it finishes even after player is hidden)
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // Spawn death VFX
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Hide player (disable renderer and collider)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log("Player died!");

        // End game after a short delay
        Invoke(nameof(EndGame), 1.5f);
    }

    private void EndGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }
}

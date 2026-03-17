using UnityEngine;
using UnityEngine.InputSystem; 

/// <summary>
/// Moves forward/backward and rotates with WASD/Arrow keys.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Tooltip("Forward/back speed (units/sec).")]
    public float speed = 5.0f;

    [Tooltip("Turn speed (degrees/sec).")]
    public float rotationSpeed = 120.0f;

    private Rigidbody rb; 

    [Tooltip("Base height above ground for levitation.")]
    public float levitationHeight = 0.2f;

    [Tooltip("Amplitude of the up-down levitation movement.")]
    public float levitationAmplitude = 0.1f;

    [Tooltip("Speed of the levitation cycle (cycles per second).")]
    public float levitationSpeed = 1.0f;

    [Tooltip("Multiplier for upward movement speed (slower than downward).")]
    public float upwardSpeedMultiplier = 0.5f;

    [Tooltip("Force applied when jumping.")]
    public float jumpForce = 2.0f;

    [Tooltip("Speed of jump ascent.")]
    public float jumpAscentSpeed = 5.0f;

    [Tooltip("Speed of jump descent.")]
    public float jumpDescentSpeed = 3.0f;

    private float jumpOffset = 0f;
    private bool isJumping = false;
    private bool isAscending = true;

    bool IsGrounded()
    {
        // Check if the player is grounded by casting a ray downwards
        return Physics.Raycast(transform.position, Vector3.down, levitationHeight + levitationAmplitude + 0.1f);
    }

    private void Update()
    {
        // Jumping
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isJumping)
        {
            if (IsGrounded() || jumpOffset == 0f)
            {
                isJumping = true;
                isAscending = true;
            }
        }

        // Handle jump phases
        if (isJumping)
        {
            if (isAscending)
            {
                // Ascent
                jumpOffset += jumpAscentSpeed * Time.deltaTime;
                if (jumpOffset >= jumpForce)
                {
                    jumpOffset = jumpForce;
                    isAscending = false;
                }
            }
            else
            {
                // Descent
                jumpOffset -= jumpDescentSpeed * Time.deltaTime;
                if (jumpOffset <= 0f)
                {
                    jumpOffset = 0f;
                    isJumping = false;
                }
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogWarning("PlayerController needs a Rigidbody.");
        rb.useGravity = false; // Disable gravity for levitation
    }

    private void FixedUpdate() 
    {
        Vector2 moveInput = Vector2.zero;

        // Forward/backward
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)   moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y = -1f;

        // Left/right (rotation)
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x = 1f;

        // Strafe left/right
        float strafeInput = 0f;
        if (Keyboard.current.eKey.isPressed) strafeInput = 1f; // Strafe right
        if (Keyboard.current.qKey.isPressed) strafeInput = -1f; // Strafe left

        // Move in facing direction 
        Vector3 movement = transform.forward * moveInput.y * speed * Time.fixedDeltaTime + transform.right * strafeInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Y-axis rotation (invert when going backwards)
        float turnDirection = moveInput.x;
        if (moveInput.y < 0)
            turnDirection = -turnDirection;

        float turn = turnDirection * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Levitation logic
        float time = Time.time * levitationSpeed;
        float sinValue = Mathf.Sin(time);
        float verticalOffset = sinValue * levitationAmplitude;

        // Make upward movement slower
        if (sinValue > 0)
        {
            verticalOffset *= upwardSpeedMultiplier;
        }

        // Find ground height
        RaycastHit hit;
        float groundHeight = 0f;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, levitationHeight + levitationAmplitude + 1f))
        {
            groundHeight = hit.point.y;
        }

        // Set target height
        float targetY = groundHeight + levitationHeight + verticalOffset + jumpOffset;
        Vector3 targetPosition = new Vector3(rb.position.x, targetY, rb.position.z);
        rb.MovePosition(targetPosition);

        // Reset velocity to prevent drifting from collisions
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Stabilize rotation to prevent flipping (keep level)
        Vector3 euler = rb.rotation.eulerAngles;
        rb.MoveRotation(Quaternion.Euler(0f, euler.y, 0f));
    }
}
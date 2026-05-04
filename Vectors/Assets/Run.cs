using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Run : MonoBehaviour {

    float speed = 10f;
    float sprintMultiplier = 2f;
    float jumpForce = 10f;
    float gravity = -20f;

    CharacterController controller;
    Vector3 velocity;

    void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // Перевірка чи на землі
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -1f;

        // WASD рух
        Vector2 input = Vector2.zero;
        if (kb.wKey.isPressed) input.y += 1f;
        if (kb.sKey.isPressed) input.y -= 1f;
        if (kb.dKey.isPressed) input.x += 1f;
        if (kb.aKey.isPressed) input.x -= 1f;

        Vector3 move = transform.right * input.x + transform.forward * input.y;

        // Прискорення на Shift
        float currentSpeed = speed;
        if (kb.leftShiftKey.isPressed)
            currentSpeed = speed * sprintMultiplier;

        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        // Стрибок на Space
        if (kb.spaceKey.wasPressedThisFrame && controller.isGrounded)
            velocity.y = jumpForce;

        // Гравітація
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

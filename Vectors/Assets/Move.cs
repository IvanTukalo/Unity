using UnityEngine;

public class Move : MonoBehaviour {

    public GameObject goal;
    Vector3 direction;
    public float speed = 0.01f;
    float rotationSpeed = 2f;
    float pushRadius = 2f;
    float pushForce = 0.02f;

    void Start() {
        direction = goal.transform.position - this.transform.position;
    }

    private void LateUpdate() {
        direction = goal.transform.position - this.transform.position;

        // Зупинка
        if (direction.sqrMagnitude < 2.25f)
            return;

        // Рух тільки по горизонталі (без зміни висоти)
        Vector3 velocity = direction.normalized * speed;
        velocity.y = 0f;

        // Відштовхування від інших свиней
        Move[] allPigs = FindObjectsByType<Move>(FindObjectsSortMode.None);
        Vector3 pushAway = Vector3.zero;

        foreach (Move other in allPigs) {
            if (other == this) continue;

            Vector3 diff = this.transform.position - other.transform.position;
            float dist = diff.magnitude;

            if (dist < pushRadius && dist > 0.01f) {
                // Чим ближче — тим сильніше відштовхування
                pushAway += diff.normalized * (pushRadius - dist) * pushForce;
            }
        }
        pushAway.y = 0f;

        this.transform.position = this.transform.position + velocity + pushAway;

        // Плавний поворот
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.Slerp(
            this.transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}

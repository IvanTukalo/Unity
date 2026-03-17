using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed;
    public GameObject onCollectEffect;
    public float verticalAmplitude = 0.05f;
    public float verticalSpeed = 1f;
    public AudioClip collectSound;
    [Range(0f, 1f)]
    public float volume = 1f;

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotation
        transform.Rotate(0, rotationSpeed, 0);

        // Vertical bobbing
        float verticalOffset = Mathf.Sin(Time.time * verticalSpeed) * verticalAmplitude;
        transform.position = startPosition + new Vector3(0, verticalOffset, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy the collectible
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

        // Instantiate the onCollectEffect at the collectible's position and rotation
        if (onCollectEffect != null)
        {
            Instantiate(onCollectEffect, transform.position, transform.rotation);
        }

        // Play collect sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
        }
    }
}

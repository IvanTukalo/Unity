using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCollectible2D : MonoBehaviour
{

    public float rotationSpeed = 0.5f;
    public GameObject onCollectEffect;

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(0, 0, rotationSpeed);
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
        // Check if the other object has a MyPlayerController2D component
        MyPlayerController2D player = other.GetComponent<MyPlayerController2D>();
        if (player != null) {
            
            // Play collect sound via player script
            player.PlayCollectSound();

            // Instantiate the particle effect
            if (onCollectEffect != null)
            {
                Instantiate(onCollectEffect, transform.position, transform.rotation);
            }

            // Destroy the collectible
            Destroy(gameObject);
        }

        
    }


}



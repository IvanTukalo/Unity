using UnityEngine;

/// <summary>
/// Attach this script to enemy animal objects (Cat, Dog).
/// When the player touches the animal, the player dies:
/// - VFX_2D_Burst particles are spawned
/// - Death sound plays
/// - Player disappears
/// - Game ends
/// </summary>
public class EnemyAnimal2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        MyPlayerController2D player = other.GetComponent<MyPlayerController2D>();
        if (player != null)
        {
            // Tell the player to die (handles VFX, sound, and game end)
            player.Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Also handle collision-based contact (in case colliders are not triggers)
        MyPlayerController2D player = collision.gameObject.GetComponent<MyPlayerController2D>();
        if (player != null)
        {
            player.Die();
        }
    }
}

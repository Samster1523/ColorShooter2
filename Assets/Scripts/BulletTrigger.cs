using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BulletTrigger : MonoBehaviour
{
    Bullet b;

    void Awake() { b = GetComponent<Bullet>(); }

    void OnTriggerEnter2D(Collider2D other)
    {
        var bubble = other.GetComponent<Bubble>();
        if (bubble == null) return;

        // Delegate decision to GameManager
        GameManager.I.HandleBulletBubble(b, bubble);
    }
}

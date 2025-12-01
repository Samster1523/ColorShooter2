using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BulletTrigger : MonoBehaviour
{
    Bullet _bullet;

    void Awake()
    {
        _bullet = GetComponent<Bullet>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var bubble = other.GetComponent<NumberBubble>();
        if (bubble == null || GameManager.I == null) return;

        GameManager.I.HandleBulletHit(bubble, _bullet);
    }
}

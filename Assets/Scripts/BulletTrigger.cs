using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BulletTrigger : MonoBehaviour
{
    Bullet b;
    void Awake() { b = GetComponent<Bullet>(); }

    void OnTriggerEnter2D(Collider2D other)
    {
        // placeholder; real logic will live in GameManager when bubbles exist
        var bubble = other.GetComponent<Bubble>(); // Bubble script will come later
        if (bubble == null) return;

        // pseudo: if (b.colorType == bubble.colorType) destroy both & +1 score
        // else pass-through (do nothing)
    }
}

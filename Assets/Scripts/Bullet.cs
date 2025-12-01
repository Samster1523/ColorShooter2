using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;

    bool _active;

    public void Fire(Vector3 startPos)
    {
        transform.position = startPos;
        _active = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!_active) return;

        transform.position += Vector3.up * speed * Time.deltaTime;

        // Off screen → return to pool
        if (transform.position.y > 12f)
        {
            Deactivate();
            if (GameManager.I != null)
                GameManager.I.ReturnBullet(this);
        }
    }

    public void Deactivate()
    {
        _active = false;
        gameObject.SetActive(false);
    }
}

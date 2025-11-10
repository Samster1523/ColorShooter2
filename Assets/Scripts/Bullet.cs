using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ColorType colorType;
    public float speed = 12f;
    private SpriteRenderer _sr;
    private bool _active;

    void Awake() { _sr = GetComponent<SpriteRenderer>(); }

    public void Fire(ColorType c, Color displayColor, Vector3 startPos)
    {
        colorType = c;
        if (_sr) _sr.color = displayColor;
        transform.position = startPos;
        _active = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!_active) return;
        transform.position += Vector3.up * speed * Time.deltaTime;

        // cull offscreen
        if (transform.position.y > 12f)
            gameObject.SetActive(false);
    }
}

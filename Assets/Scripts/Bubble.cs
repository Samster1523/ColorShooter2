using UnityEngine;

public class Bubble : MonoBehaviour
{
    public ColorType colorType;
    public float fallSpeed = 2.2f;

    SpriteRenderer _sr;
    bool _alive;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    // called by spawner / GameManager
    public void Init(ColorType c, Color displayColor, Vector3 startPos, float speed)
    {
        colorType = c;
        if (_sr) _sr.color = displayColor;
        transform.position = startPos;
        fallSpeed = speed;
        _alive = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!_alive) return;
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    public void Despawn()
    {
        _alive = false;
        gameObject.SetActive(false);
    }
}

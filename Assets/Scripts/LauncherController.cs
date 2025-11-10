using UnityEngine;
using UnityEngine.Events;

public class LauncherController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float clampX = 2.8f;

    [Header("Nozzle & Preview")]
    public Transform nozzleTip;
    public SpriteRenderer nozzlePreview;
    public ColorType currentColor = ColorType.Red;

    [System.Serializable] public class ShootEvent : UnityEvent<ColorType, Vector3> { }
    public ShootEvent OnShoot; // GameManager will listen later

    void Start() { UpdateNozzleColor(); }

    void Update()
    {
        HandleMove();
        HandleInput();
    }

    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        transform.position += Vector3.right * h * moveSpeed * Time.deltaTime;

        // simple drag (mouse/touch)
        if (Input.GetMouseButton(0))
        {
            var world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, world.x, 0.25f),
                transform.position.y, 0f);
        }

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -clampX, clampX),
            transform.position.y, 0f);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.C)) NextColor();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            Shoot();
    }

    void NextColor()
    {
        currentColor = (ColorType)(((int)currentColor + 1) % GameColors.Count);
        UpdateNozzleColor();
    }

    void UpdateNozzleColor()
    {
        if (nozzlePreview) nozzlePreview.color = GameColors.ToColor(currentColor);
    }

    void Shoot()
    {
        var spawn = nozzleTip ? nozzleTip.position : transform.position;
        OnShoot?.Invoke(currentColor, spawn);
        Debug.Log($"Shoot pressed — {currentColor} at {spawn}");
    }
}


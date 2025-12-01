using UnityEngine;
using UnityEngine.Events;

public class LauncherController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float xMin = -3.5f;
    public float xMax = 3.5f;

    [Header("Shooting")]
    public Transform nozzleTip;
    public KeyCode shootKey = KeyCode.Space;

    [System.Serializable]
    public class ShootEvent : UnityEvent<Vector3> { }
    public ShootEvent OnShoot;

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");   // A/D or Arrow keys
        Vector3 pos = transform.position;
        pos.x += h * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        transform.position = pos;
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(shootKey) && nozzleTip != null)
        {
            OnShoot?.Invoke(nozzleTip.position);
        }
    }
}

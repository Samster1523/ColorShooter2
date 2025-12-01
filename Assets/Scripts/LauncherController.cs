using UnityEngine;
using UnityEngine.Events;

public class LauncherController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float minX = -3.5f;
    public float maxX = 3.5f;

    [Header("Shooting")]
    public Transform nozzleTip;
    public KeyCode shootKey = KeyCode.Space;

    [System.Serializable]
    public class ShootEvent : UnityEvent<Vector3> { }
    public ShootEvent OnShoot;

    // --- Mobile input variables ---
    private int activeTouchId = -1;
    private Vector2 lastTouchPos;
    public float touchSensitivity = 0.005f;

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        // ================================
        // PC / WebGL movement (A/D keys)
        // ================================
#if UNITY_STANDALONE || UNITY_WEBGL
        float input = Input.GetAxisRaw("Horizontal");  // A/D or Arrow keys
        Vector3 pos = transform.position;
        pos.x += input * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
#endif

        // ================================
        // MOBILE – Touch drag left/right
        // ================================
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 0)
        {
            activeTouchId = -1;
            return;
        }

        Touch t = default;
        bool found = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (activeTouchId == -1 || Input.GetTouch(i).fingerId == activeTouchId)
            {
                t = Input.GetTouch(i);
                activeTouchId = t.fingerId;
                found = true;
                break;
            }
        }

        if (!found) return;

        if (t.phase == TouchPhase.Began)
        {
            lastTouchPos = t.position;
        }
        else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
        {
            Vector2 currentPos = t.position;
            float deltaX = currentPos.x - lastTouchPos.x;
            lastTouchPos = currentPos;

            Vector3 pos = transform.position;
            pos.x += deltaX * touchSensitivity; // convert pixels → world movement
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }
        else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            activeTouchId = -1;
        }
#endif
    }

    void HandleShooting()
    {
        // PC / WebGL spacebar shooting
#if UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetKeyDown(shootKey) && nozzleTip != null)
        {
            OnShoot.Invoke(nozzleTip.position);
        }
#endif
    }

    // ================================
    // MOBILE Shoot Button UI
    // ================================
    public void ShootFromButton()
    {
        if (nozzleTip != null)
            OnShoot.Invoke(nozzleTip.position);
    }
}

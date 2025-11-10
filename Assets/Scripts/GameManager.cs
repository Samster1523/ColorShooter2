using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Refs")]
    public LauncherController launcher;
    public Bullet bulletPrefab;

    SimplePool<Bullet> bulletPool;

    void Awake()
    {
        if (I && I != this) Destroy(gameObject);
        I = this;
    }

    void Start()
    {
        bulletPool = new SimplePool<Bullet>(bulletPrefab, 8, transform);
        if (launcher != null)
            launcher.OnShoot.AddListener(FireBullet);
    }

    void FireBullet(ColorType cType, Vector3 spawn)
    {
        var b = bulletPool.Get();
        b.Fire(cType, GameColors.ToColor(cType), spawn);
    }
}

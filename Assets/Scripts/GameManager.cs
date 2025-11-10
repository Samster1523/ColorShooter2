using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Refs")]
    public LauncherController launcher;
    public Bullet bulletPrefab;
    public Bubble bubblePrefab;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;

    [Header("Spawning")]
    public float spawnInterval = 1.0f;
    public float spawnXMin = -2.5f;
    public float spawnXMax = 2.5f;
    public float spawnY = 9.5f;
    public Vector2 bubbleSpeedRange = new Vector2(1.8f, 3.2f);

    [Header("Gameplay")]
    public float failLineY = -3.0f; // set from launcher nozzleTip during Start
    public bool isOver;

    SimplePool<Bullet> bulletPool;
    SimplePool<Bubble> bubblePool;

    int _score;
    float _timer;

    readonly List<Bubble> _activeBubbles = new List<Bubble>();

    void Awake()
    {
        if (I && I != this) Destroy(gameObject);
        I = this;
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void Start()
    {
        // pools
        bulletPool = new SimplePool<Bullet>(bulletPrefab, 8, transform);
        bubblePool = new SimplePool<Bubble>(bubblePrefab, 16, transform);

        // hook launcher fire
        if (launcher != null)
        {
            launcher.OnShoot.AddListener(FireBullet);
            if (launcher.nozzleTip != null)
                failLineY = launcher.nozzleTip.position.y;
        }

        UpdateScore(0);
        isOver = false;
    }

    void Update()
    {
        if (isOver) return;

        // spawn cadence
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnBubble();
        }

        // fail condition: any bubble crosses fail line
        for (int i = 0; i < _activeBubbles.Count; i++)
        {
            var b = _activeBubbles[i];
            if (b != null && b.gameObject.activeSelf && b.transform.position.y <= failLineY)
            {
                GameOver();
                break;
            }
        }
    }

    // ---- bullets
    void FireBullet(ColorType cType, Vector3 spawn)
    {
        var b = bulletPool.Get();
        b.Fire(cType, GameColors.ToColor(cType), spawn);
    }

    public void ReturnBullet(Bullet b)
    {
        if (!b) return;
        bulletPool.Return(b);
    }

    // ---- bubbles
    void SpawnBubble()
    {
        var bubble = bubblePool.Get();
        var x = Random.Range(spawnXMin, spawnXMax);
        var cType = (ColorType)Random.Range(0, GameColors.Count); // 0..3
        var color = GameColors.ToColor(cType);
        var speed = Random.Range(bubbleSpeedRange.x, bubbleSpeedRange.y);

        bubble.Init(cType, color, new Vector3(x, spawnY, 0), speed);
        _activeBubbles.Add(bubble);
    }

    void DespawnBubble(Bubble b)
    {
        if (!b) return;
        b.Despawn();
        _activeBubbles.Remove(b);
        bubblePool.Return(b);
    }

    // ---- collision decision
    public void HandleBulletBubble(Bullet bullet, Bubble bubble)
    {
        if (isOver || bullet == null || bubble == null) return;

        if (bullet.colorType == bubble.colorType)
        {
            // match → destroy both + score +1
            bullet.gameObject.SetActive(false);
            DespawnBubble(bubble);
            UpdateScore(_score + 1);
        }
        else
        {
            // mismatch → pass-through (do nothing)
        }
    }

    // ---- UI & state
    void UpdateScore(int s)
    {
        _score = s;
        if (scoreText) scoreText.text = $"Score: {_score}";
    }

    public void GameOver()
    {
        isOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (launcher) launcher.enabled = false;

        // stop all bubbles
        foreach (var b in _activeBubbles) if (b) b.Despawn();
        _activeBubbles.Clear();
    }

    public void Restart()
    {
        isOver = false;
        UpdateScore(0);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (launcher) launcher.enabled = true;
    }
}

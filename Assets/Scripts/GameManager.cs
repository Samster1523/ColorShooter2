using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("References")]
    public LauncherController launcher;
    public Bullet bulletPrefab;
    public Bubble bubblePrefab;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;

    [Header("Spawning (Timing & Position)")]
    [Tooltip("Starting delay between bubble spawns (seconds).")]
    public float initialSpawnInterval = 1.6f;
    [Tooltip("Minimum delay between spawns (seconds).")]
    public float minSpawnInterval = 0.5f;
    [Tooltip("How much to reduce the interval at each step.")]
    public float spawnIntervalStep = 0.15f;
    [Tooltip("Increase spawn speed every N points.")]
    public int pointsPerStep = 20;

    [Tooltip("X range where bubbles can spawn.")]
    public float spawnXMin = -2.5f;
    public float spawnXMax = 2.5f;
    [Tooltip("World Y where bubbles spawn (top).")]
    public float spawnY = 9.5f;

    [Header("Bubble Falling Speed (Range)")]
    [Tooltip("Initial bubble fall speed range (units/sec).")]
    public Vector2 bubbleSpeedRange = new Vector2(0.9f, 1.6f);

    [Header("Gameplay")]
    [Tooltip("Fail line Y — set from launcher.nozzleTip at Start if present.")]
    public float failLineY = -3.0f;

    // ---- runtime state ----
    public bool isOver { get; private set; }

    SimplePool<Bullet> _bulletPool;
    SimplePool<Bubble> _bubblePool;

    int _score;
    float _spawnTimer;
    float _currentSpawnInterval;
    int _nextStepScore;

    readonly List<Bubble> _activeBubbles = new List<Bubble>();

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void Start()
    {
        // Pools
        _bulletPool = new SimplePool<Bullet>(bulletPrefab, 8, transform);
        _bubblePool = new SimplePool<Bubble>(bubblePrefab, 16, transform);

        // Hook launcher
        if (launcher)
        {
            launcher.OnShoot.AddListener(FireBullet);
            if (launcher.nozzleTip) failLineY = launcher.nozzleTip.position.y;
        }

        // UI/state
        UpdateScore(0);
        isOver = false;

        // Spawn pacing
        _currentSpawnInterval = initialSpawnInterval;
        _nextStepScore = pointsPerStep;
        _spawnTimer = 0f;
    }

    void Update()
    {
        if (isOver) return;

        // ---- one-at-a-time spawn loop ----
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _currentSpawnInterval)
        {
            _spawnTimer = 0f;       // reset to prevent burst spawns
            SpawnBubble();          // spawn exactly one bubble
        }

        // ---- fail condition: any bubble reaches fail line ----
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

    // ===================== Bullets =====================

    void FireBullet(ColorType cType, Vector3 spawn)
    {
        var b = _bulletPool.Get();
        b.Fire(cType, GameColors.ToColor(cType), spawn);
    }

    public void ReturnBullet(Bullet b)
    {
        if (!b) return;
        _bulletPool.Return(b);
    }

    // ===================== Bubbles =====================

    void SpawnBubble()
    {
        var bubble = _bubblePool.Get();

        float x = Random.Range(spawnXMin, spawnXMax);
        var cType = (ColorType)Random.Range(0, GameColors.Count); // 4 colors: 0..3
        Color c = GameColors.ToColor(cType);
        float speed = Random.Range(bubbleSpeedRange.x, bubbleSpeedRange.y);

        bubble.Init(cType, c, new Vector3(x, spawnY, 0f), speed);
        _activeBubbles.Add(bubble);
    }

    void DespawnBubble(Bubble b)
    {
        if (!b) return;
        b.Despawn();
        _activeBubbles.Remove(b);
        _bubblePool.Return(b);
    }

    // Bullet ↔ Bubble collision decision (called by BulletTrigger)
    public void HandleBulletBubble(Bullet bullet, Bubble bubble)
    {
        if (isOver || bullet == null || bubble == null) return;

        if (bullet.colorType == bubble.colorType)
        {
            bullet.gameObject.SetActive(false); // bullet returns to pool via deactivation path
            DespawnBubble(bubble);
            UpdateScore(_score + 1);
        }
        // else mismatch → pass-through (do nothing)
    }

    // ===================== UI & State =====================

    void UpdateScore(int newScore)
    {
        _score = newScore;
        if (scoreText) scoreText.text = $"Score: {_score}";

        // Every N points, tighten the spawn interval and gently raise fall speed
        if (_score >= _nextStepScore)
        {
            _currentSpawnInterval = Mathf.Max(minSpawnInterval, _currentSpawnInterval - spawnIntervalStep);

            // Optional: nudge fall speeds so difficulty ramps smoothly
            bubbleSpeedRange.x += 0.05f;
            bubbleSpeedRange.y += 0.06f;

            _nextStepScore += pointsPerStep;
        }
    }

    public void GameOver()
    {
        isOver = true;
        if (launcher) launcher.enabled = false;
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // stop & clear all bubbles
        for (int i = 0; i < _activeBubbles.Count; i++)
            if (_activeBubbles[i]) _activeBubbles[i].Despawn();
        _activeBubbles.Clear();
    }

    public void Restart()
    {
        isOver = false;
        if (launcher) launcher.enabled = true;
        if (gameOverPanel) gameOverPanel.SetActive(false);

        UpdateScore(0);

        // Reset pacing
        _currentSpawnInterval = initialSpawnInterval;
        _nextStepScore = pointsPerStep;
        _spawnTimer = 0f;
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("References")]
    public LauncherController launcher;
    public Bullet bulletPrefab;
    public NumberBubble bubblePrefab;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject startPanel;
    public GameObject gameOverPanel;

    [Header("Spawn Area")]
    public float spawnXMin = -2.5f;
    public float spawnXMax = 2.5f;
    public float spawnYMin = 0.5f;
    public float spawnYMax = 4.0f;

    [Header("Bubble HP")]
    public int minHits = 1;
    public int maxHits = 5;

    [Header("Spawn Timing")]
    public float initialSpawnInterval = 1.6f;
    public float minSpawnInterval = 0.6f;
    public float spawnIntervalStep = 0.15f;
    public int pointsPerStep = 15;

    [Header("Game Over Condition")]
    public int maxBubblesOnScreen = 15;

    [Header("Bubble Color")]
    [Tooltip("Base blue-ish color for bubbles.")]
    public Color baseBubbleColor = new Color(0.4f, 0.7f, 1f, 0.45f);

    // runtime
    public bool IsOver { get; private set; }
    bool _hasStarted;

    SimplePool<Bullet> _bulletPool;
    SimplePool<NumberBubble> _bubblePool;

    readonly List<NumberBubble> _activeBubbles = new List<NumberBubble>();

    int _score;
    float _spawnTimer;
    float _currentSpawnInterval;
    int _nextStepScore;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;

        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void Start()
    {
        _bulletPool = new SimplePool<Bullet>(bulletPrefab, 12, transform);
        _bubblePool = new SimplePool<NumberBubble>(bubblePrefab, 20, transform);

        if (launcher != null)
        {
            launcher.OnShoot.RemoveAllListeners();
            launcher.OnShoot.AddListener(FireBullet);
        }

        _currentSpawnInterval = initialSpawnInterval;
        _nextStepScore = pointsPerStep;
        _spawnTimer = 0f;
        IsOver = false;

        _hasStarted = false;
        if (startPanel) startPanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (scoreText) scoreText.gameObject.SetActive(false);
        if (launcher) launcher.enabled = false;

        UpdateScore(0);
    }

    void Update()
    {
        if (IsOver || !_hasStarted) return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _currentSpawnInterval)
        {
            _spawnTimer = 0f;
            TrySpawnBubble();
        }

        if (_activeBubbles.Count >= maxBubblesOnScreen)
        {
            GameOver();
        }
    }

    // ---------- UI actions ----------

    public void StartGame()
    {
        _hasStarted = true;
        IsOver = false;

        if (startPanel) startPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (scoreText) scoreText.gameObject.SetActive(true);
        if (launcher) launcher.enabled = true;

        ClearAllBubbles();
        _currentSpawnInterval = initialSpawnInterval;
        _nextStepScore = pointsPerStep;
        _spawnTimer = 0f;
        UpdateScore(0);
    }

    public void Restart()
    {
        StartGame();
    }

    // ---------- Bullets ----------

    void FireBullet(Vector3 spawnPos)
    {
        var bullet = _bulletPool.Get();
        bullet.Fire(spawnPos);
    }

    public void ReturnBullet(Bullet bullet)
    {
        if (bullet == null) return;
        bullet.Deactivate();
        _bulletPool.Return(bullet);
    }

    // ---------- Bubbles ----------

    void TrySpawnBubble()
    {
        if (_activeBubbles.Count >= maxBubblesOnScreen)
            return;

        var bubble = _bubblePool.Get();

        float x = Random.Range(spawnXMin, spawnXMax);
        float y = Random.Range(spawnYMin, spawnYMax);
        int hp = Random.Range(minHits, maxHits + 1);

        // Slight variation on blue-ish color
        float tint = Random.Range(-0.08f, 0.08f);
        var color = baseBubbleColor + new Color(tint, tint, 0f, 0f);
        color.a = baseBubbleColor.a;

        bubble.Init(hp, new Vector3(x, y, 0f), color);
        _activeBubbles.Add(bubble);
    }

    void RemoveBubble(NumberBubble bubble)
    {
        if (bubble == null) return;
        bubble.Deactivate();
        _activeBubbles.Remove(bubble);
        _bubblePool.Return(bubble);
    }

    public void HandleBulletHit(NumberBubble bubble, Bullet bullet)
    {
        if (IsOver || !_hasStarted) return;

        if (bullet != null)
            ReturnBullet(bullet);

        if (bubble == null) return;
        bubble.TakeHit();   // will call PopNumberBubble when hp <= 0
    }

    public void PopNumberBubble(NumberBubble bubble)
    {
        RemoveBubble(bubble);
        UpdateScore(_score + 1);
    }

    // ---------- Score & Game Over ----------

    void UpdateScore(int newScore)
    {
        _score = newScore;
        if (scoreText)
            scoreText.text = $"Score: {_score}";

        if (_score >= _nextStepScore)
        {
            _currentSpawnInterval = Mathf.Max(minSpawnInterval,
                                              _currentSpawnInterval - spawnIntervalStep);
            _nextStepScore += pointsPerStep;
        }
    }

    public void GameOver()
    {
        if (IsOver) return;
        IsOver = true;
        _hasStarted = false;

        if (launcher) launcher.enabled = false;
        if (scoreText) scoreText.gameObject.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    void ClearAllBubbles()
    {
        for (int i = 0; i < _activeBubbles.Count; i++)
            if (_activeBubbles[i] != null)
                _activeBubbles[i].Deactivate();

        _activeBubbles.Clear();
    }
}

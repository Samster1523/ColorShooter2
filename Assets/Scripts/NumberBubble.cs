using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class NumberBubble : MonoBehaviour
{
    [Header("HP")]
    public int hitsRemaining = 3;

    [Header("Floating Motion")]
    public Vector2 floatAmplitudeRange = new Vector2(0.2f, 0.5f);
    public Vector2 floatSpeedRange = new Vector2(0.5f, 1.2f);

    [Header("Visuals")]
    public SpriteRenderer bubbleRenderer;      // circle sprite
    public TextMeshProUGUI numberText;        // TMP UGUI inside BubbleCanvas

    float _baseY;
    float _floatAmplitude;
    float _floatSpeed;
    float _phaseOffset;

    public void Init(int hp, Vector3 position, Color bubbleColor)
    {
        hitsRemaining = Mathf.Max(1, hp);
        transform.position = position;

        if (!bubbleRenderer)
            bubbleRenderer = GetComponent<SpriteRenderer>();

        if (bubbleRenderer != null)
            bubbleRenderer.color = bubbleColor;

        _baseY = position.y;
        _floatAmplitude = Random.Range(floatAmplitudeRange.x, floatAmplitudeRange.y);
        _floatSpeed = Random.Range(floatSpeedRange.x, floatSpeedRange.y);
        _phaseOffset = Random.value * Mathf.PI * 2f;

        SetupText();
        UpdateLabel();

        gameObject.SetActive(true);
    }

    void Start()
    {
        // For manually placed test bubbles
        _baseY = transform.position.y;
        if (_floatAmplitude == 0f)
            _floatAmplitude = Random.Range(floatAmplitudeRange.x, floatAmplitudeRange.y);
        if (_floatSpeed == 0f)
            _floatSpeed = Random.Range(floatSpeedRange.x, floatSpeedRange.y);
        if (_phaseOffset == 0f)
            _phaseOffset = Random.value * Mathf.PI * 2f;

        SetupText();
        UpdateLabel();
    }

    void Update()
    {
        float y = _baseY + Mathf.Sin(Time.time * _floatSpeed + _phaseOffset) * _floatAmplitude;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public void TakeHit()
    {
        hitsRemaining--;

        if (hitsRemaining <= 0)
        {
            if (GameManager.I != null)
                GameManager.I.PopNumberBubble(this);
            else
                Deactivate();
        }
        else
        {
            UpdateLabel();
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void UpdateLabel()
    {
        if (numberText == null) return;

        numberText.text = hitsRemaining.ToString();
        numberText.enableAutoSizing = false;
        numberText.fontSize = 40f;
        numberText.color = Color.white;
    }

    void SetupText()
    {
        if (numberText == null) return;

        // put text in front of the bubble
        var canvas = numberText.GetComponentInParent<Canvas>();
        if (canvas != null && bubbleRenderer != null)
        {
            canvas.sortingLayerID = bubbleRenderer.sortingLayerID;
            canvas.sortingOrder = bubbleRenderer.sortingOrder + 1;
        }

        // center the rect
        var rt = numberText.rectTransform;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(80f, 80f);
        rt.localScale = Vector3.one;
    }
}

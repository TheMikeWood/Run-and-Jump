using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Backgrounds")]
    public SpriteRenderer backgroundRenderer;
    public Sprite daySprite;
    public Sprite sunsetSprite;
    public Sprite nightSprite;
    public Sprite twistedSprite;

    [Header("Speed Thresholds")]
    public float sunsetSpeed = 10f;   // matches StreetLamp.glowStartSpeed
    public float nightSpeed = 20f;    // matches StreetLamp.glowFullSpeed
    public float twistedSpeed = 35f;

    [Header("Crossfade")]
    public float crossfadeDuration = 1.5f;

    private enum Phase { Day, Sunset, Night, Twisted, Crossfading }
    private Phase currentPhase = Phase.Day;
    private Phase nextPhase;

    private SpriteRenderer backRenderer;
    private float crossfadeTimer;

    private void Awake()
    {
        if (backgroundRenderer == null)
        {
            Debug.LogError("DayNightCycle: backgroundRenderer not assigned.");
            enabled = false;
            return;
        }

        backgroundRenderer.sprite = daySprite;
        backgroundRenderer.color = Color.white;

        GameObject backObj = new GameObject("BackgroundBack");
        backObj.transform.SetParent(transform);
        backObj.transform.localPosition = Vector3.zero;
        backObj.transform.localScale = Vector3.one;

        backRenderer = backObj.AddComponent<SpriteRenderer>();
        backRenderer.sortingLayerName = backgroundRenderer.sortingLayerName;
        backRenderer.sortingOrder = backgroundRenderer.sortingOrder - 1;
        backRenderer.material = backgroundRenderer.material;
        backRenderer.drawMode = backgroundRenderer.drawMode;
        backRenderer.size = backgroundRenderer.size;
        backRenderer.color = new Color(1f, 1f, 1f, 0f);
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        float speed = GameManager.Instance.gameSpeed;

        if (currentPhase == Phase.Crossfading)
        {
            TickCrossfade();
            return;
        }

        if (currentPhase == Phase.Day && speed >= sunsetSpeed)
            BeginCrossfade(Phase.Sunset, sunsetSprite);
        else if (currentPhase == Phase.Sunset && speed >= nightSpeed)
            BeginCrossfade(Phase.Night, nightSprite);
        else if (currentPhase == Phase.Night && speed >= twistedSpeed)
            BeginCrossfade(Phase.Twisted, twistedSprite);
    }

    private void BeginCrossfade(Phase target, Sprite incoming)
    {
        if (incoming == null)
        {
            Debug.LogWarning($"DayNightCycle: sprite for {target} is not assigned.");
            return;
        }

        nextPhase = target;
        currentPhase = Phase.Crossfading;
        crossfadeTimer = 0f;

        backgroundRenderer.color = Color.white;
        backRenderer.sprite = incoming;
        backRenderer.color = new Color(1f, 1f, 1f, 0f);
    }

    private void TickCrossfade()
    {
        crossfadeTimer += Time.deltaTime;
        float t = Mathf.Clamp01(crossfadeTimer / crossfadeDuration);

        Color front = backgroundRenderer.color;
        front.a = 1f - t;
        backgroundRenderer.color = front;

        Color back = backRenderer.color;
        back.a = t;
        backRenderer.color = back;

        if (t >= 1f)
        {
            backgroundRenderer.sprite = backRenderer.sprite;
            backgroundRenderer.color = Color.white;
            backRenderer.sprite = null;
            backRenderer.color = new Color(1f, 1f, 1f, 0f);
            currentPhase = nextPhase;
        }
    }

    public void ResetCycle()
    {
        currentPhase = Phase.Day;
        crossfadeTimer = 0f;
        backgroundRenderer.sprite = daySprite;
        backgroundRenderer.color = Color.white;
        backRenderer.sprite = null;
        backRenderer.color = new Color(1f, 1f, 1f, 0f);
    }
}
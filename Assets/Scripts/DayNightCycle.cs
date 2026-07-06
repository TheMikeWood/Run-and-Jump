using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Backgrounds")]
    public SpriteRenderer backgroundRenderer;
    public Sprite daySprite;
    public Sprite sunsetSprite;
    public Sprite nightSprite;
    public Sprite twistedSprite;

    [Header("Crossfade")]
    public float crossfadeDuration = 1.5f;

    private enum Phase
    {
        Day,
        Sunset,
        Night,
        Twisted,
        Crossfading,
    }

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
        if (currentPhase == Phase.Crossfading)
        {
            TickCrossfade();
        }
    }

    public void SetDay()
    {
        SetPhase(Phase.Day, daySprite);
    }

    public void SetSunset()
    {
        SetPhase(Phase.Sunset, sunsetSprite);
    }

    public void SetNight()
    {
        SetPhase(Phase.Night, nightSprite);
    }

    public void SetTwisted()
    {
        SetPhase(Phase.Twisted, twistedSprite);
    }

    private void SetPhase(Phase targetPhase, Sprite targetSprite)
    {
        if (targetSprite == null)
        {
            Debug.LogWarning($"DayNightCycle: sprite for {targetPhase} is not assigned.");
            return;
        }

        if (currentPhase == targetPhase)
            return;

        BeginCrossfade(targetPhase, targetSprite);
    }

    private void BeginCrossfade(Phase target, Sprite incoming)
    {
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
        nextPhase = Phase.Day;
        crossfadeTimer = 0f;

        backgroundRenderer.sprite = daySprite;
        backgroundRenderer.color = Color.white;

        if (backRenderer != null)
        {
            backRenderer.sprite = null;
            backRenderer.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}

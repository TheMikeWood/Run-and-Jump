using UnityEngine;

public class StreetLamp : MonoBehaviour
{
    public SpriteRenderer glowSprite;
    public float glowStartSpeed = 10f; // when lamps start turning on
    public float glowFullSpeed = 20f;  // when lamps are fully on
    private float flickerTimer;

private void Update()
{
    if (glowSprite == null) return;

    float t = Mathf.InverseLerp(glowStartSpeed, glowFullSpeed,
              GameManager.Instance.gameSpeed);

    // flicker in the middle range
    float alpha = t;
    if (t > 0.3f && t < 0.7f)
    {
        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            alpha = Random.value > 0.5f ? t : 0f;
            flickerTimer = Random.Range(0.05f, 0.2f);
        }
    }

    Color c = glowSprite.color;
    c.a = alpha;
    glowSprite.color = c;
}

    
}
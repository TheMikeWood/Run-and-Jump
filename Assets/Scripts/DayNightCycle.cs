using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public SpriteRenderer[] backgroundLayers;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f, 1f);

    private void Update()
    {
        float t = Mathf.Clamp01(GameManager.Instance.gameSpeed / 20f);
        
        Color current = Color.Lerp(dayColor, nightColor, t);
        foreach (var sr in backgroundLayers)
        {
            sr.color = current;
        }

        if (GameManager.Instance.musicSource != null)
        {
            GameManager.Instance.musicSource.pitch = Mathf.Lerp(1f, 0.85f, t);
        }
    }
}
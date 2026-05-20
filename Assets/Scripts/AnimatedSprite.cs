using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] runSprites;
    public Sprite[] jumpSprites;

    private SpriteRenderer spriteRenderer;
    private Sprite[] currentSprites;
    private int frame;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EnsureSpriteRenderer();

        PlayRunAnimation();

        CancelInvoke();
        Invoke(nameof(Animate), 0f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void EnsureSpriteRenderer()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Animate()
    {
        EnsureSpriteRenderer();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on Player.");
            return;
        }

        if (currentSprites == null || currentSprites.Length == 0)
        {
            return;
        }

        frame++;

        if (frame >= currentSprites.Length)
        {
            frame = 0;
        }

        spriteRenderer.sprite = currentSprites[frame];

        float animationSpeed = 8f;

        if (GameManager.Instance != null && GameManager.Instance.gameSpeed > 0f)
        {
            animationSpeed = GameManager.Instance.gameSpeed;
        }

        Invoke(nameof(Animate), 1f / animationSpeed);
    }

    public void PlayRunAnimation()
    {
        EnsureSpriteRenderer();

        if (currentSprites == runSprites)
        {
            return;
        }

        currentSprites = runSprites;
        frame = 0;

        if (spriteRenderer != null && runSprites != null && runSprites.Length > 0)
        {
            spriteRenderer.sprite = runSprites[0];
        }
    }

    public void PlayJumpAnimation()
    {
        EnsureSpriteRenderer();

        if (currentSprites == jumpSprites)
        {
            return;
        }

        currentSprites = jumpSprites;
        frame = 0;

        if (spriteRenderer != null && jumpSprites != null && jumpSprites.Length > 0)
        {
            spriteRenderer.sprite = jumpSprites[0];
        }
    }
}
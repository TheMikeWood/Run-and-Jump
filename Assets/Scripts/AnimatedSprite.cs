using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] runSprites;
    public Sprite[] jumpSprites;
    public Sprite[] doubleJumpSprites;
    public Sprite[] slideSprites;

    private SpriteRenderer spriteRenderer;
    private Sprite[] currentSprites;
    private int frame;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentSprites = null;
        frame = 0;

        PlayRunAnimation();

        CancelInvoke();
        Invoke(nameof(Animate), 0f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Animate()
    {
        if (spriteRenderer == null || currentSprites == null || currentSprites.Length == 0)
        {
            Invoke(nameof(Animate), 0.1f);
            return;
        }

        spriteRenderer.sprite = currentSprites[frame];
        frame = (frame + 1) % currentSprites.Length;

        float animationSpeed = 8f;

        if (GameManager.Instance != null && GameManager.Instance.gameSpeed > 0f)
            animationSpeed = GameManager.Instance.gameSpeed;

        Invoke(nameof(Animate), 1f / animationSpeed);
    }

    private void SetAnimation(Sprite[] sprites)
    {
        if (sprites == null || sprites.Length == 0)
            return;

        // Prevent the same animation from restarting every frame
        if (currentSprites == sprites)
            return;

        currentSprites = sprites;
        frame = 0;

        if (spriteRenderer != null)
            spriteRenderer.sprite = currentSprites[0];
    }

    public void PlayRunAnimation()
    {
        SetAnimation(runSprites);
    }

    public void PlayJumpAnimation()
    {
        SetAnimation(jumpSprites);
    }

    public void PlayDoubleJumpAnimation()
    {
        SetAnimation(doubleJumpSprites);
    }

    public void PlaySlideAnimation()
    {
        SetAnimation(slideSprites);
    }
}
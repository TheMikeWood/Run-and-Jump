using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] runSprites;
    public Sprite[] jumpSprites;
    public Sprite[] doubleJumpSprites;
    public Sprite[] slideSprites;
    public Sprite[] deathSprites;

    private SpriteRenderer spriteRenderer;
    private Sprite[] currentSprites;
    private int frame;

    private bool playingDeath = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentSprites = null;
        frame = 0;
        playingDeath = false;

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

        if (playingDeath)
        {
            // Death animation plays once and freezes on the last frame
            if (frame < currentSprites.Length - 1)
            {
                frame++;
                Invoke(nameof(Animate), 0.12f);
            }

            return;
        }

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
        playingDeath = false;
        SetAnimation(runSprites);
    }

    public void PlayJumpAnimation()
    {
        playingDeath = false;
        SetAnimation(jumpSprites);
    }

    public void PlayDoubleJumpAnimation()
    {
        playingDeath = false;
        SetAnimation(doubleJumpSprites);
    }

    public void PlaySlideAnimation()
    {
        playingDeath = false;
        SetAnimation(slideSprites);
    }

    public void PlayDeathAnimation()
    {
        playingDeath = true;
        SetAnimation(deathSprites);
    }
}
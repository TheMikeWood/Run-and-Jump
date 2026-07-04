// Player.cs
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController character;
    private AnimatedSprite animatedSprite;

    private Vector3 direction;

    public float gravity = 9.81f * 2f;
    public float jumpForce = 8f;

    [Header("Jump Settings")]
    public int maxJumps = 2;
    private int jumpsUsed = 0;

    [Header("Slide Settings")]
    public float slideDuration = 0.6f;
    private bool isSliding = false;
    private float slideTimer = 0f;

    [Header("Ground Forgiveness")]
    public float coyoteTime = 0.15f;
    private float coyoteTimer = 0f;

    [Header("Hitbox References")]
    [SerializeField] private GameObject runHitbox;
    [SerializeField] private GameObject slideHitbox;

    [Header("Touch Controls")]
    [SerializeField] private float swipeThreshold = 80f;

    private Vector2 touchStartPosition;
    private bool touchStarted = false;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip doubleJumpSfx;
    [SerializeField] private AudioClip slideSfx;

    [Header("Death")]
    [SerializeField] private AudioClip deathSfx;
    private bool isDead = false;

    private bool wasGrounded = false;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        animatedSprite = GetComponent<AnimatedSprite>();

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        direction = Vector3.zero;

        jumpsUsed = 0;

        isSliding = false;
        slideTimer = 0f;

        coyoteTimer = coyoteTime;
        wasGrounded = false;

        touchStarted = false;
        isDead = false;

        if (runHitbox != null)
            runHitbox.SetActive(true);

        if (slideHitbox != null)
            slideHitbox.SetActive(false);

        animatedSprite?.PlayRunAnimation();
    }

    private void Update()
    {
        if (isDead)
            return;

        bool grounded = character.isGrounded;

        if (grounded)
        {
            coyoteTimer = coyoteTime;

            if (direction.y < 0f)
                direction.y = -1f;

            jumpsUsed = 0;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (grounded && !wasGrounded && !isSliding)
        {
            animatedSprite?.PlayRunAnimation();
        }

        // Keyboard jump input
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpsUsed < maxJumps)
            {
                Jump();
            }
        }

        // Keyboard slide input
        if (
            Input.GetKeyDown(KeyCode.LeftControl) ||
            Input.GetKeyDown(KeyCode.RightControl) ||
            Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.S)
        )
        {
            if (coyoteTimer > 0f && !isSliding)
            {
                StartSlide();
            }
        }

        // Phone / touch input
        HandleTouchInput();

        // Slide timer
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }

        direction.y -= gravity * Time.deltaTime;

        character.Move(direction * Time.deltaTime);

        wasGrounded = grounded;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount <= 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            touchStartPosition = touch.position;
            touchStarted = true;
        }

        if (touch.phase == TouchPhase.Ended && touchStarted)
        {
            Vector2 touchEndPosition = touch.position;
            Vector2 swipeDelta = touchEndPosition - touchStartPosition;

            bool isSwipeDown =
                swipeDelta.y < -swipeThreshold &&
                Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x);

            if (isSwipeDown)
            {
                if (coyoteTimer > 0f && !isSliding)
                {
                    StartSlide();
                }
            }
            else
            {
                if (jumpsUsed < maxJumps)
                {
                    Jump();
                }
            }

            touchStarted = false;
        }
    }

    private void Jump()
    {
        direction.y = jumpForce;
        jumpsUsed++;

        // This lets the player jump out of a slide.
        if (isSliding)
            EndSlide();

        if (jumpsUsed == 1)
        {
            animatedSprite?.PlayJumpAnimation();
            PlaySfx(jumpSfx);
        }
        else if (jumpsUsed == 2)
        {
            animatedSprite?.PlayDoubleJumpAnimation();

            if (doubleJumpSfx != null)
                PlaySfx(doubleJumpSfx);
            else
                PlaySfx(jumpSfx);
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        if (runHitbox != null)
            runHitbox.SetActive(false);

        if (slideHitbox != null)
            slideHitbox.SetActive(true);

        animatedSprite?.PlaySlideAnimation();
        PlaySfx(slideSfx);
    }

    private void EndSlide()
    {
        isSliding = false;

        if (slideHitbox != null)
            slideHitbox.SetActive(false);

        if (runHitbox != null)
            runHitbox.SetActive(true);

        if (character.isGrounded)
        {
            animatedSprite?.PlayRunAnimation();
        }
    }

    public void PlayDeath()
    {
        if (isDead)
            return;

        isDead = true;
        direction = Vector3.zero;
        isSliding = false;
        slideTimer = 0f;

        if (runHitbox != null)
            runHitbox.SetActive(false);

        if (slideHitbox != null)
            slideHitbox.SetActive(false);

        animatedSprite?.PlayDeathAnimation();
        PlaySfx(deathSfx);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}
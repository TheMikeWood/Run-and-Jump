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

    private bool wasGrounded = false;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        animatedSprite = GetComponent<AnimatedSprite>();
    }

    private void OnEnable()
    {
        direction = Vector3.zero;

        jumpsUsed = 0;

        isSliding = false;
        slideTimer = 0f;

        coyoteTimer = coyoteTime;
        wasGrounded = false;

        if (runHitbox != null)
            runHitbox.SetActive(true);

        if (slideHitbox != null)
            slideHitbox.SetActive(false);

        animatedSprite?.PlayRunAnimation();
    }

    private void Update()
    {
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

        // Jump input
        if (Input.GetButtonDown("Jump") && !isSliding)
        {
            if (jumpsUsed < maxJumps)
            {
                Jump();
            }
        }

        // Slide input
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

    private void Jump()
    {
        direction.y = jumpForce;
        jumpsUsed++;

        // Stop slide if somehow jump happens during slide state
        if (isSliding)
            EndSlide();

        if (jumpsUsed == 1)
        {
            animatedSprite?.PlayJumpAnimation();
        }
        else if (jumpsUsed == 2)
        {
            animatedSprite?.PlayDoubleJumpAnimation();
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
}
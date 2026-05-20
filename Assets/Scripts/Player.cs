using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;
    private AnimatedSprite animatedSprite;

    public float gravity = 9.81f * 2f;
    public float jumpForce = 8f;

    private bool isJumping = false; // track state explicitly

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        animatedSprite = GetComponent<AnimatedSprite>();
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
        isJumping = false;
        animatedSprite?.PlayRunAnimation();
    }

    private void Update()
    {
        direction += gravity * Time.deltaTime * Vector3.down;

        if (character.isGrounded)
        {
            direction = Vector3.down;

            if (Input.GetButton("Jump"))
            {
                direction = Vector3.up * jumpForce;

                if (!isJumping) // only switch once
                {
                    isJumping = true;
                    animatedSprite?.PlayJumpAnimation();
                }
            }
            else if (isJumping) // just landed — switch back once
            {
                isJumping = false;
                animatedSprite?.PlayRunAnimation();
            }
        }

        character.Move(direction * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
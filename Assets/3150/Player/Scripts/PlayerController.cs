using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float speed = 5f;
    public float jumpForce = 10f;
    public bool facingRight = true;

    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float horizontal;
    private bool jumpPressed;
    private bool isGrounded;
    private bool isFalling;
    private float yVelocity;
    private bool fallStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        jumpPressed = Input.GetButtonDown("Jump");

        if (isGrounded)
        {
            animator.SetBool("isGrounded", false);
            animator.SetBool("isFalling", false);
        }
        if (fallStarted)
        {
            isFalling = true;
        }
        
        if(jumpPressed && isGrounded)
        {
            Jump();
        }

        if(horizontal > 0 && !facingRight)
        {
            ChangeDirection();
            facingRight = true;
        }
        else if (horizontal < 0 && facingRight)
        {
            ChangeDirection();
            facingRight = false;
        }
        Move();
    }

    void Jump()
    {
        yVelocity = jumpForce;
        animator.SetTrigger("jump");
    }

    void Move()
    {
        rb2D.linearVelocity = new Vector2(horizontal * speed, yVelocity);

        animator.SetBool("isWalking", horizontal != 0);

        if (rb2D.linearVelocity.y < 0)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }

    void ChangeDirection()
    {
        animator.SetTrigger("changeDirection");
    }

    void FlipCharacter()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        Debug.Log("Flipped");
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("Grounded");
            isGrounded = true;
        }
        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}

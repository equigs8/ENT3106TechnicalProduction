using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float walkingSpeed = 1f;
    public float speed = 1f;
    public float runningSpeed = 4f;
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
    private bool jumping;
    private bool isRunning;
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
            animator.SetBool("isGrounded", true);
            animator.SetBool("isFalling", false);
        }
        else
        {
            animator.SetBool("isGrounded", false);
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
        animator.SetFloat("yVelocity", rb2D.linearVelocity.y);
        Move();
    }

    void Jump()
    {
        
        animator.SetTrigger("Jump");
        animator.SetBool("jumping", true);
        jumping = true;
    }

    void JumpTrigger()
    {
        rb2D.AddForce(Vector2.up * jumpForce);
    }

    void Move()
    {
        if (Input.GetKey("left shift"))
        {
            speed = runningSpeed;
        }
        else
        {
            speed =  walkingSpeed;
        }
        rb2D.linearVelocity = new Vector2(horizontal * speed, rb2D.linearVelocity.y);

        if(Mathf.Abs(rb2D.linearVelocity.x) <= 1 && Mathf.Abs(rb2D.linearVelocity.x) > 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            isRunning = false;
            animator.SetFloat("xVelocity", walkingSpeed);
        }else if (Mathf.Abs(rb2D.linearVelocity.x)> 1)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
            isRunning = true;
            animator.SetFloat("xVelocity", runningSpeed);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            isRunning = false;
            animator.SetFloat("xVelocity", 0);
        }
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


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkingSpeed = 1f;
    public float speed = 1f;
    public float runningSpeed = 4f;
 
    public bool facingRight = true;
    public bool turnAnimationFinshed = true;

    

    [Header("Jumping")]
    public float jumpForce = 10f;
    public float wallJumpForce = 300f;

    [Header("Gravity")]
    public float baseGravity = 1f;
    public float fallGravityMult = 2f;
    public float maxFallSpeed = 20f;


    [Header("Ground Check")]
    public Transform groundCheck;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Wall Check")]
    public Transform wallCheck;
    public Vector2 wallCheckSize;
    public LayerMask wallLayer;
    [Header("Wall Slide")]
    public bool isWallSliding;
    public float wallSlideSpeed = 1f;

    [Header("Wall Jump")]
    private bool isWallJumping;
    public bool wallJumpRelease;

    [Header("Attack")]
    public float lastComboEnd = 0f;
    public float cooldownTime = 0.5f;
    public int comboCounter = 0;
    private float lastPressed = 0f;
    public float comboWindow;
    public bool isComboWindowOver;
    [Header("Ledge Grab")]
    public Transform ledgeCheck;
    public Vector2 ledgeCheckSize;
    public LayerMask ledgeLayer;
    public bool isLedgeGrabbing;

    [Header("Ledge Info")]
    [SerializeField] private Vector3 ledgeGrabOffset1;
    [SerializeField] private Vector3 ledgeGrabOffset2;
    private Vector3 climbBegunPosition;
    private Vector3 climbEndPosition;
    

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;


    //Components
    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float horizontal;
    private bool jumpPressed;
    
    private bool isFalling;
    private float yVelocity;
    private bool fallStarted;
    private bool jumping;
    private bool isRunning;
    private bool isAttacking;
    private bool canAttack;
    private bool attackEnded;

    private bool lockHorizontalMovement;
    private bool isGrabable;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(maxHealth > 0)
        {
            currentHealth = maxHealth;
        }
        else
        {
            Debug.LogError("Max Health is not set");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //horizontal = Input.GetAxis("Horizontal");
        //jumpPressed = Input.GetButtonDown("Jump");
        

        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded)
        {
            animator.SetBool("isGrounded", true);
            animator.SetBool("isFalling", false);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }

        animator.SetBool("isWallSliding", isWallSliding);

        if (fallStarted)
        {
            isFalling = true;
        }
        
        if(jumpPressed && isGrounded)
        {
            //Jump();
        }else if (jumpPressed && isWallSliding)
        {
            //WallJump();
        }else if (jumpPressed && isLedgeGrabbing)
        {
            //LedgeGrab();
        }

        // if(horizontal > 0 && !facingRight)
        // {
        //     ChangeDirection();
        //     facingRight = true;
        // }
        // else if (horizontal < 0 && facingRight)
        // {
        //     ChangeDirection();
        //     facingRight = false;
        // }
        animator.SetFloat("yVelocity", rb2D.linearVelocity.y);
        animator.SetFloat("magnitude", rb2D.linearVelocity.magnitude);
        animator.SetBool("attack", isAttacking);
        animator.SetInteger("comboCounter", comboCounter);
        //CheckMovementLock();
        Debug.Log(horizontal);
        GroundCheck();
        LedgeCheck();
        ProcessGravity();
        ProcessWallSlide();
        Flip();
        Move();
        ProcessWallJump();

        if (isComboWindowOver && attackEnded)
        {
            comboCounter = 0;
            isAttacking = false;
            Debug.Log("Attack Ended");
        }
    }
    private void ProcessWallJump()
    {
        if (isWallJumping && wallJumpRelease)
        {
            isWallJumping = false;
        }
        animator.SetBool("wallJumping", isWallJumping);
    }

    private void CheckMovementLock()
    {
        throw new NotImplementedException();
    }

    private void LedgeGrab()
    {
        throw new NotImplementedException();
    }

    void LedgeCheck()
    {
        if (WallCheck() && Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, groundLayer))
        {
            isGrabable = true;
        }
        else
        {
            isGrabable = false;
        }
    }

    void WallJump()
    {
        Debug.Log("Wall Jump");
        animator.SetTrigger("wallJump");
        animator.SetBool("wallJumping", true);
        isWallJumping = true;
        wallJumpRelease = false;
    }
    public void WallJumpRelease()
    {
        Debug.Log("WAll Jump Release");
        wallJumpRelease = true;
        rb2D.AddForce(Vector2.up * wallJumpForce);
        // if (facingRight)
        // {
        //     rb2D.AddForce(Vector2.right * wallJumpForce);
        // }else
        // {
        //     rb2D.AddForce(Vector2.left * wallJumpForce);
        // }
        rb2D.AddForce(Vector2.right * wallJumpForce);
        animator.SetBool("wallJumping", isWallJumping);
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

        // if(lockHorizontalMovement){
        //     rb2D.linearVelocity = new Vector2(0 * speed, rb2D.linearVelocity.y);
        // }else{
        //     rb2D.linearVelocity = new Vector2(horizontal * speed, rb2D.linearVelocity.y);
        // }


        if(WallCheck() && facingRight && horizontal > 0)
        {
            horizontal = 0;
            Debug.Log("Wall check and not facing right");
        }else if (WallCheck() && !facingRight && horizontal < 0)
        {
            horizontal = 0;
            Debug.Log("Wall check and not facing right");
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
        facingRight = !facingRight;
        Vector3 ls = transform.localScale;
        ls.x *= -1;
        transform.localScale = ls;
        turnAnimationFinshed = true;
    }
    public void AttackEnded()
    {

        attackEnded = true;
        
        
    }

    private void Flip()
    {
        if((facingRight && horizontal < 0 || !facingRight && horizontal > 0) && turnAnimationFinshed)
        {
            turnAnimationFinshed = false;
            animator.SetTrigger("changeDirection");
            Debug.Log("Change Direction");
        }
    }

    private bool IsGrounded()
    {
        if(Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
    }
    private void GroundCheck()
    {
        isGrounded = IsGrounded();
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, wallLayer);
    }

    private void ProcessGravity()
    {
        if (rb2D.linearVelocity.y < 0)
        {
            rb2D.gravityScale = baseGravity * fallGravityMult;
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Mathf.Max(rb2D.linearVelocity.y, -maxFallSpeed));

        }
        else
        {
            rb2D.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if (WallCheck())
        {
            //Debug.Log("Wall Check");
        }
        if(!isLedgeGrabbing && !isGrounded && WallCheck() && horizontal != 0)
        {
            //Debug.Log("Horizontal is: " + horizontal);
            isWallSliding = true;
        }
        else if(isWallSliding && !WallCheck())
        {
            isWallSliding = false;
        }else if (isWallSliding && isGrounded)
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Mathf.Max(rb2D.linearVelocity.y, -wallSlideSpeed));
        }
    }

    public void NewMove(InputAction.CallbackContext context)
    {
        if(WallCheck() && facingRight && context.ReadValue<Vector2>().x > 0)
        {
            horizontal = 0;
            Debug.Log("Wall check and not facing right");
        }else if (WallCheck() && !facingRight && context.ReadValue<Vector2>().x < 0)
        {
            horizontal = 0;
            Debug.Log("Wall check and not facing right");
        }else{
            horizontal = context.ReadValue<Vector2>().x;
        }

       
    }

    public void NewJump(InputAction.CallbackContext context)
    {
        if (isWallSliding)
        {
            if (context.performed)
            {
                WallJump();
                Debug.Log("Preform Wall Jump");
            }else if (context.canceled)
            {
                if (isLedgeGrabbing)
                {
                    animator.SetTrigger("grab");
                } 
            }
            
        }else if (isGrounded && isGrabable)
        {
            Debug.Log("grabbable");
            if (context.performed)
            {
                if(true)
                {
                    climbBegunPosition = transform.position + ledgeGrabOffset1;
                    if(!facingRight){
                        ledgeGrabOffset2.x *= -1;
                    }
                    climbEndPosition = transform.position + ledgeGrabOffset2;
                    //transform.position = new Vector3(transform.position.x, transform.position.y + .33f, transform.position.z);
                    transform.position = climbBegunPosition;
                    animator.SetTrigger("climb");
                    Debug.Log("Grab");
                    isLedgeGrabbing = true;
                }
            }
        }
        else if(IsGrounded()){
            animator.SetTrigger("Jump");
            if (context.performed)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            }else if (context.canceled)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y * 0.5f);
            }
        }
    }

    public void ClimbEnded()
    {
        transform.position = climbEndPosition;
        // if (facingRight)
        // {
        //     transform.position = new Vector3(transform.position.x + .52f, transform.position.y, transform.position.z);
        // }else
        // {
        //     transform.position = new Vector3(transform.position.x - .52f, transform.position.y, transform.position.z);
        // }
        // transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        isLedgeGrabbing = false;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attackEnded = false;
            comboCounter++;
            if (comboCounter > 3)
            {
                comboCounter = 0;
            }
            if(!isAttacking){
                isAttacking = true;
                Debug.Log("Combo Number: " + comboCounter);
                if(comboCounter == 1){
                    animator.SetTrigger("attackStart");
                }
                StartCoroutine(ComboTimer());
                
            }
        }
    }

    IEnumerator ComboTimer()
    {
        isComboWindowOver = false;
        Debug.Log("Combo Timer");
        yield return new WaitForSeconds(comboWindow);
        isComboWindowOver = true;
    }
    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Ground")
    //     {
    //         Debug.Log("Grounded");
    //         isGrounded = true;
    //     }
        
    // }

    // void OnCollisionExit2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Ground")
    //     {
    //         isGrounded = false;
    //     }
    // }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(wallCheck.position, wallCheckSize);
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetHealth()
    {
        return currentHealth;
    }
}


// math notes:
// pos y start = -.65
// pos y end = -.33
// ==> 0.32

// pos x start = -.13
// pos x end = .33
// ==> 0.52
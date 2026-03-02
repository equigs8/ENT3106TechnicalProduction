using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    [Header("Inventory")]
    public List<LevelKey> levelKeys = new List<LevelKey>();
    public List<BlockerUpgrade> blockerUpgrades = new List<BlockerUpgrade>();
    public bool inBlockerRange;
    private Blocker blockerInRange;

    [Header("Movement")]
    public float walkingSpeed = 1f;
    public float runningSpeed = 4f;
    private float speed = 1f;
    public bool facingRight = true;
    public bool turnAnimationFinished = true;

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
    public int attackDamage = 1;
    public int comboCounter = 0;
    public float attackRange = .75f;
    public float comboResetTime = 0.5f;
    private float lastAttackTime;
    private bool isAttacking;

    [Header("Ledge Grab")]
    public Transform ledgeCheck;
    public Vector2 ledgeCheckSize;
    public LayerMask ledgeLayer;
    public bool isLedgeGrabbing;

    [Header("Ledge Info")]
    [SerializeField] private Vector3 ledgeGrabOffset1;
    [SerializeField] private Vector3 ledgeGrabOffset2;
    private Vector3 climbEndPosition;
    private bool isGrabbable;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;
    public GameObject healthBar;
    public GameObject deathMessage;

    [Header("Input Buffering")]
    public float bufferWindow = 0.2f;
    private float lastBufferTime;
    private bool isBufferActive;

    // Components
    private Rigidbody2D rb2D;
    private Animator animator;
    private CapsuleCollider2D collider2D;
    private float horizontal;
    public TilemapCollider2D spikeCollider2D;
    public PlatformGameManager manager;



    [Header("Floor Sliding")]
    public bool isFloorSliding;
    public LayerMask spikeLayer;

    [Header("Locking Input")]
    public bool playerControlsLocked;

    [Header("Environment")]
    public int spikeDamge;
    public float spikeKnockbackForce;
    public float KnockbackCooldownTime;

    //public float floorSlideSpeed = 1f;
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<CapsuleCollider2D>();
        
        if (maxHealth > 0) currentHealth = maxHealth;
        else Debug.LogError("Max Health is not set");

        
    }

    void Update()
    {
        
        // Physics Checks
        GroundCheck();
        LedgeCheck();
        
        // Logic Processing
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();
        ProcessRunningSlide();
        
        // Movement and Visuals
        HandleFlipping();
        ApplyMovement();
        UpdateAnimator();
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
    }

    private void LedgeCheck()
    {
        // A ledge is grabbable if we hit a wall but the ledge check (higher up) is clear
        isGrabbable = WallCheck() && !Physics2D.OverlapBox(ledgeCheck.position, ledgeCheckSize, 0, groundLayer) && !Physics2D.OverlapBox(ledgeCheck.position, ledgeCheckSize, 0, wallLayer);
        Debug.Log($"Is Grabbable: {isGrabbable}");
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, wallLayer);
    }

    private void ProcessGravity()
    {
        if (rb2D.linearVelocity.y < 0 && !isWallSliding && !isLedgeGrabbing)
        {
            rb2D.gravityScale = baseGravity * fallGravityMult;
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Mathf.Max(rb2D.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb2D.gravityScale = isLedgeGrabbing ? 0 : baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if (!isLedgeGrabbing && !isGrounded && WallCheck())
        {
            isWallSliding = true;
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Mathf.Max(rb2D.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ApplyMovement()
    {
        if (isLedgeGrabbing || isAttacking || isWallJumping) return;

        if(playerControlsLocked) return;

        speed = Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed;
        rb2D.linearVelocity = new Vector2(horizontal * speed, rb2D.linearVelocity.y);
    }

    private void HandleFlipping()
    {
        if (isAttacking) return;
        // Only flip if grounded to prevent turn-animation bugs in mid-air
        if (isGrounded && turnAnimationFinished)
        {
            if ((facingRight && horizontal < 0) || (!facingRight && horizontal > 0))
            {
                facingRight = !facingRight;
                turnAnimationFinished = false;
                animator.SetTrigger("changeDirection");
            }
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWallSliding", isWallSliding);
        animator.SetBool("isFalling", !isGrounded && rb2D.linearVelocity.y < -0.1f);
        animator.SetFloat("yVelocity", rb2D.linearVelocity.y);
        animator.SetFloat("magnitude", Mathf.Abs(rb2D.linearVelocity.x));
        animator.SetFloat("xVelocity", Mathf.Abs(rb2D.linearVelocity.x));
        animator.SetBool("isWalking", isGrounded && Mathf.Abs(rb2D.linearVelocity.x) > 0.1f && speed == walkingSpeed);
        animator.SetBool("isRunning", isGrounded && Mathf.Abs(rb2D.linearVelocity.x) > 0.1f && speed == runningSpeed);
        animator.SetInteger("comboCounter", comboCounter);
        animator.SetBool("attack", isAttacking);
    }

    // --- Renamed Input System Methods ---

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        
        // Prevent walking into walls
        if (WallCheck() && ((facingRight && input.x > 0) || (!facingRight && input.x < 0)))
        {
            horizontal = 0;
        }
        else
        {
            if (!playerControlsLocked)
            {
                horizontal = input.x;
            }
            else
            {
                horizontal = rb2D.linearVelocity.x;
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && !playerControlsLocked)
        {
            if (isWallSliding)
            {
                WallJump();
            }
            else if (isGrabbable && !isLedgeGrabbing)
            {
                StartLedgeClimb();
            }
            else if (isGrounded)
            {
                animator.SetTrigger("Jump");
                
            }
            
        }

        if (context.canceled)
        {
            if (isLedgeGrabbing) animator.SetTrigger("grab");
            else if (rb2D.linearVelocity.y > 0)
            {
                // Variable jump height
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y * 0.5f);
            }
        }
    }
    public void JumpTrigger()
    {
        // Applies the force when the animation event fires
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
    }
    // --- Internal Logic ---

    private void StartLedgeClimb()
    {
        Debug.Log("Start Ledge Climb");
        Vector3 climbBegunPosition = transform.position + ledgeGrabOffset1;
        Vector3 actualOffset2 = ledgeGrabOffset2;
        if (!facingRight) actualOffset2.x *= -1;
        
        climbEndPosition = transform.position + actualOffset2;
        transform.position = climbBegunPosition;
        
        animator.SetTrigger("climb");
        isLedgeGrabbing = true;
        rb2D.linearVelocity = Vector2.zero;
    }

    void WallJump()
    {
        animator.SetTrigger("wallJump");
        animator.SetBool("wallJumping", true);
        isWallJumping = true;
        wallJumpRelease = false;
    }

    public void WallJumpRelease()
    {
        wallJumpRelease = true;

        rb2D.linearVelocity = Vector2.zero;
        float sideForce = facingRight ? -wallJumpForce : wallJumpForce; // Jump AWAY from wall

        Vector2 jumpDirection = new Vector2(sideForce, wallJumpForce);

        rb2D.AddForce(jumpDirection, ForceMode2D.Impulse);
    }

    private void ProcessWallJump()
    {
        if(isGrounded && wallJumpRelease) isWallJumping = false;
        animator.SetBool("wallJumping", isWallJumping);
    }

    public void FlipCharacter()
    {
        Vector3 ls = transform.localScale;
        ls.x *= -1;
        transform.localScale = ls;
        turnAnimationFinished = true;
    }

    public void FlipCharacterDirection()
    {
        facingRight = !facingRight;
    }

    public void ClimbEnded()
    {
        transform.position = climbEndPosition;
        isLedgeGrabbing = false;
    }

    // --- Attack System ---

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lastBufferTime = Time.time;
            isBufferActive = true;
            if (!isAttacking) ProcessAttack();
        }
    }

    private void ProcessAttack()
    {
        isAttacking = true;
        isBufferActive = false;

        if (Time.time - lastAttackTime > comboResetTime) comboCounter = 0;

        lastAttackTime = Time.time;
        comboCounter = (comboCounter % 3) + 1;

        animator.SetTrigger("attackStart");

        EnemyController enemy = EnemyInAttackingRange();
        if (enemy != null) enemy.TakeDamage(attackDamage);
    }

    public void CheckForBuffer()
    {
        if (isBufferActive && (Time.time - lastBufferTime <= bufferWindow)) ProcessAttack();
        else isBufferActive = false;
    }

    public void ResetAttackState()
    {
        isAttacking = false;
        comboCounter = 0;
    }

    private EnemyController EnemyInAttackingRange()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, attackRange, LayerMask.GetMask("Enemy"));
        return hit.collider != null ? hit.collider.GetComponentInParent<EnemyController>() : null;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("tookDamage");
        if (currentHealth <= 0) {
            animator.SetTrigger("death");
            playerControlsLocked = true;
            healthBar.transform.Find("Fill Area").gameObject.SetActive(false);
        }
    }

    public void Die()
    {
        Destroy(healthBar);
        Destroy(gameObject);
        deathMessage.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(wallCheck.position, wallCheckSize);
        if (isGrabbable)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(ledgeCheck.position, ledgeCheckSize);
        }else{
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(ledgeCheck.position, ledgeCheckSize);
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public void Knockback(bool attackComingFromRight, float force)
    {
        // If the attack comes from the right, we want to push the player Left (negative)
        // If the attack comes from the left, we want to push the player Right (positive)
        float direction = attackComingFromRight ? -1f : 1f;

        // Reset velocity before adding force to ensure the knockback feels consistent
        rb2D.linearVelocity = Vector2.zero; 
        
        // Apply the force
        rb2D.AddForce(new Vector2(direction * force, 0), ForceMode2D.Impulse); 
        
        Debug.Log($"Knockback applied. Direction: {direction}, Force: {force}");

        StartCoroutine(KnockbackCooldown());
    }

    IEnumerator KnockbackCooldown()
    {
        playerControlsLocked = true;
        yield return new WaitForSeconds(KnockbackCooldownTime);
        
        playerControlsLocked = false;
        rb2D.linearVelocity = Vector2.zero;
        horizontal = 0;
        AddToCollisionLayer(spikeLayer);
        Debug.Log("Knockback Cooldown Over");
    }

    public void RunningSlide(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isFloorSliding)
            {
                animator.SetBool("runningSlide", true);
                isFloorSliding = true;
            }
        }
    }

    public void RunningSlideRelease(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            animator.SetBool("runningSlide", false);
            isFloorSliding = false;
        }
    }

    private void ProcessRunningSlide()
    {

        if (isFloorSliding)
        {
            

            if(collider2D == null)
            {
                Debug.Log("Collider is null");
                return;
            }
            Debug.Log("Sliding");
            spikeCollider2D.enabled = false;
            RemoveCollsionLayer(spikeLayer);
        }
        else
        {
            Debug.Log("Not Sliding");
            if (!spikeCollider2D.enabled)
            {
                spikeCollider2D.enabled = true;
            }
            AddToCollisionLayer(spikeLayer);
        }
    }

    void AddToCollisionLayer(LayerMask layer)
    {
        Debug.Log("Added to Collision Layer");
        Physics.IgnoreLayerCollision(8, 10, false);
    }
    void RemoveCollsionLayer(LayerMask layer)
    {
        //collider2D.excludeLayers = layer;
        Debug.Log("Removed from Collision Layer");
        Physics.IgnoreLayerCollision(8, 10, true);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 10)
        {
            Debug.Log("Hit Spikes");
            RemoveCollsionLayer(spikeLayer);
            Knockback(facingRight, spikeKnockbackForce);
            TakeDamage(spikeDamge);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            collision.GetComponent<PlayerPrompt>().PlayerInRange(true);
            blockerInRange = collision.GetComponent<Blocker>();
            inBlockerRange = true;
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            collision.GetComponent<PlayerPrompt>().PlayerInRange(false);
            blockerInRange = null;
            inBlockerRange = false;
        }
    }


    public void RemoveBlocker()
    {
        Debug.Log("Pressed E");
        if (inBlockerRange)
        {
            Debug.Log("BlockerInRange");
            manager.PlayerHasBlockerUpgrade(blockerInRange); 
        }
    }

    internal List<LevelKey> GetLevelKeys()
    {
        return levelKeys;
    }

    internal List<BlockerUpgrade> GetBlockerUpgrades()
    {
        Debug.Log("Blocker upgrades: " + blockerUpgrades.Count);
        return blockerUpgrades;
    }
}
using UnityEngine;

public class ProjectileControler : MonoBehaviour
{

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 200f;
    public float lockInRange = 3f; // Distance at which it stops tracking

    [Header("Damage Settings")]
    public int damage = 1;
    public float knockbackForce = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private bool isLockedOn = false;
    private Vector2 fixedDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Find the player using the PlayerController component
        PlayerController pc = FindObjectOfType<PlayerController>();
        if (pc != null) player = pc.transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isLockedOn)
        {
            if (distanceToPlayer <= lockInRange)
            {
                // Transition to fixed path for dodging
                isLockedOn = true;
                fixedDirection = (player.position - transform.position).normalized;
            }
            else
            {
                // Heat-seeking logic
                Vector2 direction = (Vector2)player.position - rb.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.right).z;
                rb.angularVelocity = -rotateAmount * rotationSpeed;
                rb.linearVelocity = transform.right * speed;
            }
        }
        else
        {
            // Move in the final locked-on direction
            rb.angularVelocity = 0;
            rb.linearVelocity = fixedDirection * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc != null)
        {
            // Use the methods from your PlayerController.cs
            bool attackFromRight = transform.position.x > pc.transform.position.x;
            pc.Knockback(attackFromRight, knockbackForce);
            pc.TakeDamage(damage);
        }
        
        // Destroy skull on impact with anything (player, walls, floor)
        Destroy(gameObject);
    }
}

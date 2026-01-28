using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Week1PlayerController : MonoBehaviour
{

    public float speed = 5f;
    public float jumpForce = 500f;
    public bool facingRight = false;

    private Rigidbody2D rb2D;
    private SpriteRenderer leftArm;
    private SpriteRenderer rightArm;
    private SpriteRenderer leftLeg;
    private SpriteRenderer rightLeg;
    private SpriteRenderer body;
    private SpriteRenderer head;

    public bool isJumping = false;
    public bool isGrounded;
 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetArmsLegsAndHead();
        
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") > 0 && isGrounded)
        {
            Jump();
        }
        Move();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");



        transform.Translate(horizontal * speed * Time.deltaTime, 0, 0);
    }
    void Jump()
    {
        rb2D.AddForce(Vector2.up * jumpForce);
    }
    void ChangeDirection()
    {
        
    }

    void GetArmsLegsAndHead()
    {
        leftArm = transform.Find("LeftArm").GetComponent<SpriteRenderer>();
        rightArm = transform.Find("RightArm").GetComponent<SpriteRenderer>();
        leftLeg = transform.Find("LeftLeg").GetComponent<SpriteRenderer>();
        rightLeg = transform.Find("RightLeg").GetComponent<SpriteRenderer>();
        head = transform.Find("Head").GetComponent<SpriteRenderer>();
        body = transform.Find("Body").GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
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

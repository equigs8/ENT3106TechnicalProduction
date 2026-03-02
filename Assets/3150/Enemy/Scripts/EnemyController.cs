using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType
    {
        NightBorne,
        Necromancer
    }
    [Header("Enemy Info")]
    public EnemyType enemyType;
    [Tooltip("The projectile used by the enemy. Only needed for Necromancer")]
    public GameObject projectilePrefab;
    private GameObject projectile;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer sprite;
    public Transform projectileSpawnPoint;

    [Header("Movement")]
    public float moveSpeed = 1f;
    private bool facingRight = true;
    private bool isMoving = false;
    public float detectPlayerRange = 5f;
    public float movementCoolDown;


    [Header("Combat")]
    public float attackPlayerRange = 1f;
    public float attackCooldown = 1f;
    public int attackDamage = 1;
    public float knockbackForce = 1f;
    public float rangedAttackDamage = 1f;
    public float rangedAttackSpeed = 1f;
    public float healAmount = 1f;
    
    [SerializeField]private bool canAttack = true;
    private bool isAttacking = false;

    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth;

    
    private string[] necromancerAbilities= { "Ranged Attack"};
    //, "Heal", "Close Attack" 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateDirection();
        if (isAttacking)
        {
            canAttack = false;
        }

        if (CheckIfPlayerInDetectRange())
        {
            if(enemyType == EnemyType.Necromancer && canAttack)
            {
                PickRandomAction();
            }
            else
            {
                Move();
            }
            if (CheckIfPlayerInAttackRange() && canAttack)
            {
                
                Attack();
            }
            else if(!CheckIfPlayerInAttackRange() && canAttack)
            {
               
                Move();
            }
            else
            {
                Idle();
            }
        }
        else
        {
            Idle();
        }

        
                

        //If in attack range and can attack, attack. else if in detect range, move towards player. else do nothing
    }

    private void PickRandomAction()
    {
        int random = UnityEngine.Random.Range(0, necromancerAbilities.Length);
        isAttacking = true;
        if (necromancerAbilities[random] == "Ranged Attack")
        {
            RangedAttack();
        }
        else if (necromancerAbilities[random] == "Heal")
        {
            Heal();
        }
        else if (necromancerAbilities[random] == "Close Attack")
        {
            CloseAttack();
        }
        StartCoroutine(AttackCooldown());
    }

    private void CloseAttack()
    {
        throw new NotImplementedException();
    }

    private void Heal()
    {
        throw new NotImplementedException();
    }

    private void RangedAttack()
    {
        animator.SetTrigger("rangedAttack");
    }

    public void EndRangedAttack()
    {
        projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
    }

    void UpdateDirection()
    {
        sprite.flipX = !facingRight;
    }

    
    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator MovementCooldown()
    {
        isMoving = true;
        yield return new WaitForSeconds(movementCoolDown);
        isMoving = false;
    }

    private bool CheckIfPlayerInDetectRange()
    {
        //if player in detect range
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, detectPlayerRange, LayerMask.GetMask("Player"));
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, detectPlayerRange, LayerMask.GetMask("Player"));

        Debug.DrawRay(transform.position, Vector2.right * detectPlayerRange, Color.blue);
        Debug.DrawRay(transform.position, Vector2.left * detectPlayerRange, Color.blue);

        if (rightHit.collider != null || leftHit.collider != null)
        {

            if(rightHit.collider != null)
            {
                facingRight = true;
            }else if(leftHit.collider != null)
            {
                facingRight = false;
            }

            return true;
        }else
        {
            return false;
        }
        
    }

    private bool CheckIfPlayerInAttackRange()
    {
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, attackPlayerRange, LayerMask.GetMask("Player"));
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, attackPlayerRange, LayerMask.GetMask("Player"));

        Debug.DrawRay(transform.position, Vector2.right * attackPlayerRange, Color.red);
        Debug.DrawRay(transform.position, Vector2.left * attackPlayerRange, Color.red);

        if (rightHit.collider != null || leftHit.collider != null)
        {
            return true;
        }else
        {
            return false;
        }
    }

    Transform GetPlayerTransform()
    {
        return GameObject.Find("Player").transform;
    }

    private void Idle()
    {
        animator.SetBool("moving", false);
    }
    private void Move()
    {
        animator.SetBool("moving", true);
        
        transform.position = Vector2.MoveTowards(transform.position, GetPlayerTransform().position, moveSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        animator.SetBool("moving", false);
        animator.SetTrigger("attack");
        isAttacking = true;
        

    }

    public void AttackEnded()
    {   
        
        isAttacking = false;
        StartCoroutine(AttackCooldown());
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("tookDamage");
        HealthCheck();
    }

    public void DealDamage()
    {
        if (CheckIfPlayerInAttackRange())
        {
            PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
            player.TakeDamage(attackDamage);
            player.Knockback(facingRight, knockbackForce);
        }
    }

    public void HealthCheck()
    {
        if (currentHealth <= 0)
        {
            animator.SetTrigger("death");
        }
    }


    public void Die()
    {
        Destroy(gameObject);
    }
   

    internal int GetHealth()
    {
        return currentHealth;
    }

    internal int GetMaxHealth()
    {
        return maxHealth;
    }


    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector2.right * detectPlayerRange, Color.blue);
        Debug.DrawRay(transform.position, Vector2.left * detectPlayerRange, Color.blue);
        Debug.DrawRay(transform.position, Vector2.right * attackPlayerRange, Color.red);
        Debug.DrawRay(transform.position, Vector2.left * attackPlayerRange, Color.red);
    }

    internal void DealDamage(int attackDamage)
    {
        throw new NotImplementedException();
    }
}

using UnityEngine;

public class enemySpriteControler : MonoBehaviour
{

    public EnemyController enemyController;
    

    public void EndAttack()
    {
        enemyController.AttackEnded();
    }
    public void DealDamge()
    {
        enemyController.DealDamage();
    }
    public void Die()
    {
        enemyController.Die();
    }
}

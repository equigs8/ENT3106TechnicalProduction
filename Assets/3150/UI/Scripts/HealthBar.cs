using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [Header("Refrances")]
    public GameObject character;
    private Slider slider;
    public enum HealthBarType
    {
        Player,
        Enemy
    }
    public HealthBarType healthBarType;
    private PlayerController playerController;
    private EnemyController enemyController;

    private int maxHealth;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        if(healthBarType == HealthBarType.Player)
        {
            playerController = character.GetComponent<PlayerController>();
            maxHealth = playerController.GetMaxHealth();
            currentHealth = playerController.GetHealth();

        }
        else
        {
            enemyController = character.GetComponent<EnemyController>();
            maxHealth = enemyController.GetMaxHealth();
            currentHealth = enemyController.GetHealth();
        }
        InitHealthBar(maxHealth, currentHealth);
        Debug.Log("Finished Init Health Bar. Current Health: " + currentHealth);
    }

    void InitHealthBar(int max, int current)
    {
        slider.maxValue = max;
        slider.value = current;
    }

    // Update is called once per frame
    void Update()
    {
        switch (healthBarType)
        {
            case HealthBarType.Player:
                if(maxHealth != playerController.GetMaxHealth() || currentHealth != playerController.GetHealth())
                {
                    maxHealth = playerController.GetMaxHealth();
                    currentHealth = playerController.GetHealth();
                    InitHealthBar(maxHealth, currentHealth);
                }
                break;
            case HealthBarType.Enemy:

                if(maxHealth != enemyController.GetMaxHealth() || currentHealth != enemyController.GetHealth())
                {
                    maxHealth = enemyController.GetMaxHealth();
                    currentHealth = enemyController.GetHealth();
                    InitHealthBar(maxHealth, currentHealth);
                }
                break;
            
        }
        
    }
}

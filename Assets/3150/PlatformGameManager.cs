using System.Collections.Generic;
using UnityEngine;

public class PlatformGameManager : MonoBehaviour
{
    public enum CurrentLevel { Spring, Summer, Autumn, Winter };
    [Header("Levels")]
    [Tooltip("The current level. Is also the default starting level.")]
    public CurrentLevel currentLevel;
    public Level springLevel;
    public Level summerLevel;
    public Level autumnLevel;
    public Level winterLevel;
    private Level[] levels;

    public Level activeLevel;
    private Level requestedLevel;



    [Header("Player")]
    public PlayerController player;
    public List<LevelKey> levelKeys = new List<LevelKey>();
    public List<BlockerUpgrade> blockerUpgrades = new List<BlockerUpgrade>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null) player = GameObject.Find("Player").GetComponent<PlayerController>();

        levels = new Level[] { springLevel, summerLevel, autumnLevel, winterLevel };
        activeLevel = levels[(int)currentLevel];
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentLevel){
            case CurrentLevel.Spring:
                requestedLevel = springLevel;
                break;
            case CurrentLevel.Summer:
                requestedLevel = summerLevel;
                break;
            case CurrentLevel.Autumn:
                requestedLevel = autumnLevel;
                break;
            case CurrentLevel.Winter:
                requestedLevel = winterLevel;
                break;
        }
        
        if (activeLevel != requestedLevel)
        {
            SwitchLevel(requestedLevel);
        }


        CheckPlayerInventory();
        CheckPlayerHealth();
    }

    void SwitchLevel(Level newLevel)
    {
        activeLevel.gameObject.SetActive(false);
        newLevel.gameObject.SetActive(true);
        activeLevel = newLevel;
    }


    void CheckPlayerInventory()
    {
        levelKeys = player.GetLevelKeys();
        blockerUpgrades = player.GetBlockerUpgrades();

        if (levelKeys.Count == 4)
        {
            GameOver(true);
        }
    }

    void CheckPlayerHealth()
    {
        if (player.currentHealth <= 0)
        {
            GameOver(false);
        }
    }

    void GameOver(bool win)
    {
        if (win)
        {
            Debug.Log("You win!");
        }
        else
        {
            Debug.Log("You lose!");
        }
    }
}

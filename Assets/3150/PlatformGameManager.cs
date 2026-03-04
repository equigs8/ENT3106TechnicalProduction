using System;
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

    [Header("Level Keys and Upgrades")]
    public Dictionary<Level, Blocker> levelBlockersDict = new Dictionary<Level, Blocker>();
    public Dictionary<Level, LevelKey> levelKeysDict = new Dictionary<Level, LevelKey>();

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

        player.transform.position = activeLevel.spawnPoint.position;
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
        player.transform.position = activeLevel.spawnPoint.position;
        player.portalControler.ResetPortal();
        player.CloseLevelSelectMenu();

    }

    public void LevelButton(Level level)
    {
        if(level == null) return;
        if(level == summerLevel)
        {
            currentLevel = CurrentLevel.Summer;
        }else if(level == autumnLevel)
        {
            currentLevel = CurrentLevel.Autumn;
        }else if(level == winterLevel)
        {
            currentLevel = CurrentLevel.Winter;
        }else if(level == springLevel)
        {
            currentLevel = CurrentLevel.Spring;
        }else
        {
            Debug.LogWarning("Level not found");
        }

        // currentLevel = level;
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

    internal void PlayerHasBlockerUpgrade(Blocker blockerInRange)
    {
        for (int i = 0; i < blockerUpgrades.Count; i++)
        {
            if (blockerUpgrades[i] == null)
            {
                Debug.Log("Blocker upgrades is null, skipping");
                continue;
            }
            if (blockerInRange == null)
            {
                Debug.Log("Blocker is null, skipping");
                continue;
            }
            if(blockerInRange.GetUpdateRequired() == null)
            {
                Debug.Log("Blocker upgrade is null, skipping");
                continue;
            }
            //Debug.Log($"Blocker upgrade {i}: {blockerUpgrades[i]}");
            if (blockerUpgrades[i] == blockerInRange.GetUpdateRequired())
            {
                Debug.Log("Blocker upgrade unlocked!");
                DestroyLevelBlocker(blockerInRange);
            }
            else
            {
                Debug.Log("Blocker upgrade not unlocked!");
                blockerInRange.ShowFailMessage();
            }
        }
    }

    void DestroyLevelBlocker(Blocker blockerInRange)
    {
        GameObject tilemap = null;
        if(blockerInRange == blockerInRange.GetLevel().blocker2)
        {
            tilemap = blockerInRange.GetLevel().GetBlockerTilemap2();
            activeLevel.blocker2Cleared = true;
        }
        else
        {
            tilemap = blockerInRange.GetLevel().GetBlockerTilemap();
            activeLevel.blockerCleared = true;
        }
        
        
        if (tilemap == null)
        {
            Debug.Log("Blocker tilemap is null");
        }
        else
        {
            tilemap.SetActive(false);
        }
        if(blockerInRange == null)
        {
            Debug.Log("Blocker is null");
        }
        else
        {
            blockerInRange.RemoveBlocker();
        }
    }
}

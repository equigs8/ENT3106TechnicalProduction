using System;
using NUnit.Framework;
using UnityEngine;

public class Level : MonoBehaviour
{

    [Header("Level Settings")]
    public string levelName;
    public Transform spawnPoint;


    [Header("Level Objects")]
    public Blocker blocker;
    public GameObject blockerTilemap;
    
    public bool blockerCleared;
    public LevelKey levelKey;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GetBlockerStatus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void GetBlockerStatus()
    {
        //blockerCleared = blocker.blockerCleared;
    }

    public GameObject GetBlockerTilemap()
    {
        return blockerTilemap;
    }
}

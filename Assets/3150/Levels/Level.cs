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
    public Blocker blocker2;
    public GameObject blockerTilemap2;
    
    public bool blockerCleared;
    public LevelKey levelKey;
    internal bool blocker2Cleared;



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

    internal GameObject GetBlockerTilemap2()
    {
        return blockerTilemap2;
    }
}

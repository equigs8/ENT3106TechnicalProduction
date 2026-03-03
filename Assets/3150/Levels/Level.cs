using System;
using NUnit.Framework;
using UnityEngine;

public class Level : MonoBehaviour
{

    [Header("Level Settings")]
    public string levelName;


    [Header("Level Objects")]
    public Blocker blocker;
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
}

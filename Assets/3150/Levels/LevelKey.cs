using System;
using UnityEngine;

public class LevelKey : MonoBehaviour
{
    public bool slotted;
    internal void Collected()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

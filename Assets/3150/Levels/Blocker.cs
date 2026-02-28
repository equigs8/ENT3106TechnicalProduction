using System;
using UnityEngine;

public class Blocker : MonoBehaviour
{

    public BlockerUpgrade blockerUpgrade;

    internal BlockerUpgrade GetUpdateRequired()
    {
        return blockerUpgrade;
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

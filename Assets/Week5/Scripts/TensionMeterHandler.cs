using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System; // Required when Using UI elements.

public class TensionMeterHandler : MonoBehaviour
{
public Slider tensionMeter;

public float speed;
public bool meterStarted;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tensionMeter.value = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(meterStarted){
           tensionMeter.value += speed;
        }
        
    }

    internal float GetTensionMeterValue(){
        return tensionMeter.value;
    }

}

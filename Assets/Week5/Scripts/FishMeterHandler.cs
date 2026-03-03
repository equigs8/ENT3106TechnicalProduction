using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class FishMeterHandler : MonoBehaviour
{
    public Slider FishMeter;

    public float speed;
    public bool meterStarted;

    void Start()
    {


    }

    void Update()
    {
        if(meterStarted){
       FishMeter.value = Mathf.PingPong(Time.time*speed,100);
        }


    }

}

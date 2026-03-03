using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class FishMeterHandler : MonoBehaviour
{
    public Slider fishMeter;

    public float speed;
    public bool meterStarted;

    public GameObject winArea;

    private float meterHeight;

    public float winAreaMax;
    public float winAreaMin;

    float t;
    
    void Start()
    {
        meterHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;

        winAreaMax = Remap(winAreaMax, -100, 100, 0, meterHeight);
        winAreaMin = Remap(winAreaMin, -100, 100, 0, meterHeight);
        Debug.Log("Meter height: " + meterHeight);
        Debug.Log("Win area max: " + winAreaMax);
        Debug.Log("Win area min: " + winAreaMin);
        winArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,winAreaMax);
        winArea.GetComponent<RectTransform>().sizeDelta = new Vector2(100,winAreaMax - winAreaMin);
        

    }

    void Update()
    {
        if(meterStarted){
            t = Mathf.PingPong(Time.time*speed,1);
            fishMeter.value = Mathf.Lerp(fishMeter.minValue, fishMeter.maxValue, t);
        }


    }

    public float Remap (float value, float inputLow, float inputHigh, float outputLow, float outputHigh)
    {
        return outputLow + (value - inputLow) * (outputHigh - outputLow) / (inputHigh - inputLow);
    }


}

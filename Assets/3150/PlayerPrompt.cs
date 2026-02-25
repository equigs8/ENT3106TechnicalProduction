using UnityEngine;

public class PlayerPrompt : MonoBehaviour
{

    public Collider2D inRangeCollider;
    public GameObject prompt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayerInRange(bool inRange)
    {
        if(inRange)
        {
            prompt.SetActive(true);
        }
        else
        {
            prompt.SetActive(false);
        }
    }
}

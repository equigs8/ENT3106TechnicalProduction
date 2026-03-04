using UnityEngine;

public class PlayerPrompt : MonoBehaviour
{

    public Collider2D inRangeCollider;
    public GameObject prompt;
    public bool showPrompt;
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
            showPrompt = true;
        }
        else
        {
            prompt.SetActive(false);
            showPrompt = false;
        }
    }

    public bool GetShowPrompt()
    {
        return showPrompt;
    }
}

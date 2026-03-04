using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Sprite item;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        icon = GetComponent<Image>();
        item = icon.sprite;
        
        if (item != null)
        {
            icon.color = new Color(255, 255, 255, 255);
        }else
        {
            icon.color = new Color(255, 255, 255, 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    public List<Item> ItemsInSlot;

    public TMP_Text itemCountText;
    public Image itemImage;

 
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ItemsInSlot.Count > 0)
        {
            itemCountText.text = ItemsInSlot.Count.ToString();
            itemImage.sprite = ItemsInSlot[0].UIIcon;
            itemImage.color = new Color(255, 255, 255, 1);
        }
        else
        {
            itemCountText.text = "";
            itemImage.sprite = null;
            itemImage.color = new Color(255, 255, 255, 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotItem : MonoBehaviour
{
    public List<Item> items;


    void FixedUpdate()
    {
        if(items.Count > 0) 
            gameObject.transform.Find("InventorySlotItemAmount").GetComponent<TMP_Text>().text = items.Count.ToString();
    }
}

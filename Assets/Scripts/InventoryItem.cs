using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public string itemName;

    public string internalName;

    public Sprite UIIcon;

    public int maxStackSize;

    public int stackCount;

    public GameObject itemPrefab;
}

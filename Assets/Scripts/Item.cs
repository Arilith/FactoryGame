﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{

    public string itemName;

    public string internalName;

    public float hardness;

    public float smeltingTime;
    public float crushingTime;
    public float fuelBurnTime;

    public bool canBeSmelted;
    public bool canBeCrushed;
    public bool canBeFuel;

    public Sprite UIIcon;

    public GameObject BakedItem;
    public GameObject CrushedItem;

    public int crushMultiplier;

    public int maxStackSize;

    public int stackCount;

    public GameObject itemPrefab;

    void Start()
    {
        fuelBurnTime = hardness / 2f;
        smeltingTime = hardness / 10f;
        crushingTime = hardness / 5f;
    }

    // void Update()
    // {
    //     if(stackCount == 0)
    //         Destroy(gameObject);
    // }
}

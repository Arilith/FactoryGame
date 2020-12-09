using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{

    public string itemName;
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


    void Start()
    {
        fuelBurnTime = hardness / 2f;
        smeltingTime = hardness / 10f;
        crushingTime = hardness / 5f;
    }

}

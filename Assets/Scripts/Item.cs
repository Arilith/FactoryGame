using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public string itemName;
    public float hardness;

    public float smeltingTime;

    public float crushingTime;

    public bool canBeSmelted;
    public bool canBeCrushed;

    public Texture UIIcon;

    public GameObject BakedItem;
    public GameObject CrushedItem;

    public int crushMultiplier;

    void Start()
    {
        
        smeltingTime = hardness / 10f;
        crushingTime = hardness / 5f;
    }

}

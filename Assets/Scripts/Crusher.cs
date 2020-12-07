using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crusher : MonoBehaviour
{

    public List<GameObject> inputItems;
    public List<GameObject> outputItems;

    [SerializeField] private bool isCrushing;

    [SerializeField] private bool hasStarted;

    [SerializeField] private bool isLooking;

    public AudioSource machineSound;
    public AudioClip machineStartClip;
    public AudioClip machineWorkingClip;
    public AudioClip machineStopClip;

    public GameObject environment;

    [SerializeField] private int inputItemsLength;

    // Start is called before the first frame update
    void Start()
    {

        machineSound = GetComponent<AudioSource>();
        machineStartClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceStart");
        machineWorkingClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceWork");
        machineStopClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceStop");

    }

    // Update is called once per frame
    void Update()
    {
        //Stacksize in een global manager?
        if (inputItems.Count < 100 && !isLooking)
        {
            isLooking = true;
            StartCoroutine(LookForItems());
        }
        else if (inputItems.Count > 100)
        {
            isLooking = false;
        }


        //Check if machine is smelting items.
        if (!hasStarted)
        {

            //Check if there are items to be smelted
            if (inputItems.Count > 0)
            {
                hasStarted = true;

                //Play starting sound
                machineSound.PlayOneShot(machineStartClip);

                //Start smelting the items.
                StartCoroutine(CrushItems());
            }
        }
    }

    public IEnumerator LookForItems()
    {
        while (true)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.GetChild(1).transform.position, transform.GetChild(1).transform.forward, out hit))
            {
                if (hit.transform.gameObject.GetComponent<Item>())
                {
                    if (hit.transform.gameObject.GetComponent<Item>().canBeCrushed && inputItems.Count > 0)
                    {
                        if (hit.transform.gameObject.GetComponent<Item>().itemName == inputItems[0].GetComponent<Item>().itemName)
                        {
                            inputItems.Add(hit.transform.gameObject);
                            inputItemsLength++;
                            hit.transform.gameObject.SetActive(false);
                        }
                    } else if (hit.transform.gameObject.GetComponent<Item>().canBeCrushed)
                    {
                        inputItems.Add(hit.transform.gameObject);
                        inputItemsLength++;
                        hit.transform.gameObject.SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(2f);
        }
    }


    //Function to smelt the items
    public IEnumerator CrushItems()
    {
        //Wait for the machine to "Start up"
        yield return new WaitForSeconds(7f);

        //Make so the sound doesn't stop playing while working
        machineSound.loop = true;
        machineSound.PlayOneShot(machineWorkingClip);

        //Set smelting to true.
        isCrushing = true;

        //Go through all the items in the input
        inputItemsLength = inputItems.Count;
        int bakedItems = 0;

        for (int i = 0; i < inputItemsLength; i++)
        {
            //-bakedItems so the index isn't out of range
            GameObject go = inputItems[i - bakedItems];
            Item item = go.GetComponent<Item>();
            //Get the smeltingtime
            float crushingTime = item.crushingTime;

            //Wait the amount of seconds defined in the items properties.
            yield return new WaitForSeconds(crushingTime);

            //Remove from current
            inputItems.Remove(go);

            //Destroy the item so it doesn't take up memory.
            Destroy(go);

            //For putting out multiple items when the multiplier is set.
            for (int j = 0; j < item.crushMultiplier; j++)
            {
                //Add to output
                outputItems.Add(item.GetComponent<Item>().CrushedItem);
            }


            //Add to the baked items so the index won't be out of range
            bakedItems++;

            StartCoroutine(ShootItemsOut());
        }

        //Stop playing oven sound
        machineSound.loop = false;
        machineSound.Stop();
        machineSound.PlayOneShot(machineStopClip);

        yield return new WaitForSeconds(5f);

        //Set smelting and started to false
        isCrushing = false;
        hasStarted = false;
    }


    public IEnumerator ShootItemsOut()
    {

        //Loop trough output items
        foreach (GameObject item in outputItems.ToList())
        {
            if (outputItems.Count != 0)
            {
                //Instantiate new item at output
                GameObject newItem = Instantiate(item, transform.GetChild(0).GetComponent<Transform>().position, gameObject.transform.rotation);
                //Add rigidbody and add force to swing it outwards
                Rigidbody rb = newItem.GetComponent<Rigidbody>();
                rb.AddForce(newItem.transform.forward * 1000f);

                //Remove it from the list
                outputItems.Remove(item);

                yield return new WaitForSeconds(2f);
            }
            
        }

        yield return null;
    }


}

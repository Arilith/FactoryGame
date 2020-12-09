using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Furnace : MonoBehaviour
{

    public Machine machine;

    [SerializeField] private bool isSmelting;

    [SerializeField] private bool hasStarted;

    [SerializeField] private bool isLooking;

    public AudioSource machineSound;
    public AudioClip machineStartClip;
    public AudioClip machineWorkingClip;
    public AudioClip machineStopClip;

    public GameObject environment;

    [SerializeField] private int inputItemsLength;

    public UI ui;

    // Start is called before the first frame update
    void Start()
    {

        machine = GetComponent<Machine>();

        machineSound = GetComponent<AudioSource>();
        machineStartClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceStart");
        machineWorkingClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceWork");
        machineStopClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceStop");

        ui = GetComponent<UI>();
    }

    public IEnumerator ShootItemsOut()
    {

        //Loop trough output items
        foreach (GameObject item in machine.outputItems.ToList())
        {
            //Instantiate new item at output
            GameObject newItem = Instantiate(item, transform.GetChild(0).GetComponent<Transform>().position, gameObject.transform.rotation);
            //Add rigidbody and add force to swing it outwards
            Rigidbody rb = newItem.GetComponent<Rigidbody>();
            rb.AddForce(newItem.transform.forward * 1000f);

            //Remove it from the list
            machine.outputItems.Remove(item);

            ui.UpdateUI();

            yield return new WaitForSeconds(2f);
        }

        yield return null;
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
                    ui.UpdateUI();

                    if (hit.transform.gameObject.GetComponent<Item>().canBeSmelted && machine.inputItems.Count > 0)
                    {
                        if (hit.transform.gameObject.GetComponent<Item>().itemName == machine.inputItems[0].GetComponent<Item>().itemName)
                        {
                            machine.inputItems.Add(hit.transform.gameObject);
                            inputItemsLength++;
                            hit.transform.gameObject.SetActive(false);
                        }
                    }
                    else if (hit.transform.gameObject.GetComponent<Item>().canBeSmelted)
                    {
                        machine.inputItems.Add(hit.transform.gameObject);
                        inputItemsLength++;
                        hit.transform.gameObject.SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(2f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Stacksize in een global manager?
        if (machine.inputItems.Count < 100 && !isLooking)
        {
            isLooking = true;
            StartCoroutine(LookForItems());
        }
        else if (machine.inputItems.Count > 100)
        {
            isLooking = false;
        }


        //Check if machine is smelting items.
        if (!hasStarted)
        {

            //Check if there are items to be smelted
            if (machine.inputItems.Count > 0)
            {
                hasStarted = true;

                //Play starting sound
                machineSound.PlayOneShot(machineStartClip);

                //Start smelting the items.
                StartCoroutine(SmeltItems());
            }
        }
    }

    //Function to smelt the items
    public IEnumerator SmeltItems()
    {
        //Wait for the machine to "Start up"
        yield return new WaitForSeconds(7f);

        //Make so the sound doesn't stop playing while working
        machineSound.loop = true;
        machineSound.PlayOneShot(machineWorkingClip);

        //Set smelting to true.
        isSmelting = true;

        //Go through all the items in the input
        inputItemsLength = machine.inputItems.Count;
        int bakedItems = 0;

        for (int i = 0; i < inputItemsLength; i++)
        {
            //-bakedItems so the index isn't out of range
            GameObject item = machine.inputItems[i-bakedItems];

            //Get the smeltingtime
            float smeltingTime = item.GetComponent<Item>().smeltingTime;

            StartCoroutine(ui.AnimateSliderOverTime(smeltingTime));

            //Wait the amount of seconds defined in the items properties.
            yield return new WaitForSeconds(smeltingTime);

            //Remove from current
            machine.inputItems.Remove(item);

            //Destroy the item so it doesn't take up memory.
            Destroy(item);
            
            //Add to output
            machine.outputItems.Add(item.GetComponent<Item>().BakedItem);

            //Add to the baked items so the index won't be out of range
            bakedItems++;

            //UpdateUI
            GetComponent<UI>().UpdateUI();

            //StartCoroutine(ShootItemsOut());
        }
        
        //Stop playing oven sound
        machineSound.loop = false;
        machineSound.Stop();
        machineSound.PlayOneShot(machineStopClip);

        //Reset progress slider value
        if (GetComponent<UI>().isOpen == true)
        {
            ui.progressSlider.value = 0;
        }

        yield return new WaitForSeconds(5f);

        //Set smelting and started to false
        isSmelting = false;
        hasStarted = false;

        inputItemsLength = 0;
    }


    


}

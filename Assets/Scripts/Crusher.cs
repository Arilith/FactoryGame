using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Crusher : MonoBehaviour
{


    [SerializeField] private bool isCrushing;

    [SerializeField] private bool hasStarted;

    [SerializeField] private bool isLooking;

    public AudioSource machineSound;

    public AudioClip machineStartClip;
    public AudioClip machineWorkingClip;
    public AudioClip machineStopClip;
    public AudioClip powerDownClip;

    public GameObject environment;

    public Machine machine;

    [SerializeField] private int inputItemsLength;

    public UI ui;

    public float scanningTime;
    public float shutdownTime;
    public float startupTime;
    public float spittingTime;

    public float efficiency;

    public PowerUsage PowerManager;

    // Start is called before the first frame update
    void Start()
    {

        machineSound = GetComponent<AudioSource>();
        machineStartClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceStart");
        machineWorkingClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceWork");
        machineStopClip = Resources.Load<AudioClip>("Sounds/Furnace/FurnaceStop");

        powerDownClip = Resources.Load<AudioClip>("Sounds/General/powerout");

        machine = GetComponent<Machine>();

        ui = GetComponent<UI>();
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
            if (machine.inputItems.Count > 0 && PowerManager.hasPower)
            {
                efficiency = PowerManager.recievedPower / PowerManager.standardPowerUsage;

                PowerManager.powerUsage = PowerManager.standardPowerUsage * efficiency;

                hasStarted = true;

                //Play starting sound
                machineSound.PlayOneShot(machineStartClip);

                //Start smelting the items.
                StartCoroutine(CrushItems());
            }
        }

        
    }

    void FixedUpdate()
    {
        if (PowerManager.hasPower)
        {
            if (isCrushing)
            {
                efficiency = PowerManager.recievedPower / PowerManager.standardPowerUsage;

                PowerManager.powerUsage = PowerManager.standardPowerUsage * efficiency;

            }
        }
        else
        {
            efficiency = 0;
            PowerManager.powerUsage = 0;
        }


        ui.SetEfficiency(efficiency);
        ui.SetPowerDraw(PowerManager.powerUsage);
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

                    if (hit.transform.gameObject.GetComponent<Item>().canBeCrushed && machine.inputItems.Count > 0)
                    {
                        if (hit.transform.gameObject.GetComponent<Item>().itemName == machine.inputItems[0].GetComponent<Item>().itemName)
                        {
                            machine.inputItems.Add(hit.transform.gameObject);
                            inputItemsLength++;
                            hit.transform.gameObject.SetActive(false);
                        }
                    } else if (hit.transform.gameObject.GetComponent<Item>().canBeCrushed)
                    {
                        machine.inputItems.Add(hit.transform.gameObject);
                        inputItemsLength++;
                        hit.transform.gameObject.SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(scanningTime);
        }
    }


    //Function to smelt the items
    public IEnumerator CrushItems()
    {
        //Wait for the machine to "Start up"
        yield return new WaitForSeconds(startupTime);

        //Make so the sound doesn't stop playing while working
        machineSound.loop = true;
        machineSound.PlayOneShot(machineWorkingClip);

        //Set smelting to true.
        isCrushing = true;

        //Go through all the items in the input
        inputItemsLength = machine.inputItems.Count;
        int bakedItems = 0;
        if (PowerManager.hasPower)
        {
            for (int i = 0; i < inputItemsLength; i++)
            {
                if (PowerManager.hasPower)
                {
                    //-bakedItems so the index isn't out of range
                    GameObject go = machine.inputItems[i - bakedItems];
                    Item item = go.GetComponent<Item>();
                    //Get the smeltingtime
                    float crushingTime = item.crushingTime / efficiency;

                    StartCoroutine(ui.AnimateSliderOverTime(crushingTime));

                    
                    //Wait the amount of seconds defined in the items properties.
                    yield return new WaitForSeconds(crushingTime);

                    //Remove from current
                    machine.inputItems.Remove(go);

                    //Destroy the item so it doesn't take up memory.
                    Destroy(go);

                    //For putting out multiple items when the multiplier is set.
                    for (int j = 0; j < item.crushMultiplier; j++)
                    {
                        //Add to output
                        machine.outputItems.Add(item.GetComponent<Item>().CrushedItem);
                    }


                    //Add to the baked items so the index won't be out of range
                    bakedItems++;

                    //UpdateUI
                    GetComponent<UI>().UpdateUI();

                    StartCoroutine(ShootItemsOut());
                } else if (hasStarted == true && !PowerManager.hasPower)
                {
                    StartCoroutine(Shutdown("power"));
                }
               
            }
        }
        else
        {
            StartCoroutine(Shutdown("power"));
        }
        

        StartCoroutine(Shutdown("normal"));

    }


    public IEnumerator ShootItemsOut()
    {

        //Loop trough output items
        foreach (GameObject item in machine.outputItems.ToList())
        {
            if (machine.outputItems.Count != 0)
            {
                //Instantiate new item at output
                GameObject newItem = Instantiate(item, transform.GetChild(0).GetComponent<Transform>().position, gameObject.transform.rotation);
                //Add rigidbody and add force to swing it outwards
                Rigidbody rb = newItem.GetComponent<Rigidbody>();
                rb.AddForce(newItem.transform.forward * 1000f);

                //Remove it from the list
                machine.outputItems.Remove(item);

                ui.UpdateUI();

                yield return new WaitForSeconds(spittingTime);
            }
            
        }

        yield return null;
    }

    public IEnumerator Shutdown(string type)
    {
        //Stop playing oven sound
        machineSound.loop = false;
        machineSound.Stop();
        if (type == "power")
        {
            machineSound.PlayOneShot(powerDownClip);
        }
        else
        {
            machineSound.PlayOneShot(machineStopClip);
        }
        

        //Reset progress slider value
        if (GetComponent<UI>().isOpen == true)
        {
            ui.progressSlider.value = 0;
        }

        PowerManager.powerUsage = 0;
        efficiency = 0;

        yield return new WaitForSeconds(shutdownTime);

        //Set smelting and started to false
        isCrushing = false;
        hasStarted = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityStandardAssets.Characters.FirstPerson;
using Cursor = UnityEngine.Cursor;

public class PlayerUI : MonoBehaviour,  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public bool isUiOpen;

    public bool isInventoryOpen;

    public bool isDraggingObject;

    public bool isMouseOverUi;

    public bool isLookingAtPickupItem;

    public GameObject inventoryUI;
    public GameObject ClickedInventorySlot;

    public GameObject ClickedInventoryObject;

    public GameObject inventorySlotHolder;

    public GameObject inactiveObjectsHolder;

    public TMP_Text pickupText;

    RaycastHit hit;
    
    // Start is called before the first frame update
    void Start()
    {
        pickupText = GameObject.Find("PickupText").GetComponent<TMP_Text>();
        pickupText.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2") && !isUiOpen)
        {
            OpenMachineUI();
        }

        if (Input.GetKeyDown(KeyCode.E) && !isInventoryOpen && !isUiOpen && !isLookingAtPickupItem)
        {
            OpenInventory();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isUiOpen)
        {
            CloseUI();
        }

        if (isDraggingObject)
        {
            ClickedInventoryObject.transform.position = Input.mousePosition;
        }


        if (Input.GetButtonDown("Fire1") && isDraggingObject && !isMouseOverUi)
        {
            DropItem(ClickedInventoryObject);
        }

        if (Input.GetKeyDown(KeyCode.E) && !isInventoryOpen && isLookingAtPickupItem)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.transform.tag == "PickupItem")
                {
                    PickUpItem(hit.transform.gameObject);
                }
            }
        }
    }

    void FixedUpdate()
    {
        CheckIfLookingAtItem();
    }

    public void PickUpItem(GameObject go)
    {
        LookForFreeSlot(go.GetComponent<Item>(), go.GetComponent<Item>().stackCount);

    }

    public void LookForFreeSlot(Item item, int amountOfitems)
    {
        GameObject foundSlot = null;

        GameObject inventorySlots = inventorySlotHolder;

        int amountOfSlots = inventorySlots.transform.childCount;

        for (int i = 0; i < amountOfSlots; i++)
        {
            InventorySlot currentInventorySlot = inventorySlots.transform.GetChild(i).GetComponent<InventorySlot>();
            
            int amountOfItemsInSlot = currentInventorySlot.ItemsInSlot.Count;

            if (amountOfItemsInSlot > 0)
            {
                string itemsInSlotName = currentInventorySlot.ItemsInSlot[0].internalName;
                //Check if slot has same object
                if (itemsInSlotName == item.internalName)
                {
                    if (amountOfItemsInSlot != item.maxStackSize)
                    {
                        int allowedItemsInSlot = item.maxStackSize - amountOfItemsInSlot;
                        if (item.stackCount <= allowedItemsInSlot)
                        {
                           //Add Items to slot
                            for (int j = 0; j < item.stackCount; j++)
                            {
                                //Can cause problems when item is removed. (Should be fixed due itemprefab) 
                                foundSlot = currentInventorySlot.gameObject;

                                Item itemToAdd = currentInventorySlot.ItemsInSlot[0];
                                
                                currentInventorySlot.ItemsInSlot.Add(itemToAdd);
                                
                            }

                            Destroy(item.gameObject);
                            break;
                        }
                        else
                        {
                            //Stackcount is groter dan allowed items.

                            //Add allowed items to stack.
                            

                            Item itemToAdd = currentInventorySlot.ItemsInSlot[0];

                            for (int m = 0; m < allowedItemsInSlot; m++)
                            {
                                currentInventorySlot.ItemsInSlot.Add(itemToAdd);
                            }

                            item.stackCount -= allowedItemsInSlot;

                            Debug.Log("Added " + allowedItemsInSlot + "to slot. Left: " + item.stackCount + ". Looking for new slot.");
                            
                        }
                    }
                }
            }
        }
        if (foundSlot == null)
        {
            for (int k = 0; k < amountOfSlots; k++)
            {
                InventorySlot currentInventorySlot = inventorySlots.transform.GetChild(k).GetComponent<InventorySlot>();

                int amountOfItemsInSlot = currentInventorySlot.ItemsInSlot.Count;

                Item itemToAdd = Resources.Load<GameObject>("Prefabs/Items/" + item.internalName).GetComponent<Item>();

                if (amountOfItemsInSlot == 0)
                {
                    for (int l = 0; l < item.stackCount; l++)
                    {
                        currentInventorySlot.ItemsInSlot.Add(itemToAdd);
                    }

                    Destroy(item.gameObject);
                    break;
                }

            }
        }
    }



    public void CheckIfLookingAtItem()
    {
        if (!isUiOpen)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.transform.tag == "PickupItem")
                {
                    isLookingAtPickupItem = true;

                    pickupText.text = "Press E to pick up \n " + hit.transform.GetComponent<Item>().itemName;
                    pickupText.gameObject.SetActive(true);

                }
            }
            else
            {
                pickupText.gameObject.SetActive(false);
                isLookingAtPickupItem = false;
            }
        }
    }

    public void CloseUI()
    {
        if (hit.transform != null && isUiOpen && !isInventoryOpen)
        {
            hit.transform.GetComponent<UI>().CloseUI();
        }

        if (isInventoryOpen)
        {
            inventoryUI.SetActive(false);
            isInventoryOpen = false;
            gameObject.GetComponent<FirstPersonController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        isUiOpen = false;
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;
        isUiOpen = true;
        gameObject.GetComponent<FirstPersonController>().enabled = false;
        inventoryUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void OpenMachineUI()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.transform.GetComponent<UI>() && !isUiOpen)
            {
                isUiOpen = true;
                hit.transform.GetComponent<UI>().EnableUI();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickedObject.transform.tag == "ItemSlot")
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                ClickedOnInventorySlot(clickedObject);
            if (eventData.button == PointerEventData.InputButton.Right)
                RightClickedOnInventorySlot(clickedObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverUi = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverUi = false;
    }

    public void RightClickedOnInventorySlot(GameObject clickedObject)
    {

        if (isDraggingObject)
        {
            ClickedInventorySlot = clickedObject;
            List<Item> ClickedInventorySlotList = ClickedInventorySlot.GetComponent<InventorySlot>().ItemsInSlot;
            SlotItem ClickedInventoryObjectItems = ClickedInventoryObject.GetComponent<SlotItem>();
            int amountOfItems = ClickedInventoryObjectItems.items.Count;

            if (ClickedInventorySlotList.Count > 0)
            {
                if (ClickedInventorySlotList[0].internalName == ClickedInventoryObjectItems.items[0].internalName)
                {
                    ClickedInventorySlotList.Add(ClickedInventoryObjectItems.items[amountOfItems - 1]);
                    ClickedInventoryObjectItems.items.RemoveAt(amountOfItems - 1);

                    if (ClickedInventoryObjectItems.items.Count == 0)
                    {
                        Destroy(ClickedInventoryObjectItems.gameObject);
                        isDraggingObject = false;
                    }

                }
            }

            if (ClickedInventorySlotList.Count == 0)
            {
                ClickedInventorySlotList.Add(ClickedInventoryObjectItems.items[amountOfItems - 1]);
                ClickedInventoryObjectItems.items.RemoveAt(amountOfItems - 1);

                if (ClickedInventoryObjectItems.items.Count == 0)
                {
                    Destroy(ClickedInventoryObjectItems.gameObject);
                    isDraggingObject = false;
                }

            }

        }

        if (!isDraggingObject)
        {
            ClickedInventorySlot = clickedObject;
            List<Item> ClickedInventorySlotList = ClickedInventorySlot.GetComponent<InventorySlot>().ItemsInSlot;
            if (ClickedInventorySlotList.Count > 0 && ClickedInventorySlotList.Count != 1)
            {
                //Copy items in clicked slot to cursor
                ClickedInventoryObject = Instantiate(ClickedInventorySlot.transform.GetChild(0).gameObject, Input.mousePosition, Quaternion.identity, gameObject.transform.Find("PlayerUI"));
                ClickedInventoryObject.tag = "ItemUIShouldBeDestroyed";
                
                List<Item> itemsToAdd = ClickedInventorySlotList.GetRange(0, ClickedInventorySlotList.Count / 2);

                ClickedInventoryObject.GetComponent<SlotItem>().items.AddRange(itemsToAdd);

                //Clear the items in that slot.
                int amountOfItemsToRemove = Mathf.RoundToInt(ClickedInventorySlotList.Count / 2);

                for (int i = amountOfItemsToRemove; i > 0; i--)
                {
                    ClickedInventorySlotList.RemoveAt(i);
                }
                isDraggingObject = true;
            }
        }

        
    }

    public void ClickedOnInventorySlot(GameObject clickedObject)
    {
        ClickedInventorySlot = clickedObject;

        List<Item> ClickedInventorySlotList = ClickedInventorySlot.GetComponent<InventorySlot>().ItemsInSlot;

        if (!isDraggingObject)
        {
            if (ClickedInventorySlotList.Count > 0)
            {
                //Copy items in clicked slot to cursor
                ClickedInventoryObject = Instantiate(ClickedInventorySlot.transform.GetChild(0).gameObject, Input.mousePosition, Quaternion.identity, gameObject.transform.Find("PlayerUI"));
                ClickedInventoryObject.tag = "ItemUIShouldBeDestroyed";

                ClickedInventoryObject.GetComponent<SlotItem>().items.AddRange(ClickedInventorySlotList);
                //Clear the items in that slot.
                ClickedInventorySlot.GetComponent<InventorySlot>().ItemsInSlot.Clear();
                isDraggingObject = true;
            }
        }
        else
        {
            //Check if slot contains items, and has none. 
            if (ClickedInventorySlotList.Count == 0)
            {
               
                //Add holding items to slot
                ClickedInventorySlot.GetComponent<InventorySlot>().ItemsInSlot.AddRange(ClickedInventoryObject.GetComponent<SlotItem>().items);
                ClearHoldingItem();

            } 
            //Check if clicked slot has items that youre holding.
            else if (ClickedInventorySlotList[0].internalName ==
                     ClickedInventoryObject.GetComponent<SlotItem>().items[0].internalName)
            {
                
                int totalItems = ClickedInventorySlotList.Count +
                                 ClickedInventoryObject.GetComponent<SlotItem>().items.Count;

                if (totalItems > ClickedInventorySlotList[0].maxStackSize)
                {
                    //Deduct difference from stack and keep rest.

                    int allowedItems = ClickedInventorySlotList[0].maxStackSize - ClickedInventorySlotList.Count;
                       //Take the allowed items from the list
                       List<Item> itemsTake = ClickedInventoryObject.GetComponent<SlotItem>().items
                           .GetRange(0, allowedItems);
                       //Add them to the inventory slot
                    ClickedInventorySlot.GetComponent<InventorySlot>().ItemsInSlot.AddRange(itemsTake);
                    //Remove them from the holding list
                    ClickedInventoryObject.GetComponent<SlotItem>().items.RemoveRange(0, allowedItems);

                }
                else
                {
                    //Add both together and stop holding.
                    ClickedInventorySlot.GetComponent<InventorySlot>().ItemsInSlot.AddRange(ClickedInventoryObject.GetComponent<SlotItem>().items);
                    ClearHoldingItem();
                }
            }
        }
    }

    public void ClearHoldingItem()
    {
        isDraggingObject = false;
        ClickedInventorySlot = null;
        ClickedInventoryObject = null;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("ItemUIShouldBeDestroyed"))
        {
            Destroy(go);
        }

    }

    public void DropItem(GameObject itemToDrop)
    {
        GameObject environment = GameObject.FindGameObjectWithTag("Environment");
        GameObject droppedGameObject = Instantiate(itemToDrop.GetComponent<SlotItem>().items[0].itemPrefab, gameObject.transform.position, Quaternion.identity, environment.transform);
        droppedGameObject.GetComponent<Item>().stackCount = itemToDrop.GetComponent<SlotItem>().items.Count;

        ClearHoldingItem();

    }

}

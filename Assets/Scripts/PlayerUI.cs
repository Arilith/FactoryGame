using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerUI : MonoBehaviour,  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public bool isUiOpen;

    public bool isInventoryOpen;

    public bool isDraggingObject;

    public bool isMouseOverUi;

    public GameObject inventoryUI;
    public GameObject ClickedInventorySlot;

    public GameObject ClickedInventoryObject;

    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2") && !isUiOpen)
        {
            OpenMachineUI();
        }

        if (Input.GetKeyDown(KeyCode.E) && !isInventoryOpen && !isUiOpen)
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
            Debug.Log("Clicked outside UI.");
        }

    }

    public void CloseUI()
    {
        if (hit.transform != null)
        {
            hit.transform.GetComponent<UI>().CloseUI();
        }

        if (isInventoryOpen)
        {
            inventoryUI.SetActive(false);
            isInventoryOpen = false;
        }

        isUiOpen = false;
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;
        isUiOpen = true;

        inventoryUI.SetActive(true);
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
        Debug.Log("Clicked: " + clickedObject.name);

        if (clickedObject.transform.tag == "ItemSlot")
        {
            ClickedOnInventorySlot(clickedObject);
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

        Debug.Log("Clearing items");

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("ItemUIShouldBeDestroyed"))
        {
            Debug.Log(go.name);
            Destroy(go);
        }

    }

    public void DropItem()
    {

    }

}

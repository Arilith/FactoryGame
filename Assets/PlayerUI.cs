using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerUI : MonoBehaviour,  IPointerClickHandler
{

    public bool isUiOpen;

    public bool isInventoryOpen;

    public GameObject inventoryUI;

    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2"))
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

        if (Input.GetKeyDown(KeyCode.E) && !isInventoryOpen && !isUiOpen)
        {
            isInventoryOpen = true;
            isUiOpen = true;

            inventoryUI.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isUiOpen)
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

        

        
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

}

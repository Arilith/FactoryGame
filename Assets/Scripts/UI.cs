using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class UI : MonoBehaviour
{
    public GameObject uiCanvas;

    public GameObject ui;

    private GameObject instantiatedUI;

    private Sprite inputTexture;
    private Sprite outputTexture;

    private TMP_Text inputCountText;
    private TMP_Text outputCountText;

    private GameObject currentObject;

    private bool isOpen;
    void Start()
    {
        uiCanvas = GameObject.Find("UI");
    }

    public void EnableUI()
    {
  
        instantiatedUI = Instantiate(ui, uiCanvas.transform);

        inputTexture = gameObject.GetComponent<Machine>().inputItems[0].GetComponent<Item>().UIIcon;

        //Set the icon to the icon from the current object in the list
        GameObject.Find("InputImage").GetComponent<Image>().sprite = inputTexture;
        GameObject.Find("OutputImage").GetComponent<Image>().sprite = outputTexture;

        inputCountText = GameObject.Find("InputAmount").GetComponent<TMP_Text>();
        outputCountText = GameObject.Find("OutputAmount").GetComponent<TMP_Text>();


        inputCountText.text = gameObject.GetComponent<Machine>().inputItems.Count.ToString();
        outputCountText.text = gameObject.GetComponent<Machine>().outputItems.Count.ToString();

        isOpen = true;
        //inputTexture = instantiatedUI.transform.GetChild(1).transform.GetChild(0).GetComponent<Texture>();

        //Debug.Log("Opening UI");
    }

    public void UpdateUI()
    {

        if(isOpen)
        {
            List<GameObject> inputItems = gameObject.GetComponent<Machine>().inputItems;
            List<GameObject> outputItems = gameObject.GetComponent<Machine>().outputItems;

            inputCountText.text = inputItems.Count.ToString();
            outputCountText.text = outputItems.Count.ToString();

            if(inputItems.Count > 0)
            {
                GameObject.Find("InputImage").GetComponent<Image>().sprite = inputTexture;
            } else
            {
                inputCountText.text = "";
                GameObject.Find("InputImage").GetComponent<Image>().sprite = null;
            }

            if(outputItems.Count > 0)
            {
                GameObject.Find("OutputImage").GetComponent<Image>().sprite = outputTexture;
            } else
            {
                outputCountText.text = "";
                GameObject.Find("OutputImage").GetComponent<Image>().sprite = null;
            }

        }
        
    }

    public void Update()
    {
        //if(clickedObject.)
    }

    public void CloseUI()
    {
        isOpen = false;
        Debug.Log("Closing UI");
        Destroy(instantiatedUI);
        instantiatedUI = null;
        
    }

}

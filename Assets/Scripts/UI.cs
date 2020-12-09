using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

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

    private List<GameObject> inputItems;
    private List<GameObject> outputItems;

    public FirstPersonController fpController;

    public Slider progressSlider;

    public bool isOpen;
    void Start()
    {
        uiCanvas = GameObject.Find("UI");
        fpController = GameObject.FindObjectOfType<FirstPersonController>();



    }

    public void EnableUI()
    {
        //Disable player controller
        fpController.enabled = false;

        instantiatedUI = Instantiate(ui, uiCanvas.transform);

        inputItems = gameObject.GetComponent<Machine>().inputItems;
        outputItems = gameObject.GetComponent<Machine>().outputItems;

        inputCountText = GameObject.Find("InputAmount").GetComponent<TMP_Text>();
        outputCountText = GameObject.Find("OutputAmount").GetComponent<TMP_Text>();

        if (inputItems.Count > 0)
        {
            inputCountText.text = gameObject.GetComponent<Machine>().inputItems.Count.ToString();

            inputTexture = inputItems[0].GetComponent<Item>().UIIcon;

            GameObject.Find("InputImage").GetComponent<Image>().sprite = inputTexture;
            GameObject.Find("InputImage").GetComponent<Image>().color = new Color(255, 255, 255, 1);

            GameObject.Find("InputItem").GetComponent<TMP_Text>().text = inputItems[0].GetComponent<Item>().itemName;
        }
        else
        {
            GameObject.Find("InputImage").GetComponent<Image>().color = new Color(255, 255, 255, 0);
            GameObject.Find("InputItem").GetComponent<TMP_Text>().text = "Insert Item";
        }

        if (outputItems.Count > 0)
        {
            outputCountText.text = gameObject.GetComponent<Machine>().outputItems.Count.ToString();

            outputTexture = outputItems[0].GetComponent<Item>().UIIcon;
            GameObject.Find("OutputImage").GetComponent<Image>().sprite = outputTexture;
            GameObject.Find("OutputImage").GetComponent<Image>().color = new Color(255, 255, 255, 1);

            GameObject.Find("OutputItem").GetComponent<TMP_Text>().text = outputItems[0].GetComponent<Item>().itemName;

        }
        else
        {
            GameObject.Find("OutputImage").GetComponent<Image>().color = new Color(255, 255, 255, 0);
            GameObject.Find("OutputItem").GetComponent<TMP_Text>().text = "Insert Item";
        }
            

        //Set the icon to the icon from the current object in the list
        GameObject.Find("InputImage").GetComponent<Image>().sprite = inputTexture;
        GameObject.Find("OutputImage").GetComponent<Image>().sprite = outputTexture;

        
        isOpen = true;
        //inputTexture = instantiatedUI.transform.GetChild(1).transform.GetChild(0).GetComponent<Texture>();

        //Debug.Log("Opening UI");
    }

    public void UpdateUI()
    {
        if(isOpen)
        {
            inputItems = gameObject.GetComponent<Machine>().inputItems;
            outputItems = gameObject.GetComponent<Machine>().outputItems;

            inputCountText.text = inputItems.Count.ToString();
            outputCountText.text = outputItems.Count.ToString();
            
            if (inputItems.Count > 0)
            {
                inputTexture = inputItems[0].GetComponent<Item>().UIIcon;

                
                GameObject.Find("InputImage").GetComponent<Image>().sprite = inputTexture;
                GameObject.Find("InputImage").GetComponent<Image>().color = new Color(255, 255, 255, 1);
                
                GameObject.Find("InputItem").GetComponent<TMP_Text>().text = inputItems[0].GetComponent<Item>().itemName;
            } else
            {
                inputCountText.text = "";

                
                GameObject.Find("InputImage").GetComponent<Image>().sprite = null;
                GameObject.Find("InputImage").GetComponent<Image>().color = new Color(255, 255, 255, 0);
                GameObject.Find("InputItem").GetComponent<TMP_Text>().text = "Insert Item";
            }

            if(outputItems.Count > 0)
            {
                outputTexture = outputItems[0].GetComponent<Item>().UIIcon;

                GameObject.Find("OutputImage").GetComponent<Image>().sprite = outputTexture;
                GameObject.Find("OutputImage").GetComponent<Image>().color = new Color(255, 255, 255, 1);
                
                GameObject.Find("OutputItem").GetComponent<TMP_Text>().text = outputItems[0].GetComponent<Item>().itemName;
            } else
            {
                outputCountText.text = "";
                
                GameObject.Find("OutputImage").GetComponent<Image>().sprite = null;
                GameObject.Find("OutputImage").GetComponent<Image>().color = new Color(255, 255, 255, 0);
                GameObject.Find("OutputItem").GetComponent<TMP_Text>().text = "Insert Item";
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
        fpController.enabled = true;
    }

    public IEnumerator AnimateSliderOverTime(float seconds)
    {
        if (isOpen)
        {
            progressSlider = GameObject.Find("ProgressSlider").GetComponent<Slider>();
            
            float animationTime = 0f;
            while (animationTime < seconds)
            {
                animationTime += Time.deltaTime;
                float lerpValue = animationTime / seconds;
                progressSlider.value = Mathf.Lerp(0f, 1f, lerpValue);
                yield return null;
            }
        }
    }

}

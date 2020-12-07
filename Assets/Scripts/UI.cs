using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject uiCanvas;

    public GameObject ui;

    private GameObject instantiatedUI;

    private Texture inputTexture;
    private Texture outputTexture;

    private GameObject currentObject;

    void Start()
    {
        uiCanvas = GameObject.Find("UI");
    }

    public void EnableUI()
    {
  
        instantiatedUI = Instantiate(ui, uiCanvas.transform);

       // inputTexture = gameObject.GetComponent<Machine>()
        //inputTexture = instantiatedUI.transform.GetChild(1).transform.GetChild(0).GetComponent<Texture>();

        //Debug.Log("Opening UI");
    }


    public void Update()
    {
        //if(clickedObject.)
    }

    public void CloseUI()
    {
        Debug.Log("Closing UI");
        Destroy(instantiatedUI);
        instantiatedUI = null;
    }

}

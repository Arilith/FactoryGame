using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrigger : MonoBehaviour
{
    private bool isUiOpen;

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

        if (Input.GetKeyDown(KeyCode.Escape) && isUiOpen)
        {
            hit.transform.GetComponent<UI>().CloseUI();
            isUiOpen = false;
        }
    }
}


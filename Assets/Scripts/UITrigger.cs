using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrigger : MonoBehaviour
{
    private bool isUIopen;

    RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform.GetComponent<UI>() && !isUIopen)
                {
                    isUIopen = true;
                    hit.transform.GetComponent<UI>().EnableUI();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isUIopen)
        {
            hit.transform.GetComponent<UI>().CloseUI();
            isUIopen = false;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerExplosion : MonoBehaviour
{

    public GameObject explosion;
    public PlayerUI playerUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1") && !playerUI.isUiOpen) {
             RaycastHit hit;

            if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
                Instantiate(explosion, hit.point, Quaternion.identity);
            }
        }
    }
}

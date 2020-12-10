using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalPlant : MonoBehaviour
{
    public float DefaultPowerProduction;
    public float PowerProduction;

    public float efficiency;

    // Start is called before the first frame update
    void Start()
    {
        DefaultPowerProduction = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        PowerProduction = DefaultPowerProduction * 0.5f;
    }
}

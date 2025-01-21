using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondSensorIncar : MonoBehaviour
{
    public bool OtherCarDetected;
    public bool CrossingDetected;
    
    
    // Start is called before the first frame update
    void Start()
    {
        OtherCarDetected = false;
        CrossingDetected = false;   
    }

    // Update is called once per frame
    void Update()
    {

        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag =="Car")
        {
            OtherCarDetected = true;
        }

        if(other.gameObject.tag == "Crosser")
        {
           

           if(other.gameObject.GetComponent<DoesCrossingHavePedestrian>().IsTherePedestrian)
            {
                CrossingDetected = true;
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            OtherCarDetected = false;
        }

        if (other.gameObject.tag == "Crosser")
        {
            if (!other.gameObject.GetComponent<DoesCrossingHavePedestrian>().IsTherePedestrian)
            {
                CrossingDetected = false;
            }
        }
    }

    
    }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoesCrossingHavePedestrian : MonoBehaviour
{
    public bool IsTherePedestrian;
    public bool CarCanMove;
    // Start is called before the first frame update
    void Start()
    {
        IsTherePedestrian = false;
        CarCanMove = true ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag== "Pedestrian")
        {
            IsTherePedestrian = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Pedestrian")
        {
            IsTherePedestrian = true;
            CarCanMove = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Pedestrian")
        {
            IsTherePedestrian = false;
        }
    }



}

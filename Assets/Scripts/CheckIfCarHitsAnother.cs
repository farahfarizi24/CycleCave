using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CheckIfCarHitsAnother : MonoBehaviour
{
    public CarMovement MoveScript;
    public SecondSensorIncar SecondSensor;
    public float TempFloat;
    public bool CrossingActivate = false;
    // Start is called before the first frame update
    void Start()
    {
        MoveScript = this.gameObject.GetComponent<CarMovement>();
        SecondSensor = this.gameObject.GetComponentInChildren<SecondSensorIncar>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            if (SecondSensor.OtherCarDetected)
            {
                TempFloat = MoveScript.speed;

                MoveScript.speed = other.gameObject.GetComponent<CarMovement>().speed;

            }
        }

        if (other.gameObject.tag == "Crosser")
        {
            if (SecondSensor.CrossingDetected&& !CrossingActivate)
            {
                TempFloat = MoveScript.speed;
                MoveScript.speed = 0;
                CrossingActivate = true;
                StartCoroutine(PedestrianTicker());
            }

        }
      
    }


    private IEnumerator PedestrianTicker()
    {
        yield return new WaitForSeconds(5.0f);
       CrossingActivate=false;
        MoveScript.speed=TempFloat;



    }

    private void OnTriggerExit(Collider other) 
    {

        if (other.gameObject.tag == "Car")
        {
            if (!SecondSensor.OtherCarDetected)
            {
                MoveScript.speed = TempFloat;
                

            }
        }


    }

}

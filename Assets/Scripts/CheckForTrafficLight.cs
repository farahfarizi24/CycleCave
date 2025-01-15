using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrafficLightController;

public class CheckForTrafficLight : MonoBehaviour
{
    public TrafficLightController trafficControl;
    public CarSimpleMove CarMoveScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if( trafficControl.CurrentState == SignalState.Green || trafficControl.CurrentState == SignalState.Yellow)
        {
            CarMoveScript.isGreenLight = true;
        }

       if (trafficControl.CurrentState == SignalState.Red)
        {
            CarMoveScript.isGreenLight = false;
        }

    }
}

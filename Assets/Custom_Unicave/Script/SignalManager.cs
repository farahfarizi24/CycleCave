using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager : MonoBehaviour
{
    [System.Serializable]
    public class TrafficSignal
    {
        public string signalName;
        public TrafficLightController trafficLightController;
    }

    public List<TrafficSignal> trafficSignals; // List of all traffic lights
    public float yellowLightDuration = 3f;

    private void Start()
    {
        if (trafficSignals.Count > 0)
        {
            StartCoroutine(ManageSignals());
        }
        else
        {
            Debug.LogWarning("No traffic signals assigned to the manager!");
        }
    }

    private IEnumerator ManageSignals()
    {
        while (true)
        {
            for (int i = 0; i < trafficSignals.Count; i++)
            {
                TrafficLightController currentController = trafficSignals[i].trafficLightController;

                // Turn on the green light for the current signal
                currentController.ActivateGreenLight();
                yield return new WaitForSeconds(currentController.greenLightDuration);

                // Transition to yellow light
                currentController.ActivateYellowLight();
                yield return new WaitForSeconds(yellowLightDuration);

                // Transition to red light
                currentController.ActivateRedLight();
                yield return new WaitForSeconds(1f); // Small buffer before the next signal turns green
            }
        }
    }
}

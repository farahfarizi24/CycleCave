using System.Collections;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public enum SignalState
    {
        Red,
        Yellow,
        Green
    }

    public SignalState CurrentState { get; private set; } = SignalState.Red;
    public CheckTrigger TriggerScript;
    public bool LightHasStarted;
    public GameObject greenLight;
    public GameObject yellowLight;
    public GameObject redLight;

    public float greenLightDuration = 10f; // Duration for the green light

    private void Start()
    {
        LightHasStarted = false;
        ActivateRedLight(); // Default to red light
    }
    private void Update()
    {
        if (TriggerScript.TriggerHit && LightHasStarted==false)
        {
            StartRedtoGreen();
            LightHasStarted=true;
        }
    }


    public void ActivateGreenLight()
    {
        SetAllLightsOff();
        greenLight.SetActive(true);
        CurrentState = SignalState.Green; // Update state
        Debug.Log($"{gameObject.name}: Green Light ON, State = {CurrentState}");
    }

    public void ActivateYellowLight()
    {
        SetAllLightsOff();
        yellowLight.SetActive(true);
        CurrentState = SignalState.Yellow; // Update state
        Debug.Log($"{gameObject.name}: Yellow Light ON, State = {CurrentState}");
    }

    public void ActivateRedLight()
    {
        SetAllLightsOff();
        redLight.SetActive(true);
        CurrentState = SignalState.Red; // Update state
        Debug.Log($"{gameObject.name}: Red Light ON, State = {CurrentState}");
    }

    private void SetAllLightsOff()
    {
        greenLight.SetActive(false);
        yellowLight.SetActive(false);
        redLight.SetActive(false);
    }

    public void StartRedtoGreen()
    {
        StartCoroutine(manageRedtoGreenLights());
    }

    public void StartGreentoRed()
    {
        StartCoroutine(manageGreentoRedLights());
    }

    private IEnumerator manageRedtoGreenLights()
    {
        ActivateRedLight();
        yield return new WaitForSeconds(5.0f);
        ActivateGreenLight();
        yield return new WaitForSeconds(10.0f);
        StartGreentoRed();
    }



    private IEnumerator manageGreentoRedLights()
    {
       
        ActivateGreenLight();
        yield return new WaitForSeconds(3.0f);
        ActivateYellowLight();
        yield return new WaitForSeconds(3.0f);
        ActivateRedLight();
        yield return new WaitForSeconds(5.0f);
        StartRedtoGreen();

    }


}

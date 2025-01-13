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

    public GameObject greenLight;
    public GameObject yellowLight;
    public GameObject redLight;

    public float greenLightDuration = 10f; // Duration for the green light

    private void Start()
    {
        ActivateRedLight(); // Default to red light
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
}

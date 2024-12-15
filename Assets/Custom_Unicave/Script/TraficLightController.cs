using System.Collections;
using UnityEngine;

public class TraficLightController : MonoBehaviour
{
    public GameObject greenLight;
    public GameObject yellowLight;
    public GameObject redLight;

    // Duration for each light in seconds
    public float greenLightDuration = 10f;
    public float yellowLightDuration = 3f;
    public float redLightDuration = 10f;

    private IEnumerator Start()
    {
        while (true)
        {
            // Green Light
            ActivateLight(greenLight);
            yield return new WaitForSeconds(greenLightDuration);

            // Yellow Light
            ActivateLight(yellowLight);
            yield return new WaitForSeconds(yellowLightDuration);

            // Red Light
            ActivateLight(redLight);
            yield return new WaitForSeconds(redLightDuration);
        }
    }

    private void ActivateLight(GameObject activeLight)
    {
        // Turn off all lights
        greenLight.SetActive(false);
        yellowLight.SetActive(false);
        redLight.SetActive(false);

        // Turn on the active light
        activeLight.SetActive(true);
    }
}

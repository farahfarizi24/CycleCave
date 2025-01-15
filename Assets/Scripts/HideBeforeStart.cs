using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideBeforeStart : MonoBehaviour
{

    public CheckTrigger trigger;
    public GameObject bodyContainer;
    // Start is called before the first frame update
    void Start()
    {
        bodyContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.TriggerHit)
        {
            bodyContainer.SetActive(true);
        }
    }
}

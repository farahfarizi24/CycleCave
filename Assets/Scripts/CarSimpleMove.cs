using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSimpleMove : MonoBehaviour
{
    public CheckTrigger startTrigger;
    public CheckTrigger endTrigger;
    public bool isGreenLight;
    public float speed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        isGreenLight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTrigger.TriggerHit || isGreenLight)
        {
            MoveObject();
        }

       
    }


    void MoveObject()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

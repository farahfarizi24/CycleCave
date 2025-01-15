using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    public bool isPlayerTrigger;
    public bool TriggerHit;
    // Start is called before the first frame update
    void Start()
    {
        TriggerHit =  false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player")
        { if (!TriggerHit)
            TriggerHit = true;
        }
        if (other.gameObject.tag == "Car" && !isPlayerTrigger)
        {
           Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Pedestrian")
        {
            Destroy(other.gameObject);

        }
    }
}

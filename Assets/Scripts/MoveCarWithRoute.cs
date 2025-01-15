using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoveCarWithRoute : MonoBehaviour
{
    [SerializeField]
    private Transform[] routes;
    private int curRouteTarget;
    public float speed;
    public bool moveOnRoute;
    public CheckTrigger trigger;
    // Start is called before the first frame update
    void Start()
    {
        curRouteTarget = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.TriggerHit == true)
        {
            if (moveOnRoute)
            {
                MoveObject(curRouteTarget);
            }
            else
            {
                MoveObjectNormally();
            }
        }

     
    }


    void MoveObject(int routeTarget)
    {
        Vector3 relativePos = routes[routeTarget].position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.time * speed);
        transform.position = Vector3.MoveTowards(transform.position, routes[routeTarget].position, speed*Time.deltaTime);
        if (Vector3.Distance(transform.position, routes[routeTarget].position) < 0.001f)
        {
            // Go to the next route
            curRouteTarget++;
            if (curRouteTarget == routes.Length) { moveOnRoute = false; }
        }

        

    }

    void MoveObjectNormally()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

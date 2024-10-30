using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{

    public enum LocomotionSystem { Translation, Rotation, Fly }
    public LocomotionSystem locomotion;
    public float rotationSpeed = 1.5f;
    public float translationSpeed = 1.0f;
    public GameObject laser;



    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        if (locomotion == LocomotionSystem.Translation)
        {
            Vector3 movement = new Vector3(moveHorizontal* translationSpeed, 0.0f, moveVertical* translationSpeed);
            Vector3 position = transform.position;
            transform.position = position + movement;
        }

        if(locomotion == LocomotionSystem.Rotation)
        {
          //  Vector3 movement = new Vector3(0.0f, 0.0f, moveVertical* translationSpeed);
           // Vector3 position = transform.position;
          //  transform.position = position + movement;
           // transform.Translate(moveVertical * translationSpeed * transform.forward);
           // transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), moveHorizontal * rotationSpeed);

            transform.position += (transform.forward * Input.GetAxis("Vertical")) * translationSpeed;
            transform.eulerAngles += new Vector3(0, Input.GetAxis("Horizontal"), 0);
        }

        if(locomotion == LocomotionSystem.Fly)
        {
            Vector3 movement = new Vector3(0.0f, 0.0f, moveVertical * translationSpeed);
            Vector3 position = transform.position;
            transform.position = position + movement;
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(laser.transform.forward);
            transform.Translate(moveVertical * translationSpeed * localForward);
            //transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), moveHorizontal * rotationSpeed);
            // transform.RotateAround(transform.position,new Vector3(0.0f, 1.0f, 0.0f), moveHorizontal * rotationSpeed);
            transform.position += (laser.transform.forward * Input.GetAxis("Vertical")) * translationSpeed;
            transform.eulerAngles += new Vector3(0, Input.GetAxis("Horizontal"), 0);
        }


    }
}

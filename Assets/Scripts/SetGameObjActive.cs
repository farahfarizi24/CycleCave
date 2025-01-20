using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SetGameObjActive : MonoBehaviour
{
    public CheckTrigger trigger;
    public GameObject[] GOList;
    public bool ObjectsAreOn;
    // Start is called before the first frame update
    void Start()
    {
        ObjectsAreOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.TriggerHit &&!ObjectsAreOn)
        {
            SetObjActive();
           
        }
    }


    void SetObjActive()
    {
       

            for (int i = 0; i < GOList.Length; i++)
            {
                GOList[i].gameObject.SetActive(true);
            }

            ObjectsAreOn = true;

        
    }
}

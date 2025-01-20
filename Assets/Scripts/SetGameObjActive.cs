using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SetGameObjActive : MonoBehaviour
{
    public CheckTrigger trigger;
    public GameObject[] GOList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.TriggerHit)
        {
            if (GOList.Length >= 1) {

                for (int i = 0; i < GOList.Length; i++)
                {
                    GOList[i].gameObject.SetActive(true);
                }
              
                
                Array.Clear(GOList, 0, GOList.Length);
                GOList = null;
                
            }
           
        }
    }
}

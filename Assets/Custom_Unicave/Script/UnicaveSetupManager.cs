using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicaveSetupManager : MonoBehaviour
{
   
    public static UnicaveSetupManager instance;

    public Animator animator;

    public Cameras cameras;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}

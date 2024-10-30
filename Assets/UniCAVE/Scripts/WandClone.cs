using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandClone : MonoBehaviour
{

	public GameObject Wand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Wand == null)
            return;

        transform.position = Wand.transform.position;
	transform.rotation = Wand.transform.rotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{

    Material m_Material;
    public GameObject Laser;
	public GameObject Wand;
    private bool inside = false;
    private Transform parent;
    private bool grab = false;

    // Start is called before the first frame update
    void Start()
    {

        BoxCollider bc = gameObject.GetComponent<BoxCollider>();

        if (bc == null)
        {
            BoxCollider bct = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
            bct.isTrigger = true;
        }
        else
        {
            bc.isTrigger = true;
        }

        m_Material = Laser.GetComponent<Renderer>().material;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")&&inside)
        {

            if (grab == false)
            {
                parent = transform.parent;
                transform.SetParent(Wand.transform);
                grab = true;
                Debug.Log("grab");
            }
            else
            {
                transform.SetParent(parent);
                grab = false;
                Debug.Log("ungrab");

            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        m_Material.color = Color.green;
        inside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        m_Material.color = Color.white;
        inside = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cameras : MonoBehaviourPunCallbacks
{
    public List<camera> cameras;
    public float cameraMoveSpeed = 5f;

    private void Update()
    {
        // Check if the player is the master client and allow camera movement
        if (PhotonNetwork.IsMasterClient)
        {
            MoveCamera();
        }
    }

    public void setCamera(Screen screen)
    {
        foreach (var item in cameras)
        {
            if (item.screen == screen)
            {
                item.Camera.SetActive(true);
            }
            else
            {
                item.Camera.SetActive(false);
            }
        }
    }

    public camera GetCamera(Screen screen)
    {
        foreach (var item in cameras)
        {
            if (item.screen == screen)
            {
                return item;
            }
        }
        return null;
    }

    private void MoveCamera()
    {
        // Move forward with 'W' and backward with 'S'
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * cameraMoveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * cameraMoveSpeed * Time.deltaTime;
        }

    }
}

[System.Serializable]
public class camera
{
    public Screen screen;
    public GameObject Camera;
}

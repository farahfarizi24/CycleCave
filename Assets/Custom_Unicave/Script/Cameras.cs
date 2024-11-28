using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cameras : MonoBehaviourPunCallbacks
{
    public List<camera> cameras;
    public float cameraMoveSpeed = 5f;
    public float cameraRotationSpeed = 5f;

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
        float moveDirection = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection = 1f; // Forward
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection = -1f; // Backward
        }

        // Apply forward/backward movement
        if (moveDirection != 0f)
        {
            transform.position += transform.forward * moveDirection * cameraMoveSpeed * Time.deltaTime;
        }

        // Steer left with 'A' and right with 'D'
        float turnDirection = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            turnDirection = -1f; // Turn left
        }
        else if (Input.GetKey(KeyCode.D))
        {
            turnDirection = 1f; // Turn right
        }

        // Apply rotation
        if (turnDirection != 0f)
        {
            transform.Rotate(0f, turnDirection * cameraRotationSpeed * Time.deltaTime * 100f, 0f); // Adjust the rotation speed as needed
        }
    }
}

[System.Serializable]
public class camera
{
    public Screen screen;
    public GameObject Camera;
}

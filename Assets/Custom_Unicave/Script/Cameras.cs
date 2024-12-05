using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cameras : MonoBehaviourPunCallbacks
{
    public List<camera> cameras; // List of cameras
    public float cameraMoveSpeed;// Speed of camera movement
    public float rotationSpeed = 2f; // Speed of rotation smoothing
    public Transform[] waypoints; // Waypoints for predefined path
    private int currentWaypointIndex = 0; // Current waypoint index
   
    private void Update()
    {
        // Check if the player is the master client and allow camera movement
        if (PhotonNetwork.IsMasterClient)
        {
            MoveAlongPath();
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

    private void MoveAlongPath()
    {
        // Move forward along the path with 'W'
        if (Input.GetKey(KeyCode.W))
        {
            cameraMoveSpeed = 5.0f;
            MoveCode();   
        }
    }

    public void MoveCode()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            // Move towards the current waypoint
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                cameraMoveSpeed * Time.deltaTime
            );

            // Smoothly rotate towards the waypoint, only updating the Y-axis
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Keep only Y rotation
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed // Smoother transition
                );
            }

            // Check if the object has reached the current waypoint
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
    }
}

[System.Serializable]
public class camera
{
    public Screen screen;
    public GameObject Camera;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;

public class Cameras : MonoBehaviourPunCallbacks
{
    public List<camera> cameras; // List of cameras
    public float cameraMoveSpeed = 5f; // Speed of camera movement
    public float rotationSpeed = 2f; // Speed of rotation smoothing
    public Transform[] waypoints; // Waypoints for predefined path
    private int currentWaypointIndex = 0; // Current waypoint index

    public static Cameras instance;
    public Action OnPathCompleted;

    Vector3 start_pos;
    Quaternion start_rot;

    // Timer variables
    private float timer;

    private void Awake()
    {
        start_pos = transform.position;
        start_rot = transform.rotation;
        ResetTimer(); // Initialize the timer
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateTimer();
           // Move();
        }
    }

    private void UpdateTimer()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                Manager.instance.FinishScene(); // Call finish scene if timer runs out
            }
        }
    }

    public void Move()
    {
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
        if (!Manager.instance.movementDisabled)
        {

            if ( waypoints[currentWaypointIndex].position!=null && currentWaypointIndex <= waypoints.Length - 1 && transform.position!=null)
            {
                Vector3 targetPosition =  waypoints[currentWaypointIndex].position;


                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    cameraMoveSpeed * Time.deltaTime
                );

                Vector3 direction = (targetPosition - transform.position).normalized;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = waypoints[currentWaypointIndex].rotation;
                    targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    currentWaypointIndex++;

                    if (currentWaypointIndex >= waypoints.Length)
                    {
                        Manager.instance.FinishScene();
                    }
                }
            }
        }


    }

    public void ResetCamera()
    {
        // Reset waypoint index
        currentWaypointIndex = 0;

        // Re-initialize waypoints
        var waypoint = GameObject.FindGameObjectWithTag("waypoint");
        List<Transform> temp = new List<Transform>();
        foreach (Transform item in waypoint.transform)
        {
            temp.Add(item);
        }
        waypoints = temp.ToArray();

        // Reset camera position and rotation
        transform.position = waypoints[0].position;
        transform.rotation = waypoints[0].rotation;


        // Reset the timer
        ResetTimer();
    }

    private void ResetTimer()
    {
        if (SceneManager.GetActiveScene().name == "Scene 1")
        {
            timer = 60f; // Reset time to 60 seconds for Scene 1
        }
        else
        {
            timer = 120f; // Reset time to 120 seconds for other scenes
        }
    }
}

[System.Serializable]
public class camera
{
    public Screen screen;
    public GameObject Camera;
}

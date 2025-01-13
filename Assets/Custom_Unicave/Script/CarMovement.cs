using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float detectionRange = 10f;      // Range to detect the player
    public float speed = 5f;                // Car movement speed
    public string playerTag = "Player";     // Tag to identify the player

    public Transform frontLeftWheel;        // Wheel references
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    public float wheelRotationSpeed = 360f;
    public TrafficLightController trafficSignal; // Reference to traffic signal

    public Transform stopLine;             // Position of the stop line
    public float stopDistance = 2f;        // Distance to maintain behind a car

    private Transform player;              // Player reference
    private bool isActivated = false;
    private bool isStopped = false;

    private void Start()
    {
        // Find the player in the scene by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Please ensure the player has the correct tag.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isActivated && distance <= detectionRange)
        {
            isActivated = true;
            Debug.Log("Player in range, car starts moving.");
        }

        if (isActivated)
        {
            HandleTrafficSignal();
            HandleCarCollisions();

            if (!isStopped)
            {
                MoveCar();
                RotateWheels();
            }

            if (distance > detectionRange)
            {
                Destroy(gameObject);
            }
        }
    }

    private void HandleTrafficSignal()
    {
        if (trafficSignal != null)
        {
            // Stop if the signal is red and the car is near the stop line
            if (trafficSignal.CurrentState == TrafficLightController.SignalState.Red)
            {
                float stopLineDistance = Vector3.Distance(transform.position, stopLine.position);
                if (stopLineDistance <= stopDistance)
                {
                    isStopped = true;
                   // Debug.Log($"{gameObject.name} stopped at the stop line.");
                }
            }
            else if (trafficSignal.CurrentState == TrafficLightController.SignalState.Green)
            {
                isStopped = false; // Start moving when the signal is green
            }
        }
    }

    private void HandleCarCollisions()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, stopDistance * 2f))
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Car"))
            {
                CarMovement otherCar = hit.collider.GetComponent<CarMovement>();
                if (otherCar != null && otherCar.isStopped)
                {
                    isStopped = true;
                    Debug.Log($"{gameObject.name} stopped behind another car.");
                    return;
                }
            }
        }
    }

    private void MoveCar()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void RotateWheels()
    {
        float rotationAmount = wheelRotationSpeed * Time.deltaTime;

        if (frontLeftWheel != null)
            frontLeftWheel.Rotate(Vector3.right, rotationAmount);

        if (frontRightWheel != null)
            frontRightWheel.Rotate(Vector3.right, rotationAmount);

        if (rearLeftWheel != null)
            rearLeftWheel.Rotate(Vector3.right, rotationAmount);

        if (rearRightWheel != null)
            rearRightWheel.Rotate(Vector3.right, rotationAmount);
    }

    private void OnDrawGizmos()
    {
        // Debug Gizmo to show stopping line detection
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * stopDistance * 2f);
    }
}

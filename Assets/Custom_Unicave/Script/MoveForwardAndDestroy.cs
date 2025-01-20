using UnityEngine;

public class MoveWhenPlayerNearAndDestroy : MonoBehaviour
{
    float speed = 2f;                // Movement speed
    float lifetime = 40f;           // Lifetime after movement starts (in seconds)
    float detectionRange = 50f;      // Range to detect the player
    public string playerTag = "Player";     // Tag to identify the player
    Animator animator;               // Reference to the Animator component

    private Transform player;               // Reference to the player's transform
    private bool isMoving = false;          // Flag to check if the GameObject is moving
    private bool destroyTimerStarted = false; // Ensure the timer starts only once

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

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
            Debug.LogError("Player not found! Ensure the player has the correct tag.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Check the distance to the player
        float distance = Vector3.Distance(transform.position, player.position);

        if (!isMoving && distance <= detectionRange)
        {
            StartMoving();
        }

        if (isMoving)
        {
            // Move the GameObject forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void StartMoving()
    {
        isMoving = true;

        // Activate the walking animation
        if (animator != null)
        {
            //animator.SetBool("isWalking", true);
        }

        Debug.Log("Player is near, object starts moving.");

        // Start the destruction timer (only once)
        if (!destroyTimerStarted)
        {
            destroyTimerStarted = true;
            Invoke(nameof(DestroyObject), lifetime);
        }
    }

    private void DestroyObject()
    {
        Debug.Log("Object lifetime ended, destroying object.");
        Destroy(gameObject);
    }
}

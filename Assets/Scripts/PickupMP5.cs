using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MP5Pickup : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private InputActionReference pickupAction; // Reference to the pickup button action (Button West)

    // Adjustable variables for the MP5 pickup
    [SerializeField] private int ammo = 120; // Default ammo set to 120, adjustable in Inspector

    // Reference to the PlayerShooter script to pass the MP5 info
    private PlayerShooter playerShooter;

    // MP5 rotation and floating variables
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Save the initial position to use for floating effect
        startPosition = transform.position;

        // Get the PlayerShooter script from the player
        playerShooter = player.GetComponent<PlayerShooter>();

        if (playerShooter == null)
        {
            Debug.LogError("PlayerShooter script not found on player!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleFloatingAndRotation(); // Make the MP5 float and rotate

        if (IsPlayerClose() && pickupAction.action.triggered)
        {
            PickupMP5();
        }
    }

    // Check if the player is close enough to pick up the MP5
    bool IsPlayerClose()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        return distance <= 2.0f; // Adjust the pickup distance as needed
    }

    // Handles the floating and rotation of the MP5 in the air
    void HandleFloatingAndRotation()
    {
        // Rotate the MP5
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Make the MP5 float up and down
        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = newPosition;
    }

    // Called when the player picks up the MP5
    void PickupMP5()
    {
        // Replace the pistol with the MP5 in the PlayerShooter script
        playerShooter.hasPistol = false;  // Disable the pistol
        playerShooter.hasMP5 = true;      // Set MP5 as active
        playerShooter.mp5Ammo = ammo;     // Set the MP5 ammo based on the pickup
        playerShooter.currentMP5Ammo = playerShooter.mp5MagazineSize; // Refill the MP5 magazine

        // Hide the MP5 (or destroy it if you don't need it anymore)
        gameObject.SetActive(false);

        Debug.Log("MP5 picked up. Ammo: " + ammo);
    }
}

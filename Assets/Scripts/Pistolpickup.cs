using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PistolPickup : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private InputActionReference pickupAction; // Reference to the pickup button action (Button West)

    // Variables to set when the pistol is picked up
    public bool hasPistol = false;
    [SerializeField] private int ammo = 45; // Default ammo set to 45, adjustable in Inspector

    // Reference to the PlayerShooter script to pass the pistol info
    private PlayerShooter playerShooter;

    // Pistol rotation and floating variables
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
        HandleFloatingAndRotation(); // Make the pistol float and rotate

        if (IsPlayerClose() && pickupAction.action.triggered)
        {
            PickupPistol();
        }
    }

    // Check if the player is close enough to pick up the pistol
    bool IsPlayerClose()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        return distance <= 2.0f; // Adjust the pickup distance as needed
    }

    // Handles the floating and rotation of the pistol in the air
    void HandleFloatingAndRotation()
    {
        // Rotate the pistol
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Make the pistol float up and down
        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = newPosition;
    }

    // Called when the player picks up the pistol
    void PickupPistol()
    {
        // Set the hasPistol flag and ammo in the PlayerShooter script
        playerShooter.hasPistol = true;
        playerShooter.pistolAmmo = ammo; // Set the pistol ammo based on the pickup

        // Hide the pistol (or you can destroy it if you don't need it anymore)
        gameObject.SetActive(false);

        Debug.Log("Pistol picked up. Ammo: " + ammo);
    }
}

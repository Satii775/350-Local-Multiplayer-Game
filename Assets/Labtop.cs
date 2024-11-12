using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LabtopPickup : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private InputActionReference pickupAction; // Reference to the pickup button action (Button West)

    // Labtop rotation and floating variables
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Save the initial position to use for floating effect
        startPosition = transform.position;

        if (player == null)
        {
            Debug.LogError("Player object is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleFloatingAndRotation(); // Make the labtop float and rotate

        if (IsPlayerClose() && pickupAction.action.triggered)
        {
            PickupLabtop();
        }
    }

    // Check if the player is close enough to pick up the labtop
    bool IsPlayerClose()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        return distance <= 2.0f; // Adjust the pickup distance as needed
    }

    // Handles the floating and rotation of the labtop in the air
    void HandleFloatingAndRotation()
    {
        // Rotate the labtop
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Make the labtop float up and down
        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = newPosition;
    }

    // Called when the player picks up the labtop
    void PickupLabtop()
    {
        // Hide the labtop (or destroy it if you don’t need it anymore)
        gameObject.SetActive(false);

        // Load the next scene in the build settings
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);

        Debug.Log("Labtop picked up. Switching to next scene.");
    }
}

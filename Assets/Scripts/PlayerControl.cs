using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerCamera; // Reference to the player's camera for vertical rotation
    [SerializeField] private float movementSpeed = 5f; // Speed for movement
    [SerializeField] private float rotationSpeed = 200f; // Speed for rotation
    [SerializeField] private float lookUpDownSpeed = 100f; // Speed for looking up and down
    [SerializeField] private float jumpPower = 5f; // Power of the jump
    [SerializeField] private float jumpCooldown = 2f; // Cooldown before the player can jump again

    private float jumpTimer; // Tracks the cooldown timer
    private Vector3 movementDirection;
    public InputActionReference movement;
    public InputActionReference jumping;
    public InputActionReference look; // Reference for the right stick input

    private bool canJump = true; // Control whether the player can jump or not
    private Rigidbody rb; // Reference to the Rigidbody for applying forces
    private float cameraPitch = 0f; // Tracks the vertical rotation (pitch) of the camera

    void Start()
    {
        // Ensure the player has a Rigidbody component
        rb = player.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody component found on the player!");
        }

        if (playerCamera == null)
        {
            Debug.LogError("No Camera assigned to the player! Please assign a camera in the Inspector.");
        }
    }

    void Update()
    {
        PlayerMovement();

        // Update the jump timer
        if (!canJump)
        {
            jumpTimer -= Time.deltaTime; // Decrease the timer
            if (jumpTimer <= 0)
            {
                canJump = true; // Allow jumping again after cooldown
            }
        }

        // Handle jumping
        if (canJump)
        {
            PlayerJump();
        }
    }

    void PlayerMovement()
    {
        // Read movement input from the left stick
        Vector2 moveInput = movement.action.ReadValue<Vector2>();
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y); // X for strafing, Z for forward/backward

        // Apply movement relative to the player's current orientation
        Vector3 move = player.transform.TransformDirection(movementDirection) * movementSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        // Read aiming/looking input from the right stick
        Vector2 lookInput = look.action.ReadValue<Vector2>();

        // Handle horizontal rotation (yaw) with the player's body
        float turnAmount = lookInput.x * rotationSpeed * Time.deltaTime;
        player.transform.Rotate(Vector3.up, turnAmount);

        // Handle vertical rotation (pitch) with the camera
        float lookUpDownAmount = lookInput.y * lookUpDownSpeed * Time.deltaTime;
        cameraPitch -= lookUpDownAmount; // Invert the input for natural "FPS-like" control

        // Clamp the pitch to prevent the camera from flipping over (e.g., -90 to 90 degrees)
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        // Apply the pitch rotation to the camera
        playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    void PlayerJump()
    {
        // Check if the jump button was pressed
        if (jumping.action.triggered)
        {
            canJump = false; // Set canJump to false
            jumpTimer = jumpCooldown; // Reset the jump timer to the cooldown value

            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse); // Apply upward force to the Rigidbody for jumping
        }
    }
}

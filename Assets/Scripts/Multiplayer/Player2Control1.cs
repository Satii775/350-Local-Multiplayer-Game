using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Control1 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerCamera; // Reference to the player's camera for vertical rotation
    [SerializeField] private float movementSpeed = 5f; // Speed for movement
    [SerializeField] private float rotationSpeed = 200f; // Speed for rotation
    [SerializeField] private float lookUpDownSpeed = 100f; // Speed for looking up and down
    [SerializeField] private float jumpPower = 5f; // Power of the jump
    [SerializeField] private float jumpCooldown = 2f; // Cooldown before the player can jump again

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f; // Speed for dashing
    [SerializeField] private float dashDistance = 5f; // Distance for the dash
    [SerializeField] private float dashDuration = 0.2f; // Time taken to complete the dash
    [SerializeField] private float dashCooldown = 1f; // Cooldown between dashes

    private float jumpTimer; // Tracks the cooldown timer
    private bool canDoubleJump = false; // Controls if the player can perform a double jump
    private bool isDashing = false; // Tracks if the player is currently dashing
    private bool canDash = true; // Controls whether the player can dash (1-second cooldown)

    private Vector3 movementDirection;
    public InputActionReference movement;
    public InputActionReference jumping;
    public InputActionReference look; // Reference for the right stick input
    public InputActionReference slowMO; // Reference for the slow motion input
    public InputActionReference dashLeft; // Reference for left dash (LB)
    public InputActionReference dashRight; // Reference for right dash (RB)

    private bool canJump = true; // Control whether the player can jump or not
    private Rigidbody rb; // Reference to the Rigidbody for applying forces
    private float cameraPitch = 0f; // Tracks the vertical rotation (pitch) of the camera

    // Slow motion settings
    private int slowMoLevel = 0; // Tracks the current level of slow motion
    private bool isSlowMoActive = false;
    private bool canActivateSlowMo = true;

    void Start()
    {
        rb = player.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody component found on the player!");
        }

        if (playerCamera == null)
        {
            Debug.LogError("No Camera assigned to the player! Please assign a camera in the Inspector.");
        }

        Cursor.visible = false;
    }

    void Update()
    {
        PlayerMovement();

        // Update the jump timer
        if (!canJump)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0)
            {
                canJump = true;
            }
        }

        if (canJump || canDoubleJump)
        {
            PlayerJump();
        }

        // Check if slow motion button was pressed
        if (slowMO.action.triggered && canActivateSlowMo)
        {
            ActivateSlowMo();
        }

        // Handle dashing with cooldown and direction
        if (!isDashing && canDash)
        {
            if (dashLeft.action.triggered)
            {
                StartCoroutine(Dash(Vector3.left));
            }
            else if (dashRight.action.triggered)
            {
                StartCoroutine(Dash(Vector3.right));
            }
        }
    }

    void PlayerMovement()
    {
        if (isDashing) return; // Prevent regular movement while dashing

        Vector2 moveInput = movement.action.ReadValue<Vector2>();
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 move = player.transform.TransformDirection(movementDirection) * movementSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        Vector2 lookInput = look.action.ReadValue<Vector2>();

        float turnAmount = lookInput.x * rotationSpeed * Time.deltaTime;
        player.transform.Rotate(Vector3.up, turnAmount);

        float lookUpDownAmount = lookInput.y * lookUpDownSpeed * Time.deltaTime;
        cameraPitch -= lookUpDownAmount;

        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    void PlayerJump()
    {
        if (jumping.action.triggered)
        {
            if (canJump)
            {
                // Initial jump
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                canJump = false;
                canDoubleJump = true; // Enable double jump after first jump
                jumpTimer = jumpCooldown;
            }
            else if (canDoubleJump)
            {
                // Double jump
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                canDoubleJump = false; // Disable double jump after use
            }
        }
    }

    private IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;
        canDash = false; // Start dash cooldown

        float startTime = Time.time;
        Vector3 dashTarget = rb.position + player.transform.TransformDirection(direction) * dashDistance;
        Vector3 dashVelocity = (dashTarget - rb.position).normalized * dashSpeed;

        // Keep applying dash velocity for the duration, no deceleration in air
        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = dashVelocity; // Apply velocity in the dash direction
            yield return null;
        }

        rb.velocity = Vector3.zero; // Reset velocity after dashing
        isDashing = false;

        // Wait for the dash cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // Re-enable dashing
    }

    private void ActivateSlowMo()
    {
        slowMoLevel = (slowMoLevel + 1) % 4; // Cycle through levels: 0 -> 1 -> 2 -> 3 -> 0
        isSlowMoActive = true;

        // Apply the correct slow motion scale and duration based on the level
        switch (slowMoLevel)
        {
            case 1:
                Time.timeScale = 0.4f; // 40% speed
                StartCoroutine(SlowMoDuration(15f, 0.4f));
                break;
            case 2:
                Time.timeScale = 0.2f; // 20% speed
                StartCoroutine(SlowMoDuration(10f, 0.2f));
                break;
            case 3:
                Time.timeScale = 0.1f; // 10% speed
                StartCoroutine(SlowMoDuration(6f, 0.1f));
                break;
            default:
                // Reset to normal if it's the fourth press (level 0)
                Time.timeScale = 1f;
                isSlowMoActive = false;
                break;
        }

        // Set fixedDeltaTime to keep physics smooth during slow motion
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private IEnumerator SlowMoDuration(float duration, float slowMoScale)
    {
        canActivateSlowMo = false; // Disable reactivation until cooldown is over

        yield return new WaitForSecondsRealtime(duration); // Wait for the duration in real-world time

        // Reset time scale back to normal
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f; // Reset fixedDeltaTime
        isSlowMoActive = false;
        slowMoLevel = 0; // Reset slow motion level

        // Cooldown before reactivating slow motion
        yield return new WaitForSecondsRealtime(8f);
        canActivateSlowMo = true; // Re-enable slow motion activation
    }
}

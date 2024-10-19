using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Movements
	[SerializeField] private GameObject player;
	[SerializeField] private float movementSpeed;
	private Vector3 movementDirection;
    public InputActionReference movement;

    [SerializeField] private float rotationSpeed;
    private Vector3 lookDirection;
    public InputActionReference look;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float cameraSensitivity = 100f;
    private float xRotation = 0f;


    //Jump
    private bool isJumping = false;
    [SerializeField] private float jumpPower;
    private Vector3 jumpForce;
    public InputActionReference jumping;
	
    // Start is called before the first frame update
    void Start()
    {
        jumpForce = new Vector3(0, jumpPower, 0);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        PlayerLook();

        if (!isJumping) {
            PlayerJump();
        }
        else {
            if (player.transform.position.y <= 1) {
                isJumping = false;
            }
        }
    }
    
    private void PlayerMovement() {
        movementDirection = movement.action.ReadValue<Vector2>();
        player.transform.Translate(Vector3.forward * movementDirection.y * movementSpeed * Time.deltaTime);
        player.transform.Translate(Vector3.right * movementDirection.x * movementSpeed * Time.deltaTime);
    }

    private void PlayerLook() {
        lookDirection = look.action.ReadValue<Vector2>();

        // Get input values for X and Y axis (mouse or right stick)
        float mouseX = lookDirection.x * cameraSensitivity * Time.deltaTime;
        float mouseY = lookDirection.y * cameraSensitivity * Time.deltaTime;

        // Rotate the player around the Y-axis for horizontal camera movement
        player.transform.Rotate(Vector3.up * mouseX);

        // Adjust vertical look by rotating the camera, and clamp the rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamping to avoid over-rotation.

        // Apply rotation to the camera
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void PlayerJump() {
        if(jumping.action.triggered) {
            isJumping = true;
            player.GetComponent<Rigidbody>().AddForce(jumpForce, ForceMode.Impulse);
        }
    }
}

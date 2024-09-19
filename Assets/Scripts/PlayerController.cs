using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement properties
    [SerializeField] private GameObject player;
    [SerializeField] private float movementSpeed, rotationSpeed, verticalLookSensitivity;
    private Vector3 movementDirection;
    private Vector3 turnDirection;

    // Camera properties
    [SerializeField] private Transform playerCamera;
    private float verticalRotation = 0f;
    [SerializeField] private float verticalLookLimit = 80f;

    // Player 1 (Controller) inputs
    public InputActionReference player1Movement;
    public InputActionReference player1Turn;
    public InputActionReference player1Jumping;
    public InputActionReference player1Shooting;

    // Player 2 (Mouse/Keyboard) inputs
    public InputActionReference player2Movement;
    public InputActionReference player2Turn;
    public InputActionReference player2Jumping;
    public InputActionReference player2Shooting;

    // Jumping
    private bool isJumping = false;
    [SerializeField] private float jumpPower;
    private Vector3 jumpForce;

    // Gun handling
    [SerializeField] private GameObject gun;
    private bool hasGun = false;
    private int bullets = 0;

    // Shooting
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletLaunchPosition;
    private bool isShooting = false;

    [SerializeField] private int playerNumber = 1; // 1 = Controller, 2 = Mouse/Keyboard

    void Start()
    {
        jumpForce = new Vector3(0, jumpPower, 0);

        // Enable actions for Player 1
        player1Movement.action.Enable();
        player1Turn.action.Enable();
        player1Jumping.action.Enable();

        // Enable actions for Player 2
        player2Movement.action.Enable();
        player2Turn.action.Enable();
        player2Jumping.action.Enable();
    }

    void Update()
    {
        if (playerNumber == 1)
        {
            PlayerMovement(player1Movement);
            HandlePlayer1Look();
            HandleJump(player1Jumping);
            HandleShooting(player1Shooting);
        }
        else if (playerNumber == 2)
        {
            PlayerMovement(player2Movement);
            HandleMouseLook();
            HandleJump(player2Jumping);
            HandleShooting(player2Shooting);
        }
    }

    private void PlayerMovement(InputActionReference movement)
    {
        movementDirection = movement.action.ReadValue<Vector2>();
        player.transform.Translate(Vector3.forward * movementDirection.y * movementSpeed * Time.deltaTime);
        player.transform.Translate(Vector3.right * movementDirection.x * movementSpeed * Time.deltaTime);
    }

    private void HandlePlayer1Look()
    {
        turnDirection = player1Turn.action.ReadValue<Vector2>();
        player.transform.Rotate(Vector3.up * turnDirection.x * rotationSpeed * Time.deltaTime);
    }

    // Handle mouse (Player 2) look - horizontal and vertical
    private void HandleMouseLook()
    {
        turnDirection = player2Turn.action.ReadValue<Vector2>();

        // Horizontal turning (Y-axis) with mouse
        player.transform.Rotate(Vector3.up * turnDirection.x * rotationSpeed * Time.deltaTime);

        // Vertical turning (X-axis) with mouse
        verticalRotation -= turnDirection.y * verticalLookSensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit); // Clamp vertical rotation
        
        // Apply vertical rotation to the camera (up/down)
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleJump(InputActionReference jumping)
    {
        if (!isJumping)
        {
            if (jumping.action.triggered)
            {
                isJumping = true;
                player.GetComponent<Rigidbody>().AddForce(jumpForce, ForceMode.Impulse);
            }
        }
        else
        {
            if (player.transform.position.y <= 1)
            {
                isJumping = false;
            }
        }
    }

    private void HandleShooting(InputActionReference shooting)
    {
        if (!isShooting && hasGun && bullets > 0 && shooting.action.triggered)
        {
            ShootGun();
        }
    }

    public void GunPickedup()
    {
        if (hasGun)
        {
            bullets += 10;
        }
        else
        {
            bullets += 10;
            hasGun = true;
            gun.SetActive(true);
        }
    }

    private void ShootGun()
    {
        isShooting = true;
        bullets -= 1;

        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        bullet.SetActive(true);
        bullet.transform.position = bulletLaunchPosition.transform.position;
        bullet.transform.rotation = bulletLaunchPosition.transform.rotation;

        bullet.GetComponent<Rigidbody>().AddForce(bulletLaunchPosition.transform.up * 50, ForceMode.Impulse);

        StartCoroutine(ShootingPause());
    }

    IEnumerator ShootingPause()
    {
        yield return new WaitForSeconds(0.2f);
        isShooting = false;
    }
}

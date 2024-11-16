using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplitPlayerController : MonoBehaviour
{
   
    private bool gameStarted = false;
    private int playerNumber;

    private Vector3 jumpForce = new Vector3(0,5,0);
    private bool isJumping = false;

    private float moveSpeed = 5f;
    private Vector3 movementDirection;

    public InputActionReference movementP1;
    public InputActionReference movementP2;
    public InputActionAsset player2IAA;

    private float rotationSpeed = 100f;
    private Vector3 lookDirection;
    public InputActionReference lookP1;
    public InputActionReference lookP2;

    public void OnLook() {
        switch (playerNumber) {
            case 0:
                lookDirection = lookP1.action.ReadValue<Vector2>();
                break;
            case 1:
                lookDirection = lookP2.action.ReadValue<Vector2>();
                break;
        }
    }


    public void OnJump() {
        Debug.Log("Jumped");
        if (!gameStarted) {
            PlacePlayer();
        }
        else if (!isJumping) {
            isJumping = true;
            this.gameObject.GetComponent<Rigidbody>().AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    public void OnMove() {
        switch (playerNumber)
        {
            case 0:
                movementDirection = movementP1.action.ReadValue<Vector2>();
                break;
            case 1:
                movementDirection = movementP2.action.ReadValue<Vector2>();
                break;
        }
    }

    private void PlacePlayer() {

        Debug.Log("Test");
        var PM = GameObject.Find("GameManager");

        playerNumber = PM.GetComponent<PlayerManager>().CheckTotalPlayers();

        switch (playerNumber)
        {
            case 0:
                transform.position = new Vector3(-5,1,0);
                this.gameObject.name = "Player1";
                this.gameObject.GetComponent<Renderer>().material.color = Color.blue;
                //GameObject.Find("Body").GetComponent<RobotStateController>().SetPlayer(this.gameObject);
                break;
            
            case 1:
                transform.position = new Vector3( 5, 1, 0);
                this.gameObject.name = "Player2";
                this.gameObject.GetComponent<Renderer>().material.color = Color.red;
                this.gameObject.GetComponent<PlayerInput>().actions = player2IAA;
                break;
        }

        PM.GetComponent<PlayerManager>().AddPlayer();
        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Translate(Vector3.forward * movementDirection.y * moveSpeed * Time.deltaTime);
        this.gameObject.transform.Translate(Vector3.right * movementDirection.x * moveSpeed * Time.deltaTime);

        this.gameObject.transform.Rotate(Vector3.up * lookDirection.x * rotationSpeed * Time.deltaTime);
        this.gameObject.transform.Rotate(Vector3.right * lookDirection.z * rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision) {
        if (isJumping) {
            if (collision.collider.name == "Floor") {
                isJumping = false;
            }
        }
    }
}

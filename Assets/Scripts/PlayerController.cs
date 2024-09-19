using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Movements
	[SerializeField] private GameObject player;
	[SerializeField] private float movementSpeed, rotationSpeed;
	private Vector3 movementDirection;
    public InputActionReference movement;

    //Jump
    private bool isJumping = false;
    [SerializeField] private float jumpPower;
    private Vector3 jumpForce;
    public InputActionReference jumping;

    //Gun
    [SerializeField] private GameObject gun;
    private bool hasGun = false;
    private int bullets = 0;

    //Shooting
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletLaunchPosition;
    private bool isShooting = false;
    public InputActionReference shooting;
	
    // Start is called before the first frame update
    void Start()
    {
        jumpForce = new Vector3(0, jumpPower, 0);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        if (!isJumping) {
            PlayerJump();
        }
        else {
            if (player.transform.position.y <= 1) {
                isJumping = false;
            }
        }

        if(!isShooting && hasGun && bullets > 0 && shooting.action.triggered) {
            ShootGun();
        }
    }
    
    private void PlayerMovement() {
        movementDirection = movement.action.ReadValue<Vector2>();
        player.transform.Translate(Vector3.forward * movementDirection.y * movementSpeed * Time.deltaTime);
        player.transform.Rotate(Vector3.up * movementDirection.x * rotationSpeed * Time.deltaTime);
    }

    private void PlayerJump() {
        if(jumping.action.triggered) {
            isJumping = true;
            player.GetComponent<Rigidbody>().AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    public void GunPickedup() {
        if(hasGun) {
            bullets += 10;
        }
        else {
            bullets += 10;
            hasGun = true;
            gun.SetActive(true);
        }
    }

    private void ShootGun() {
        isShooting = true;

        bullets -= 1;

        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        bullet.SetActive(true);
        bullet.transform.position = bulletLaunchPosition.transform.position;
        bullet.transform.rotation = bulletLaunchPosition.transform.rotation;

        bullet.GetComponent<Rigidbody>().AddForce(bulletLaunchPosition.transform.up * 50, ForceMode.Impulse);

        StartCoroutine(ShootingPause());
    }

    IEnumerator ShootingPause() {
        yield return new WaitForSeconds(0.2f);
        isShooting = false;
    }
}

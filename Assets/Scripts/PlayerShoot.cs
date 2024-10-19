using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float zoomedFOV = 20f;
    public float zoomSpeed = 5f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 365.76f; // 1200 fps converted to meters per second
    public float projectileGravity = 9.81f; // Realistic gravity for bullet drop

    [Header("Pistol Settings")]
    public bool hasPistol = false;           // Do you have a pistol?
    public int pistolAmmo = 45;              // Total ammo for the pistol
    private int currentPistolAmmo = 15;      // Ammo in the current magazine
    public int pistolMagazineSize = 15;      // Pistol holds 15 bullets per magazine
    private float pistolFireRate = 1f;       // Fire rate of the pistol (semi-auto, 1 shot per trigger)
    private float pistolReloadTime = 2f;     // Time it takes to reload the pistol
    private bool isPistolReloading = false;  // Is the pistol reloading?

    [Header("MP5 Settings")]
    public bool hasMP5 = false;              // Do you have an MP5?
    public int mp5Ammo = 120;                // Total ammo for the MP5
    public int currentMP5Ammo = 30;      // Ammo in the current MP5 magazine
    public int mp5MagazineSize = 30;         // MP5 holds 30 bullets per magazine
    private float mp5FireRate = 950f / 60f;  // MP5 fires at 950 rounds per minute (~15.83 shots per second)
    private float mp5ReloadTime = 2f;        // Time it takes to reload the MP5
    private bool isMP5Reloading = false;     // Is the MP5 reloading?
    private float nextTimeToFire = 0f;       // Time until the next shot can be fired (for full-auto MP5)

    private PlayerInput playerInput;
    private InputAction aimAction;
    private InputAction shootAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Use the action map and action names as per your input asset setup
        aimAction = playerInput.actions["Aim"];   // Left Trigger
        shootAction = playerInput.actions["Shoot"]; // Right Trigger
    }

    void Update()
    {
        HandleZoom();
        UpdateFirePoint();
        HandleShooting();
    }

    void HandleZoom()
    {
        if (aimAction.ReadValue<float>() > 0.1f)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomedFOV, zoomSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, zoomSpeed * Time.deltaTime);
        }
    }

    void UpdateFirePoint()
    {
        // Ensure the firePoint is at the camera's position and pointing in the same direction as the camera
        firePoint.position = playerCamera.transform.position + playerCamera.transform.forward * 1.0f;
        firePoint.rotation = playerCamera.transform.rotation;
    }

    void HandleShooting()
    {
        if (hasPistol && !isPistolReloading)
        {
            HandlePistolShooting();
        }
        else if (hasMP5 && !isMP5Reloading)
        {
            HandleMP5Shooting();
        }
    }

    void HandlePistolShooting()
    {
        // Semi-auto pistol: Fire only when the trigger is pulled
        if (shootAction.triggered && currentPistolAmmo > 0)
        {
            ShootPistol();
        }

        // Reload when magazine is empty
        if (currentPistolAmmo == 0 && !isPistolReloading)
        {
            StartCoroutine(ReloadPistol());
        }
    }

    void HandleMP5Shooting()
    {
        // Full-auto MP5: Continuously fire while holding the trigger
        if (shootAction.ReadValue<float>() > 0 && Time.time >= nextTimeToFire && currentMP5Ammo > 0)
        {
            nextTimeToFire = Time.time + 1f / mp5FireRate;
            ShootMP5();
        }

        // Reload when magazine is empty
        if (currentMP5Ammo == 0 && !isMP5Reloading)
        {
            StartCoroutine(ReloadMP5());
        }
    }

    // Shooting logic for the pistol
    void ShootPistol()
    {
        // Instantiate the projectile and set its velocity
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * projectileSpeed;
        rb.useGravity = true;

        // Decrease current magazine ammo
        currentPistolAmmo--;

        Debug.Log("Pistol shot fired! Ammo left: " + currentPistolAmmo);
    }

    // Shooting logic for the MP5
    void ShootMP5()
    {
        // Instantiate the projectile and set its velocity
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * projectileSpeed;
        rb.useGravity = true;

        // Decrease current magazine ammo
        currentMP5Ammo--;

        Debug.Log("MP5 shot fired! Ammo left: " + currentMP5Ammo);
    }

    // Reloading logic for the pistol
    IEnumerator ReloadPistol()
    {
        isPistolReloading = true;
        Debug.Log("Reloading pistol...");

        yield return new WaitForSeconds(pistolReloadTime);

        // Refill the pistol magazine
        int ammoToReload = Mathf.Min(pistolMagazineSize, pistolAmmo);
        currentPistolAmmo = ammoToReload;
        pistolAmmo -= ammoToReload;

        isPistolReloading = false;
        Debug.Log("Pistol reloaded! Ammo in magazine: " + currentPistolAmmo + " | Remaining ammo: " + pistolAmmo);
    }

    // Reloading logic for the MP5
    IEnumerator ReloadMP5()
    {
        isMP5Reloading = true;
        Debug.Log("Reloading MP5...");

        yield return new WaitForSeconds(mp5ReloadTime);

        // Refill the MP5 magazine
        int ammoToReload = Mathf.Min(mp5MagazineSize, mp5Ammo);
        currentMP5Ammo = ammoToReload;
        mp5Ammo -= ammoToReload;

        isMP5Reloading = false;
        Debug.Log("MP5 reloaded! Ammo in magazine: " + currentMP5Ammo + " | Remaining ammo: " + mp5Ammo);
    }
}

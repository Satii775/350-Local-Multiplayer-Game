using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShooter : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float zoomedFOV = 20f;
    public float zoomSpeed = 5f;

    [Header("Projectile Settings")]
    public GameObject pistolProjectilePrefab;
    public GameObject mp5ProjectilePrefab;
    public GameObject m4ProjectilePrefab;
    public GameObject shotgunPelletPrefab;
    public Transform firePoint;
    public float projectileSpeed = 365.76f;

    [Header("Pistol Settings")]
    public bool hasPistol = false;
    public int pistolAmmo = 45;
    private int currentPistolAmmo = 15;
    public int pistolMagazineSize = 15;
    private float pistolReloadTime = 2f;
    private bool isPistolReloading = false;
    public float pistolMOA = 1f;

    [Header("MP5 Settings")]
    public bool hasMP5 = false;
    public int mp5Ammo = 120;
    public int currentMP5Ammo = 30;
    public int mp5MagazineSize = 30;
    private float mp5FireRate = 950f / 60f;
    private float mp5ReloadTime = 2f;
    private bool isMP5Reloading = false;
    private float nextTimeToFireMP5 = 0f;
    public float mp5MOA = 1f;
    private float mp5AutoTime = 0f;

    [Header("M4 Settings")]
    public bool hasM4 = false;
    public int m4Ammo = 150;
    private int currentM4Ammo = 30;
    public int m4MagazineSize = 30;
    private float m4FireRate = 900f / 60f;
    private float m4ReloadTime = 2.5f;
    private bool isM4Reloading = false;
    private float nextTimeToFireM4 = 0f;
    public float m4MOA = 1f;
    private float m4AutoTime = 0f;

    [Header("Shotgun Settings")]
    public bool hasShotgun = false;
    public int shotgunAmmo = 24;
    private int currentShotgunAmmo = 6;
    public int shotgunMagazineSize = 6;
    public float shotgunReloadTime = 3f;
    public float shotgunMOA = 4f;
    private bool isShotgunReloading = false;
    private float nextShotgunTime = 0f;
    public GameObject shotgunShellPrefab;

    public GameObject mp5ShellPrefab;
    public GameObject m4ShellPrefab;

    private enum WeaponType { Pistol, MP5, M4, Shotgun }
    private WeaponType currentWeapon;

    private PlayerInput playerInput;
    private InputAction aimAction;
    private InputAction shootAction;
    private InputAction switchWeaponAction;

    [Header("Weapon HUD Settings")]
    public Image weaponUIImage;
    public Sprite pistolSprite;
    public Sprite mp5Sprite;
    public Sprite m4Sprite;
    public Sprite shotgunSprite;
    public float rotationAngle = 10f;
    public float rotationSpeed = 2f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        aimAction = playerInput.actions["Aim"];
        shootAction = playerInput.actions["Shoot"];
        switchWeaponAction = playerInput.actions["SwitchWeapon"];
        switchWeaponAction.performed += SwitchWeapon;
        currentWeapon = WeaponType.Pistol;

        UpdateWeaponHUD();
    }

    void Update()
    {
        HandleZoom();
        UpdateFirePoint();
        HandleShooting();
    }

    Vector3 CalculateMOADirection(Transform firePoint, float moa)
    {
        // Convert MOA to an angle in degrees for spread (1 MOA = ~0.000290888 radians per unit)
        float spreadAngle = moa * 0.000290888f;

        // Calculate random angular deviation for both X (up/down) and Y (left/right) directions
        float randomX = Random.Range(-spreadAngle, spreadAngle);
        float randomY = Random.Range(-spreadAngle, spreadAngle);

        // Create a deviation rotation based on the random angles
        Quaternion deviationRotation = Quaternion.Euler(randomX, randomY, 0);

        // Apply the deviation to the fire point's forward direction
        return deviationRotation * firePoint.forward;
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
        firePoint.position = playerCamera.transform.position + playerCamera.transform.forward * 1.0f;
        firePoint.rotation = playerCamera.transform.rotation;
    }

    void HandleShooting()
    {
        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                if (hasPistol && !isPistolReloading) HandlePistolShooting();
                break;
            case WeaponType.MP5:
                if (hasMP5 && !isMP5Reloading) HandleMP5Shooting();
                break;
            case WeaponType.M4:
                if (hasM4 && !isM4Reloading) HandleM4Shooting();
                break;
            case WeaponType.Shotgun:
                if (hasShotgun && !isShotgunReloading && Time.time >= nextShotgunTime) HandleShotgunShooting();
                break;
        }
    }

    void HandlePistolShooting()
    {
        if (shootAction.triggered && currentPistolAmmo > 0)
        {
            ShootProjectile(pistolProjectilePrefab, pistolMOA);
            currentPistolAmmo--;
        }

        if (currentPistolAmmo == 0 && !isPistolReloading)
        {
            StartCoroutine(ReloadPistol());
        }
    }

    void HandleMP5Shooting()
    {
        if (shootAction.ReadValue<float>() > 0 && Time.time >= nextTimeToFireMP5 && currentMP5Ammo > 0)
        {
            nextTimeToFireMP5 = Time.time + 1f / mp5FireRate;
            mp5AutoTime += 0.1f;
            ShootProjectile(mp5ProjectilePrefab, mp5MOA + mp5AutoTime * 1.5f);
            DropShell(mp5ShellPrefab);
            currentMP5Ammo--;
        }
        else if (shootAction.ReadValue<float>() == 0)
        {
            mp5AutoTime = 0f;
        }

        if (currentMP5Ammo == 0 && !isMP5Reloading)
        {
            StartCoroutine(ReloadMP5());
        }
    }

    void HandleM4Shooting()
    {
        if (shootAction.ReadValue<float>() > 0 && Time.time >= nextTimeToFireM4 && currentM4Ammo > 0)
        {
            nextTimeToFireM4 = Time.time + 1f / m4FireRate;
            m4AutoTime += 0.1f;
            ShootProjectile(m4ProjectilePrefab, m4MOA + m4AutoTime);
            DropShell(m4ShellPrefab);
            currentM4Ammo--;
        }
        else if (shootAction.ReadValue<float>() == 0)
        {
            m4AutoTime = 0f;
        }

        if (currentM4Ammo == 0 && !isM4Reloading)
        {
            StartCoroutine(ReloadM4());
        }
    }

    void HandleShotgunShooting()
    {
        if (shootAction.triggered && currentShotgunAmmo > 0)
        {
            ShootShotgun();
            DropShell(shotgunShellPrefab);
            nextShotgunTime = Time.time + 0.5f;
            currentShotgunAmmo--;
        }

        if (currentShotgunAmmo == 0 && !isShotgunReloading)
        {
            StartCoroutine(ReloadShotgun());
        }
    }

    void ShootProjectile(GameObject projectilePrefab, float moa)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = GetSpreadDirection(moa);
            rb.velocity = direction * projectileSpeed;
        }
    }

    void ShootShotgun()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject pellet = Instantiate(shotgunPelletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = pellet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = GetSpreadDirection(shotgunMOA) * projectileSpeed;
            }
        }
    }

    Vector3 GetSpreadDirection(float moa)
    {
        float spreadAngle = moa * 0.000290888f;
        Vector3 direction = firePoint.forward;
        direction = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0) * direction;
        return direction;
    }

    void DropShell(GameObject shellPrefab)
    {
        if (shellPrefab != null)
        {
            Instantiate(shellPrefab, firePoint.position, Quaternion.identity);
        }
    }

    void SwitchWeapon(InputAction.CallbackContext context)
    {
        do
        {
            currentWeapon = (WeaponType)(((int)currentWeapon + 1) % System.Enum.GetValues(typeof(WeaponType)).Length);
        } while (!HasWeaponAndAmmo());

        UpdateWeaponHUD();
    }

    void UpdateWeaponHUD()
    {
        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                weaponUIImage.sprite = pistolSprite;
                break;
            case WeaponType.MP5:
                weaponUIImage.sprite = mp5Sprite;
                break;
            case WeaponType.M4:
                weaponUIImage.sprite = m4Sprite;
                break;
            case WeaponType.Shotgun:
                weaponUIImage.sprite = shotgunSprite;
                break;
        }
    }

    bool HasWeaponAndAmmo()
    {
        switch (currentWeapon)
        {
            case WeaponType.Pistol: return hasPistol && currentPistolAmmo > 0;
            case WeaponType.MP5: return hasMP5 && currentMP5Ammo > 0;
            case WeaponType.M4: return hasM4 && currentM4Ammo > 0;
            case WeaponType.Shotgun: return hasShotgun && currentShotgunAmmo > 0;
            default: return false;
        }
    }

    IEnumerator RockHUDImage()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f)  // duration for rocking effect
        {
            float angle = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;
            weaponUIImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        weaponUIImage.rectTransform.localRotation = Quaternion.identity;
    }

    IEnumerator ReloadPistol()
    {
        isPistolReloading = true;
        StartCoroutine(RockHUDImage());
        yield return new WaitForSeconds(pistolReloadTime);

        int ammoToReload = Mathf.Min(pistolMagazineSize, pistolAmmo);
        currentPistolAmmo = ammoToReload;
        pistolAmmo -= ammoToReload;
        isPistolReloading = false;
    }

    IEnumerator ReloadMP5()
    {
        isMP5Reloading = true;
        StartCoroutine(RockHUDImage());
        yield return new WaitForSeconds(mp5ReloadTime);

        int ammoToReload = Mathf.Min(mp5MagazineSize, mp5Ammo);
        currentMP5Ammo = ammoToReload;
        mp5Ammo -= ammoToReload;
        isMP5Reloading = false;
    }

    IEnumerator ReloadM4()
    {
        isM4Reloading = true;
        StartCoroutine(RockHUDImage());
        yield return new WaitForSeconds(m4ReloadTime);

        int ammoToReload = Mathf.Min(m4MagazineSize, m4Ammo);
        currentM4Ammo = ammoToReload;
        m4Ammo -= currentM4Ammo;
        isM4Reloading = false;
    }

    IEnumerator ReloadShotgun()
    {
        isShotgunReloading = true;
        StartCoroutine(RockHUDImage());
        yield return new WaitForSeconds(shotgunReloadTime);

        int ammoToReload = Mathf.Min(shotgunMagazineSize, shotgunAmmo);
        currentShotgunAmmo = ammoToReload;
        shotgunAmmo -= ammoToReload;
        isShotgunReloading = false;
    }
}

using System.Collections;
using UnityEngine;

public class ShellEjector : MonoBehaviour
{
    [Header("Ejection Settings")]
    public Transform firePoint;             // Reference to the player's fire point (assign in Inspector)
    public float minEjectForce = 0.1f;      // Minimum ejection distance/force
    public float maxEjectForce = 1f;        // Maximum ejection distance/force
    public float upwardEjectForce = 0.5f;   // Upward force for realism
    public float backwardOffset = 0.25f;    // Offset to spawn the shell slightly behind the fire point
    public float rightOffset = 0.1f;        // Offset to spawn the shell slightly to the right of the fire point

    [Header("Despawn Settings")]
    public float despawnTime = 5f;          // Time before the shell despawns

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Ensure we have a valid fire point assigned
        if (firePoint == null)
        {
            Debug.LogError("Fire Point not assigned in ShellEjector. Please assign a fire point in the Inspector.");
            return;
        }

        // Set initial position and rotation based on the fire point
        SetInitialPositionAndRotation();

        // Apply random ejection force
        EjectShell();

        // Schedule despawn
        StartCoroutine(DespawnAfterTime());
    }

    void SetInitialPositionAndRotation()
    {
        // Set the shell's initial position based on the fire point's position, backward, and right offsets
        Vector3 spawnPosition = firePoint.position;
        spawnPosition -= firePoint.forward * backwardOffset; // Apply backward offset
        spawnPosition += firePoint.right * rightOffset;      // Apply right offset

        // Set position and rotation to match the calculated spawn position and fire point rotation
        transform.position = spawnPosition;
        transform.rotation = firePoint.rotation;
    }

    void EjectShell()
    {
        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody attached to shell. Cannot apply ejection force.");
            return;
        }

        // Generate random force between the specified min and max values
        float ejectForce = Random.Range(minEjectForce, maxEjectForce);

        // Randomize the direction with some upward force for realism
        Vector3 ejectDirection = firePoint.right + Vector3.up * upwardEjectForce;
        ejectDirection = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * ejectDirection;

        // Apply the force to the shell's Rigidbody
        rb.AddForce(ejectDirection * ejectForce, ForceMode.Impulse);

        // Apply a slight random spin to the shell for realism
        rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * ejectForce, ForceMode.Impulse);
    }

    IEnumerator DespawnAfterTime()
    {
        // Wait for the specified despawn time, then destroy the shell
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}

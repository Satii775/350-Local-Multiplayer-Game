using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 1000f;
    public int damage = 25;  // Damage dealt by the projectile
    public GameObject firedBy;

    [Header("Blood Effect")]
    public GameObject bloodEffectPrefab; // Assign the BloodSplatter prefab here

    [Header("Bullet Hole Effect")]
    public GameObject bulletHolePrefab; // Assign the BulletHole prefab here

    private Rigidbody rb;

    private void Start()
    {
        InitializeRigidbody();
        LaunchProjectile();
    }

    private void InitializeRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void LaunchProjectile()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile hit an enemy (or any object you want blood to appear on)
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SpawnBloodEffect(collision);
        }
        else if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("wall"))
        {
            // Spawn bullet hole only if the object is tagged as "Ground" or "Wall"
            SpawnBulletHole(collision);
        }

        // Destroy the projectile after a short delay to allow collision processing
        Destroy(gameObject, 0.1f);
    }

    private void SpawnBloodEffect(Collision collision)
    {
        // Get the contact point of the collision
        ContactPoint contact = collision.contacts[0];

        // Instantiate the blood effect at the point of impact and align it with the surface normal
        Instantiate(bloodEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
    }

    private void SpawnBulletHole(Collision collision)
    {
        // Get the contact point of the collision
        ContactPoint contact = collision.contacts[0];

        // Instantiate the bullet hole at the point of impact and align it with the surface normal
        GameObject bulletHole = Instantiate(bulletHolePrefab, contact.point, Quaternion.LookRotation(contact.normal));

        // Scale and adjust the bullet hole to ensure it lies flat against the surface
        bulletHole.transform.localScale = new Vector3(0.05f, 0.05f, 0.01f); // Adjust the scale to match the bullet hole size

        // Optional: Attach the bullet hole to the hit object to ensure it moves with the object if it’s dynamic
        bulletHole.transform.SetParent(collision.transform);
    }
}

using UnityEngine;

public class WanderAround : MonoBehaviour
{
    public float wanderRadius = 50f;        // Maximum distance from the original position
    public float moveSpeed = 1.5f;          // A slow, walking pace for NPC-like movement
    public float rotationSpeed = 1.5f;      // Slower rotation for more natural turning
    public float stopDuration = 2f;         // Time spent stopped
    public float avoidanceDistance = 3f;    // Distance to avoid walls
    public LayerMask wallLayer;             // Layer for walls

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMoving = true;
    private bool isStopped = false;
    private float stopTimer = 0f;

    void Start()
    {
        originalPosition = transform.position;
        SetNewTargetPosition();
    }

    void Update()
    {
        if (!isStopped && isMoving)
        {
            MoveTowardsTarget();
        }
        else if (isStopped)
        {
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopDuration)
            {
                stopTimer = 0f;
                isStopped = false;
                SetNewTargetPosition();
            }
        }
    }

    void MoveTowardsTarget()
    {
        // Check if there's a wall ahead
        if (!IsWallInFront())
        {
            // Move towards the target position smoothly
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Smoothly rotate towards the target position
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // If the target is reached, stop and wait before moving again
            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                isMoving = false;
                isStopped = true;
            }
        }
        else
        {
            // If a wall is in front, set a new target to avoid it
            SetNewTargetPosition();
        }
    }

    bool IsWallInFront()
    {
        RaycastHit hit;
        // Cast a ray forward to check for walls within the avoidance distance
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceDistance, wallLayer))
        {
            if (hit.collider.CompareTag("wall"))
            {
                return true;
            }
        }
        return false;
    }

    void SetNewTargetPosition()
    {
        bool targetValid = false;
        int attempts = 0;

        while (!targetValid && attempts < 30) // Limit attempts to avoid endless loop
        {
            // Pick a new random target position within the wander radius
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += originalPosition;
            randomDirection.y = transform.position.y;  // Keep on the same plane
            targetPosition = randomDirection;

            // Ensure the new position is not within avoidance distance of any walls
            if (!Physics.CheckSphere(targetPosition, avoidanceDistance, wallLayer))
            {
                targetValid = true;
            }
            attempts++;
        }

        isMoving = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the enemy collides with an object with a box collider, move away
        if (collision.collider.GetType() == typeof(BoxCollider))
        {
            Vector3 collisionDirection = collision.contacts[0].point - transform.position;
            collisionDirection = -collisionDirection.normalized; // Move away from the collision
            transform.position += collisionDirection * moveSpeed * Time.deltaTime;
        }
    }
}

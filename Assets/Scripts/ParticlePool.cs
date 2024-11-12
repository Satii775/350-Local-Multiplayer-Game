using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    public GameObject bloodEffectPrefab;  // Assign the blood effect prefab here
    public int poolSize = 10;

    private Queue<GameObject> pool;

    private void Start()
    {
        pool = new Queue<GameObject>();

        // Create pool of blood effects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bloodEffect = Instantiate(bloodEffectPrefab);
            bloodEffect.SetActive(false);
            pool.Enqueue(bloodEffect);
        }
    }

    public void SpawnBloodEffect(Vector3 position, Quaternion rotation)
    {
        if (pool.Count > 0)
        {
            GameObject bloodEffect = pool.Dequeue();
            bloodEffect.transform.position = position;
            bloodEffect.transform.rotation = rotation;
            bloodEffect.SetActive(true);

            StartCoroutine(ReturnToPool(bloodEffect, 2f)); // Return to pool after 2 seconds or the particle's duration
        }
    }

    private IEnumerator ReturnToPool(GameObject bloodEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        bloodEffect.SetActive(false);
        pool.Enqueue(bloodEffect);
    }
}

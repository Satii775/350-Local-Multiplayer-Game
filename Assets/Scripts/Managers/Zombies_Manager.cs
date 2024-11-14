using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombies_Manager : MonoBehaviour
{
    private GameObject[] Zombies;
    [SerializeField] private Vector3 spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize;

    private int round = 1;
    private int zombiesToSpawn = 5;
    private int zombiesAlive = 0;

    // Start is called before the first frame update
    void Start()
    {
        Zombies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("Zombies: " + Zombies.Length);

        // Debug for seeing what objects are found
        // foreach (GameObject zombie in Zombies)
        // {
        //     Debug.Log("Zombie found: " + zombie.name);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (zombiesAlive <= 0)
        {
            StartCoroutine(SpawnZombies(round));
        }
    }

    IEnumerator SpawnZombies(int round)
    {
        zombiesAlive = zombiesToSpawn;

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            // Randomly select a zombie from the list
            int randomZombie = Random.Range(0, Zombies.Length);
            GameObject zombie = Zombies[randomZombie];

            // Randomly determine a spawn position within the spawn area
            Vector3 randomPosition = spawnAreaCenter + new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            // Find a valid position on the NavMesh near the random position
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                zombie.transform.position = hit.position;
                NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = true; // Ensure the NavMeshAgent is enabled
                    yield return null; // Wait for one frame to ensure the agent is initialized
                }
                zombie.GetComponent<ZombieAIController>().SpawnZombies();
            }
            else
            {
                Debug.LogWarning("Failed to find a valid NavMesh position for zombie spawn.");
            }

            yield return new WaitForSeconds(1);
        }

        round++;
    }

    public void KillZombie()
    {
        zombiesAlive--;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombies_Manager : MonoBehaviour
{
    private GameObject[] Zombies;
    [SerializeField] private Vector3 spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize;

    public int round = 9;
    public int zombiesToSpawn = 5;
    private int zombiesAlive = 5;
    private bool roundStart = true;
    private GameObject WinScreen;

    private GameObject manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("Manager");
        if (manager == null)
        {
            Debug.LogWarning("Manager not found");
        }

        WinScreen = FindInactiveObjectByTag("Win menu");
        if (WinScreen == null)
        {
            Debug.LogWarning("Win menu not found");
        }
        else
        {
            Debug.Log("Win menu found");
        }
        Zombies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("Zombies: " + Zombies.Length);

        StartCoroutine(SpawnZombies(round));

        // Debug for seeing what objects are found
        // foreach (GameObject zombie in Zombies)
        // {
        //     Debug.Log("Zombie found: " + zombie.name);
        // }
    }

    private GameObject FindInactiveObjectByTag(string tag)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform obj in objs)
        {
            if (obj.hideFlags == HideFlags.None && obj.CompareTag(tag))
            {
                return obj.gameObject;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (zombiesToSpawn == 50 && zombiesAlive <= 0)
        {
            WinScreen.SetActive(true);
            manager.GetComponent<Player_Manager>().win = true;
            Time.timeScale = 0;
            Cursor.visible = true;
        }
        if (zombiesAlive <= 0 && roundStart == true)
        {
            roundStart = false;
            StartCoroutine(WaitAndSpawnZombies());
        }
        Debug.Log("Zombie number " + zombiesAlive);
    }

    IEnumerator WaitAndSpawnZombies()
    {
        manager.GetComponent<Player_Manager>().PlayersRevived();
        yield return new WaitForSeconds(10); // Wait for 10 seconds
        round++;
        zombiesToSpawn += 5; // Increase the number of zombies to spawn by 5 each round
        zombiesAlive = zombiesToSpawn;
        StartCoroutine(SpawnZombies(round));
    }

    IEnumerator SpawnZombies(int round)
    {
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            GameObject zombie = Zombies[i];

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
        roundStart = true;

        Debug.Log("Round number" + round);
        Debug.Log("Zombie number " + zombiesToSpawn);
    }

    public void KillZombie()
    {
        zombiesAlive--;
    }

    // Draw the spawn area in the Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}

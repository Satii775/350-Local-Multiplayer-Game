using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            //randomly Select Zombie from the list
            int randomZombie = Random.Range(0, Zombies.Length);
            GameObject zombie = Zombies[randomZombie];

            // Randomly determine a spawn position within the spawn area
            Vector3 spawnPosition = spawnAreaCenter + new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            zombie.transform.position = spawnPosition;
            yield return new WaitForSeconds(1);
        }

        round++;
    }

    public void KillZombie()
    {
        zombiesAlive--;
    }
}

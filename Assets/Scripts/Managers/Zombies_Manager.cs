using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombies_Manager : MonoBehaviour
{
    private GameObject[] Zombies;

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
        
    }
}

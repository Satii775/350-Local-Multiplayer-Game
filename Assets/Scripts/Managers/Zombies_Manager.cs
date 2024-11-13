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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

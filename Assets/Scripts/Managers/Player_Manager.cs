using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();

    private GameObject lostMenu;

    void Awake()
    {
        lostMenu = GameObject.FindWithTag("Lost menu");
    }

    public void PlayerDied(GameObject player)
    {
        players.Add(player);
    }

    public void PlayersRevived()
    {
        foreach (GameObject player in players)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.RespawnPlayer();
            }
        }
    }

    void FixedUpdate()
    {
        if (players.Count == 2)
        {
            lostMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
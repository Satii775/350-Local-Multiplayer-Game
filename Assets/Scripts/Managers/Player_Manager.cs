using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();

    private GameObject lostMenu;

    void Awake()
    {
        Debug.Log("PlayerManager Awake called");
    }

    void Start()
    {
        Debug.Log("PlayerManager Start called");
        lostMenu = FindInactiveObjectByTag("Lost menu");
        if (lostMenu == null)
        {
            Debug.LogWarning("Lost Menu not found in the scene.");
        }
        else
        {
            Debug.Log("Lost Menu found successfully.");
        }
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
        players.Clear(); // Clear the list after processing
    }

    void FixedUpdate()
    {
        if (players.Count == 2)
        {
            lostMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
        }
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
}
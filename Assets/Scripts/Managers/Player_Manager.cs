using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();

    private GameObject lostMenu;

    public bool win = false;

    void Awake()
    {
        //Debug.Log("PlayerManager Awake called");
    }

    void Start()
    {
        //Debug.Log("PlayerManager Start called");
        lostMenu = FindInactiveObjectByTag("Lost menu");
        if (lostMenu == null)
        {
            //Debug.LogWarning("Lost Menu not found in the scene.");
        }
        else
        {
            //Debug.Log("Lost Menu found successfully.");
        }
        win = false;
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

    void Update()
    {
        if (players.Count == 2)
        {
            Cursor.lockState = CursorLockMode.None;
            lostMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
        }
        else if ( win == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
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
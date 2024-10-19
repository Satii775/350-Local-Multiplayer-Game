using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private int NumberOfPlayers = 0;

    public int CheckTotalPlayers() {
        Debug.Log(NumberOfPlayers);
        return NumberOfPlayers;
    }

    public void AddPlayer() {
        Debug.Log("added player");
        NumberOfPlayers++;
    }
}

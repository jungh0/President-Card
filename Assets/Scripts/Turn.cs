using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour
{
    private int nowIndex = 0;
    private readonly List<Player> totalPlayer = new List<Player>();

    public void Initialise()
    {
        
    }

    public void AddPlayer(Player a)
    {
        totalPlayer.Add(a);
    }

    public Player GetNowTurn()
    {
        return totalPlayer[nowIndex];
    }

    public void NextTurn()
    {
        nowIndex += 1;
        if(totalPlayer.Count <= nowIndex)
        {
            nowIndex = 0;
        }
    }
}

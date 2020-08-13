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

    public int TotalPlayer()
    {
        return totalPlayer.Count;
    }

    public void DelPlayer(Player a)
    {
        totalPlayer.Remove(a);
    }

    public Player GetNowTurn()
    {
        try
        {
            return totalPlayer[nowIndex];
        }
        catch
        {
            nowIndex = 0;
            return totalPlayer[0];
        }
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

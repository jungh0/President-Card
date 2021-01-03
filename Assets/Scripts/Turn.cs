using Game;
using System.Collections.Generic;
using UnityEngine.UI;

public class Turn : WinManager
{
    private int nowIndex = 0;
    private readonly List<Player> nowPlayer = new List<Player>();
    private readonly List<Player> totalPlayer = new List<Player>();
    public Player trash;
    public Text statusText;

    public new void Initialise()
    {
    }


    public void AddPlayer(Player a)
    {
        nowPlayer.Add(a);
        totalPlayer.Add(a);
    }

    public int[] GetPlayerCardsCnt()
    {
        var total = new int[4] { 0, 0, 0, 0 };
        var index = 0;
        foreach (var tmp in this.GetTotalPlayer())
        {
            total[index] = tmp.cards.Count;
            index++;
        }
        return total;
    }

    public List<Player> GetTotalPlayer()
    {
        return totalPlayer;
    }

    public int NowPlayerCount()
    {
        return nowPlayer.Count;
    }

    public void DelNowPlayer(Player a)
    {
        nowPlayer.Remove(a);
    }

    public Player GetNowTurn()
    {
        if (nowPlayer.Count > 0)
        {
            try
            {
                return nowPlayer[nowIndex];
            }
            catch
            {
                nowIndex = 0;
                return nowPlayer[0];
            }
        }
        else
        {
            return null;
        }
    }

    public void NextTurn()
    {
        nowIndex += 1;
        if (nowPlayer.Count <= nowIndex)
        {
            nowIndex = 0;
        }
        ChangeStatusNowTurn();
    }

    public void ChangeStatus(string tt)
    {
        statusText.text = tt;
    }

    public void ChangeStatusNowTurn()
    {
        if(GetNowTurn() != null)
        {
            ChangeStatus($"{GetNowTurn().name}");
        }
        else
        {
            ChangeStatus($"Game Done!");
        }
        
    }
}

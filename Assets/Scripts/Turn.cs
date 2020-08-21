using Game;
using System.Collections.Generic;

public class Turn : WinManager
{
    private int nowIndex = 0;
    private readonly List<Player> nowPlayer = new List<Player>();
    private readonly List<Player> totalPlayer = new List<Player>();
    public Player trash;

    public void Initialise()
    {

    }

    public void AddPlayer(Player a)
    {
        nowPlayer.Add(a);
        totalPlayer.Add(a);
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
    }
}

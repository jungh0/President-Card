using Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    //1등순서대로
    public readonly List<Player> whoWin = new List<Player>();

    public void Initialise()
    {

    }

    public void AddWinner(Player p)
    {
        whoWin.Add(p);
        p.text.GetComponentInChildren<Text>().text = $"{whoWin.Count} 등!!!";
    }

}

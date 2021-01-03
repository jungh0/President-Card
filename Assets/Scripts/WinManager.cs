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
        //p.text.GetComponentInChildren<Text>().text = $"{whoWin.Count} 등!!!";
    }

    public bool IsGameDone()
    {
        return whoWin.Count >= 4;
    }

    public List<string> IsGameDoneList()
    {
        var aa = new List<string>();
        foreach(var a in whoWin)
        {
            aa.Add(a.name.Replace("Your turn","You").Replace("'s turn", ""));
        }
        return aa;
    }

}

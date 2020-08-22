using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAlgorithm : Turn
{
    private List<Card> nowStatus = null;
    private List<Card> nowStatusTmp = null;

    private int passCnt = 0;

    public new void Initialise()
    {

    }

    public bool IsCanSubmit()
    {
        return nowStatusTmp != null;
    }


    public void CheckDisable(Player now)
    {
        if(nowStatus == null)
        {
            if (nowStatusTmp?.Count > 0)
            {
                nowStatusTmp.Sort((a, b) =>
                {
                    if ((int)a.Rank > (int)b.Rank)
                    {
                        return 1;
                    }
                    else if ((int)a.Rank < (int)b.Rank)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                );

                if (nowStatusTmp?.Count == 1)
                {
                    foreach (var tmp in now.cards)
                    {
                        if (tmp.Rank != nowStatusTmp[0].Rank &&
                            tmp.Rank != nowStatusTmp[0].Rank + 1 &&
                            tmp.Rank != nowStatusTmp[0].Rank - 1)
                        {
                            tmp.Disable(true);
                        }
                    }
                }
                else if (nowStatusTmp?.Count > 1)
                {
                    bool same = nowStatusTmp[0].Rank == nowStatusTmp[1].Rank;

                    if (same)
                    {
                        foreach (var tmp in now.cards)
                        {
                            if (tmp.Rank != nowStatusTmp[0].Rank)
                            {
                                tmp.Disable(true);
                            }
                        }
                    }
                    else
                    {
                        foreach (var tmp in now.cards)
                        {
                            if (tmp.Rank != nowStatusTmp[0].Rank - 1 &&
                                tmp.Rank != nowStatusTmp[nowStatusTmp.Count - 1].Rank + 1)
                            {
                                tmp.Disable(true);
                            }
                        }
                    }

                }
            }
        }
        else
        {
            var nowCnt = nowStatus.Count;
            if (nowCnt == 1 && nowStatusTmp == null)
            {
                foreach (var tmp in now.cards)
                {
                    if (tmp.Rank <= nowStatus[0].Rank)
                    {
                        tmp.Disable(true);
                    }
                }
            }
            else if (nowCnt > 1)
            {
                bool same = nowStatus[0].Rank == nowStatus[1].Rank;
                List<Card> on = new List<Card>();

                if (same)
                {
                    for (int a = 0; a <= now.cards.Count - nowCnt; a++)
                    {
                        var tmp = now.cards[a];
                        if (tmp.Rank > nowStatus[0].Rank)
                        {
                            bool allTrue = true;
                            for (int b = 1; b < nowCnt; b++)
                            {
                                var tmp2 = now.cards[a+b];
                                if (tmp.Rank != tmp2.Rank)
                                {
                                    allTrue = false;
                                    break;
                                }
                            }
                            if (allTrue)
                            {
                                on.Add(tmp);
                            }
                        }
                    }

                    foreach(var tmp3 in now.cards)
                    {
                        if (!on.Contains(tmp3))
                        {
                            tmp3.Disable(true);
                        }
                    }
                }
                else
                {
                    for (int a = 0; a <= now.cards.Count - nowCnt; a++)
                    {
                        var tmp = now.cards[a];
                        if (tmp.Rank > nowStatus[nowStatus.Count-1].Rank)
                        {
                            int trueCnt = 0;
                            var tmpRank = tmp.Rank;

                            for (int b = a + 1; b < now.cards.Count - nowCnt; b++)
                            {
                                var tmp2 = now.cards[b];
                                if (tmpRank + 1 == tmp2.Rank)
                                {
                                    trueCnt++;
                                    tmpRank++;

                                    if (trueCnt == nowCnt)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (trueCnt == nowCnt)
                            {
                                on.Add(tmp);
                            }
                        }
                    }

                    foreach (var tmp3 in now.cards)
                    {
                        if (!on.Contains(tmp3))
                        {
                            tmp3.Disable(true);
                        }
                    }
                }
            }
            else
            {
                foreach (var tmp in now.cards)
                {
                    tmp.Disable(true);
                }
            }
        }

        if(nowStatusTmp != null)
        {
            foreach (var tmp in nowStatusTmp)
            {
                tmp.Disable(false);
            }
        }
        

    }

    public void MakeBlackCard(Player now)
    {
        now.AllCardDisable(true);

        //현재 턴인 사람 뺴고 검정 반투명
        foreach (var tmp in this.GetTotalPlayer())
        {
            tmp.AllCardDisable(tmp != now);
        }

        //내 카드중에 낼수없는데 반투명
        if (!now.isHouse)
        {
            CheckDisable(now);
        }
    }

    /// <summary>
    /// 패스
    /// </summary>
    public void PassClick()
    {
        SoundManager.instance.PlaySound("soundCard3");
        passCnt += 1;
        this.NextTurn();
        StartCoroutine(AI());
    }

    public void CancelClick()
    {
        if (GetNowTurn() is Player p)
        {
            if (!p.isHouse)
            {
                nowStatusTmp = null;
                foreach (var tmp in p.cards)
                {
                    tmp.Disable(false);
                }
                GetNowTurn().SortCard();
            }
        }  
    }

    public void SubmitClick()
    {
        var now = GetNowTurn();
        var deck = now.cards;

        SoundManager.instance.PlaySound("soundCard2");
        passCnt = 0;
        foreach (var tmp in nowStatusTmp)
        {
            deck.Remove(tmp);
        }
        trash.SortCardTrash(nowStatusTmp);
        nowStatus = nowStatusTmp;
        nowStatusTmp = null;
        now.SortCard();
        
        if (now.cards.Count == 0)
        {
            this.AddWinner(now);
        }

        //this.NextTurn();
        //StartCoroutine(AI());
    }

    public void SelectCard(Player p, Card wantTrash)
    {
        if ((!nowStatusTmp?.Contains(wantTrash) ?? true) && !wantTrash.isDisable)
        {
            if (nowStatusTmp == null)
            {
                nowStatusTmp = new List<Card>();
            }

            if(nowStatus == null)
            {
                iTween.MoveAdd(wantTrash.gameObject, new Vector2(0, 1), 1f);
                nowStatusTmp.Add(wantTrash);
            }
            else
            {
                var nowCnt = nowStatus.Count;
                for (int t = 0; t <= p.cards.Count - nowCnt; t++)
                {
                    if(p.cards[t] == wantTrash)
                    {
                        for (int tt = 0; tt < nowCnt; tt++)
                        {
                            iTween.MoveAdd(p.cards[t + tt].gameObject, new Vector2(0, 1), 1f);
                            nowStatusTmp.Add(p.cards[t+tt]);
                        }
                    }
                }
            }

            
        }
    }


    /// <summary>
    /// 카드 제출
    /// </summary>
    /// <param name="p">제출하고자하는 플레이어</param>
    /// <param name="wantTrash">제출하고자하는 카드</param>
    public void TrashCard(Player p, Card wantTrash)
    {
        CheckPass();

       /* if (nowStatus == null || wantTrash.Rank > nowStatus.Rank)
        {
            var deck = p.cards;
            if (deck.Contains(wantTrash))
            {
                SoundManager.instance.PlaySound("soundCard2");
                passCnt = 0;
                deck.Remove(wantTrash);
                trash.Add(wantTrash);
                nowStatus = wantTrash;
                p.SortCard();
                this.NextTurn();
                StartCoroutine(AI());
            }

            //이긴사람 list 추가
            if (p.cards.Count == 0)
            {
                this.AddWinner(p);
            }
        }*/

    }

    /// <summary>
    /// 모두 패스하여 이번 라운드 승리한 경우
    /// </summary>
    public void CheckPass()
    {
        //Debug.Log($"{passCnt + 1} {turn.TotalPlayer()}");
        if (passCnt + 1 >= this.NowPlayerCount())
        {
            nowStatus = null;
            passCnt = 0;
            foreach (var tmp in trash.cards.ToArray())
            {
                trash.cards.Remove(tmp);
                iTween.MoveTo(tmp.gameObject, new Vector2(5, 0), 1f);
            }
        }
    }



    /// <summary>
    /// AI 함수
    /// </summary>
    /// <returns></returns>
    public IEnumerator AI()
    {
        yield return new WaitForSeconds(1);

        try
        {
            CheckPass();

            var p = this.GetNowTurn();
            if (p.cards.Count == 0)
            {
                //이 플레이어는 승리 했다.
                //가지고있는 카드의 개수가 0이다.
                this.DelNowPlayer(p);
                StartCoroutine(AI()); //그러므로 다음 턴
            }
            else
            {
                if (p.isHouse)
                {
                    /*var available = p.PlayCard(nowStatus);
                    if (available != null)
                    {
                        TrashCard(this.GetNowTurn(), available);
                    }
                    else
                    {
                        SoundManager.instance.PlaySound("soundCard3");
                        passCnt += 1;
                        this.NextTurn();
                        StartCoroutine(AI());
                    }*/
                }
            }
        }
        catch
        {

        }
    }

}

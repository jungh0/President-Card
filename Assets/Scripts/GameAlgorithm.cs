using Game;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameAlgorithm : Turn
{
    /*
     * 0:1개만 있는거
     * 1:같은거
     * 2:연속된거
     */
    private int status = -1;
    private int count = -1;
    private Card nowCard = null;


    private List<Card> nowSelectedCard = null;

    private int passCnt = 0;

    public new void Initialise()
    {

    }

    public void MakeBlackCard(Player now)
    {
        //now.CardDisable(true);

        //현재 턴인 사람 뺴고 검정 반투명
        foreach (var tmp in this.GetTotalPlayer())
        {
            tmp.CardDisable(tmp != now);
        }

        //내 카드중에 낼수없는데 반투명
        if (!now.isHouse)
        {
            now.CardDisable(true);

            for (int a = 0; a < now.cards.Count; a++)
            {
                var tmp = now.cards[a];

                if (nowCard != null && tmp.Rank > nowCard.Rank)
                {
                    tmp.Disable(false);
                }

                //작은 경우
                /*if (nowCard != null && tmp.Rank > nowCard.Rank)
                {
                    tmp.Disable(false);
                }

                //1경우
                if (nowCard != null && tmp.Rank > nowCard.Rank && status == 1 &&  a + count <= now.cards.Count)
                {
                    for (int b = 0; b < count; b++)
                    {
                        if(now.cards[b].Rank != tmp.Rank)
                        {
                            tmp.Disable(true);
                            break;
                        }
                    }
                }

                //2경우
                if (nowCard != null && tmp.Rank > nowCard.Rank && status == 2 && a + count <= now.cards.Count)
                {
                    var tmpRank = tmp.Rank;
                    
                    for (int b = 0; b < count; b++)
                    {
                        if (now.cards[b].Rank != tmp.Rank && now.cards[b].Suit == tmp.Suit)
                        {
                            tmp.Disable(true);
                            break;
                        }
                        tmpRank += 1;
                    }
                }*/


                //내가 이겼을때 같은 경우
                if (nowCard == null)
                {
                    now.CardDisable(false);
                }

            }

        }
    }

    public bool IsExist(Player now, Card.Ranks c1)
    {
        for (int a = 0; a < now.cards.Count; a++)
        {
            if (now.cards[a].Rank == c1)
            {
                now.cards[a].Disable(false);
                return true;
            }
        }
        return false;
    }


    public bool IsExist(Player now,Card c1, Card c2)
    {
        bool found1 = false;
        bool found2 = false;

        for (int a = 0; a < now.cards.Count; a++)
        {
            if(now.cards[a] == c1)
            {
                found1 = true;
            }
            if (now.cards[a] == c2)
            {
                found2 = true;
            }
        }
        return found1 && found2;
    }

    public void CancelClick()
    {
        if (GetNowTurn() is Player p)
        {
            if (!p.isHouse)
            {
                nowSelectedCard = null;
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
        foreach (var tmp in nowSelectedCard)
        {
            deck.Remove(tmp);
        }
        trash.SortCardTrash(nowSelectedCard);
        nowCard = nowSelectedCard[0];
        nowSelectedCard = null;
        now.SortCard();

        if (now.cards.Count == 0)
        {
            this.AddWinner(now);
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

    /// <summary>
    /// 카드 제출
    /// </summary>
    /// <param name="p">제출하고자하는 플레이어</param>
    /// <param name="wantTrash">제출하고자하는 카드</param>
    public void TrashCard(Player p, Card wantTrash)
    {
        CheckPass();

        if (!wantTrash.isDisabled)
        {
            if(!nowSelectedCard?.Contains(wantTrash) ?? true)
            {
                if (nowSelectedCard == null)
                {
                    nowSelectedCard = new List<Card>();
                }
                nowSelectedCard.Add(wantTrash);
                iTween.MoveAdd(wantTrash.gameObject, new Vector2(0, 1), 1f);
            }
            

            
        }

        /*if (nowCard == null || wantTrash.Rank > nowCard.Rank)
        {
            var deck = p.cards;
            if (deck.Contains(wantTrash))
            {
                SoundManager.instance.PlaySound("soundCard2");
                passCnt = 0;
                deck.Remove(wantTrash);
                trash.Add(wantTrash);
                nowCard = wantTrash;
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
            nowCard = null;
            status = -1;
            count = -1;
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
                    var available = p.PlayCard(nowCard);
                    if (available != null)
                    {
                        TrashCard(this.GetNowTurn(), available);
                    }
                    else
                    {
                        // 패스
                        SoundManager.instance.PlaySound("soundCard3");
                        passCnt += 1;
                        this.NextTurn();
                        StartCoroutine(AI());
                    }
                }
            }
        }
        catch
        {

        }
    }

}

using Game;
using System.Collections;
using UnityEngine;

public class GameAlgorithm : Turn
{
    private Card nowStatus = null;
    private int passCnt = 0;

    public new void Initialise()
    {

    }

    public void MakeBlackCard(Player now)
    {
        now.CardDisable(true);

        //현재 턴인 사람 뺴고 검정 반투명
        foreach (var tmp in this.GetTotalPlayer())
        {
            tmp.CardDisable(tmp != now);
        }

        //내 카드중에 낼수없는데 반투명
        if (!now.isHouse)
        {
            foreach (var tmp in now.cards)
            {
                if (nowStatus != null && tmp.Rank <= nowStatus.Rank)
                {
                    tmp.Disable(true);
                }
            }
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

        if (nowStatus == null || wantTrash.Rank > nowStatus.Rank)
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
        }

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
                    var available = p.PlayCard(nowStatus);
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

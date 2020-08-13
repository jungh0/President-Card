using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Game
{
    public class GameControl : MonoBehaviour
    {
        //카메라
        public GameObject Main;
        public GameObject Sub;

        //캔버스 화면
        public GameObject Canvas;
        public Transform CanvasTranform;

        //텍스트
        public GameObject text;

        //버튼
        public Button start;
        public Button pass;

        //플레이어
        private Player player, house1, house2, house3;
        public readonly List<Player> totalPlayer = new List<Player>();
        public Player trash;

        //플레이어 텍스트
        public GameObject playerName0, playerName1, playerName2, playerName3;

        //로딩중인가
        public bool isLoading = true;
        //누구의 차례인가
        public Turn turn;
        //1등순서대로
        public readonly List<Player> whoWin = new List<Player>();


        public GameObject deckPrefab;
        public GameObject playerPrefab;
        public GameObject buttonPrefab;
        private Deck deck;

        /// <summary>
        /// 패스버튼 눌렀을때
        /// </summary>
        public void PassClick()
        {
            passCnt += 1;
            turn.NextTurn();
            StartCoroutine(AI());
        }

        /// <summary>
        /// 처음시작
        /// </summary>
        public void StartAndChaneScreen()
        {
            CanvasTranform = Canvas.GetComponent<Transform>();
            
            start.enabled = false;
            Main.GetComponent<Camera>().enabled = true;
            Sub.GetComponent<Camera>().enabled = false;

            turn = gameObject.AddComponent<Turn>();
            InitPlayers();

            deck = deckPrefab.GetComponent<Deck>();
            deck.Initialise();
            deck.Populate();
            deck.Shuffle();

            StartPressed();
        }

        void Awake()
        {
            Main.GetComponent<Camera>().enabled = false;
            Sub.GetComponent<Camera>().enabled = true;
        }
 
        /// <summary>
        /// 프레임마다 업데이트
        /// </summary>
        void Update()
        {
            //누구 차례인지 버튼에서 글자를 바꿔줌
            var now = turn?.GetNowTurn();

            if(now != null)
            {
                now.CardDisable(true);
                pass.interactable = !now.isHouse; //버튼 활성화 비활성화
                pass.GetComponentInChildren<Text>().text = now.name; //글자 변경

                //현재 턴인 사람 뺴고 검정 반투명
                foreach (var tmp in totalPlayer)
                {
                    if (now != null)
                    {
                        if (tmp == now)
                        {
                            tmp.CardDisable(false);
                        }
                        else
                        {
                            tmp.CardDisable(true);
                        }
                    }
                }

                //내 카드중에 낼수없는데 반투명
                if (!now.isHouse)
                {
                    foreach (var tmp in now.cards)
                    {
                        if(nowStatus != null)
                        {
                            if (tmp.Rank <= nowStatus.Rank)
                            {
                                tmp.Disable(true);
                            }
                        }
                        
                    }
                }
            }

            //로딩일때 버튼
            if (isLoading)
            {
                pass.interactable = false;
                pass.GetComponentInChildren<Text>().text = "Loading...";
            }


            //Debug.Log(turn.TotalPlayer().ToString());
            if (Input.GetMouseButtonDown(0) && !isLoading)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hit2);
                var clickedCard = hit2.transform?.gameObject?.GetComponent<Card>();
                if (clickedCard != null)
                {
                    if (!clickedCard.owner.isHouse && turn.GetNowTurn() == clickedCard.owner)
                    {
                        TrashCard(clickedCard.owner, clickedCard);
                    }
                }
            }

        }

        private Card nowStatus = null;
        private int passCnt = 0;

        /// <summary>
        /// 모두 패스하여 이번 라운드 승리한 경우
        /// </summary>
        private void CheckPass()
        {
            //Debug.Log($"{passCnt + 1} {turn.TotalPlayer()}");
            if (passCnt + 1 >= turn.TotalPlayer())
            {
                nowStatus = null;
                passCnt = 0;
                foreach(var tmp in trash.cards.ToArray())
                {
                    trash.cards.Remove(tmp);
                    iTween.MoveTo(tmp.gameObject, new Vector2(5, 0), 1f);
                }
            }
        }

        /// <summary>
        /// 카드 제출
        /// </summary>
        /// <param name="p">제출하고자하는 플레이어</param>
        /// <param name="wantTrash">제출하고자하는 카드</param>
        private void TrashCard(Player p , Card wantTrash)
        {
            CheckPass();

            if (nowStatus == null || wantTrash.Rank > nowStatus.Rank)
            {
                var deck = p.cards;
                if (deck.Contains(wantTrash))
                {
                    passCnt = 0;
                    deck.Remove(wantTrash);
                    trash.Add(wantTrash);
                    nowStatus = wantTrash;
                    p.SortCard();
                    turn.NextTurn();
                    StartCoroutine(AI());
                }

                //이긴사람 list 추가
                if (p.cards.Count == 0)
                {
                    whoWin.Add(p);
                    p.text.GetComponentInChildren<Text>().text = $"{whoWin.Count} 등!!!";
                }
            }
            
        }

        /// <summary>
        /// AI 함수
        /// </summary>
        /// <returns></returns>
        private IEnumerator AI()
        {
            yield return new WaitForSeconds(1);

            CheckPass();

            var p = turn.GetNowTurn();
            if (p.cards.Count == 0)
            {
                //이 플레이어는 승리 했다.
                //가지고있는 카드의 개수가 0이다.
                turn.DelPlayer(p);
                StartCoroutine(AI()); //그러므로 다음 턴
            }
            else
            {
                if (p.isHouse)
                {
                    var available = p.PlayCard(nowStatus);
                    if (available != null)
                    {
                        TrashCard(turn.GetNowTurn(), available);
                    }
                    else
                    {
                        passCnt += 1;
                        turn.NextTurn();
                        StartCoroutine(AI());
                    }
                }
            }

        }

        /// <summary>
        /// 플레이어 초기화
        /// </summary>
        private void InitPlayers()
        {
            //text를 생성
            playerName0 = Instantiate(text, new Vector3(0, -2f, 0), Quaternion.identity);
            playerName0.transform.parent = CanvasTranform;
            playerName0.GetComponentInChildren<Text>().text = "player";
            //player를 생성
            player = playerPrefab.GetComponent<Player>();
            player.Initialise(0, -3.5f, false, false, "PASS", playerName0);
            turn.AddPlayer(player);
            totalPlayer.Add(player);


            //text를 생성
            playerName1 = Instantiate(text, new Vector3(-7F, 2f, 0), Quaternion.identity);
            playerName1.transform.parent = CanvasTranform;
            playerName1.GetComponentInChildren<Text>().text = "com1";
            //player를 생성
            GameObject playerClone1 = Instantiate(playerPrefab);
            house1 = playerClone1.GetComponent<Player>();
            house1.Initialise(-7f, 3.5f, true, false, "com1's turn", playerName1);
            turn.AddPlayer(house1);
            totalPlayer.Add(house1);


            //text를 생성
            playerName2 = Instantiate(text, new Vector3(0, 2f, 0), Quaternion.identity);
            playerName2.transform.parent = CanvasTranform;
            playerName2.GetComponentInChildren<Text>().text = "com2";
            //player를 생성
            GameObject playerClone2 = Instantiate(playerPrefab);
            house2 = playerClone2.GetComponent<Player>();
            house2.Initialise(0, 3.5f, true, false, "com2's turn", playerName2);
            turn.AddPlayer(house2);
            totalPlayer.Add(house2);


            //text를 생성
            playerName3 = Instantiate(text, new Vector3(7F, 2f, 0), Quaternion.identity);
            playerName3.transform.parent = CanvasTranform;
            playerName3.GetComponentInChildren<Text>().text = "com3";
            //player를 생성
            GameObject playerClone3 = Instantiate(playerPrefab);
            house3 = playerClone3.GetComponent<Player>();
            house3.Initialise(7f, 3.5f, true, false, "com3's turn", playerName3);
            turn.AddPlayer(house3);
            totalPlayer.Add(house3);


            //쓰레기통 생성
            GameObject trashClone = Instantiate(playerPrefab);
            trash = trashClone.GetComponent<Player>();
            trash.Initialise(0f, 0f, true, true, "trash's turn", null);
        }


        private void DealCards()
        {
            StartCoroutine(InitialDeal());
        }

        /// <summary>
        /// 최조의 카드 나눠줌
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitialDeal()
        {
            yield return new WaitForSeconds(1);
            int n = 10;
            while (n > 0)
            {
                deck.Deal(player);
                deck.Deal(house1);
                deck.Deal(house2);
                deck.Deal(house3);
                yield return new WaitForSecondsRealtime(0.15F);
                n--;
            }
            isLoading = false;
        }

        private IEnumerator GameOver(int status)
        {
            yield return new WaitForSeconds(.5f);
            string playerStatus = "";

            if (status == -1) playerStatus = "You Lose!";
            else if (status == -2) playerStatus = "You Win!";
            else if (status == -3) playerStatus = "Draw!";
        }


        //private IEnumerator HouseMove()
        //{
        //    while (house1.HouseHitting())
        //    {
        //        deck.Deal(house1);
        //        yield return new WaitForSeconds(1f);
        //    }
        //    int houseVal = house1.GetValue();
        //    int playerVal = player.GetValue();
        //    if (houseVal == -2 || (houseVal > playerVal)) StartCoroutine(GameOver(-1));// house gets blackjack or greater than player, the player loses 
        //    else if (houseVal == -1 || (houseVal < playerVal)) StartCoroutine(GameOver(-2)); //bust for less than player, player wins 
        //    else if (houseVal == playerVal) StartCoroutine(GameOver(-3)); ; //tie 
        //}

        //public void RestartPressed()
        //{
        //    deck.Clear();
        //    player.Clear();
        //    house1.Clear();

        //    deck.Populate();
        //    deck.Shuffle();
        //    currentPhase = dealPhase;
        //    DealCards();
        //}

        public void StartPressed()
        {
            DealCards();
        }

    }

}

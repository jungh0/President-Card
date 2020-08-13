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
        //ī�޶�
        public GameObject Main;
        public GameObject Sub;

        //ĵ���� ȭ��
        public GameObject Canvas;
        public Transform CanvasTranform;

        //�ؽ�Ʈ
        public GameObject text;

        //��ư
        public Button start;
        public Button pass;

        //�÷��̾�
        private Player player, house1, house2, house3;
        public readonly List<Player> totalPlayer = new List<Player>();
        public Player trash;

        //�÷��̾� �ؽ�Ʈ
        public GameObject playerName0, playerName1, playerName2, playerName3;

        //�ε����ΰ�
        public bool isLoading = true;
        //������ �����ΰ�
        public Turn turn;
        //1��������
        public readonly List<Player> whoWin = new List<Player>();


        public GameObject deckPrefab;
        public GameObject playerPrefab;
        public GameObject buttonPrefab;
        private Deck deck;

        /// <summary>
        /// �н���ư ��������
        /// </summary>
        public void PassClick()
        {
            passCnt += 1;
            turn.NextTurn();
            StartCoroutine(AI());
        }

        /// <summary>
        /// ó������
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
        /// �����Ӹ��� ������Ʈ
        /// </summary>
        void Update()
        {
            //���� �������� ��ư���� ���ڸ� �ٲ���
            var now = turn?.GetNowTurn();

            if(now != null)
            {
                now.CardDisable(true);
                pass.interactable = !now.isHouse; //��ư Ȱ��ȭ ��Ȱ��ȭ
                pass.GetComponentInChildren<Text>().text = now.name; //���� ����

                //���� ���� ��� ���� ���� ������
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

                //�� ī���߿� �������µ� ������
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

            //�ε��϶� ��ư
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
        /// ��� �н��Ͽ� �̹� ���� �¸��� ���
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
        /// ī�� ����
        /// </summary>
        /// <param name="p">�����ϰ����ϴ� �÷��̾�</param>
        /// <param name="wantTrash">�����ϰ����ϴ� ī��</param>
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

                //�̱��� list �߰�
                if (p.cards.Count == 0)
                {
                    whoWin.Add(p);
                    p.text.GetComponentInChildren<Text>().text = $"{whoWin.Count} ��!!!";
                }
            }
            
        }

        /// <summary>
        /// AI �Լ�
        /// </summary>
        /// <returns></returns>
        private IEnumerator AI()
        {
            yield return new WaitForSeconds(1);

            CheckPass();

            var p = turn.GetNowTurn();
            if (p.cards.Count == 0)
            {
                //�� �÷��̾�� �¸� �ߴ�.
                //�������ִ� ī���� ������ 0�̴�.
                turn.DelPlayer(p);
                StartCoroutine(AI()); //�׷��Ƿ� ���� ��
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
        /// �÷��̾� �ʱ�ȭ
        /// </summary>
        private void InitPlayers()
        {
            //text�� ����
            playerName0 = Instantiate(text, new Vector3(0, -2f, 0), Quaternion.identity);
            playerName0.transform.parent = CanvasTranform;
            playerName0.GetComponentInChildren<Text>().text = "player";
            //player�� ����
            player = playerPrefab.GetComponent<Player>();
            player.Initialise(0, -3.5f, false, false, "PASS", playerName0);
            turn.AddPlayer(player);
            totalPlayer.Add(player);


            //text�� ����
            playerName1 = Instantiate(text, new Vector3(-7F, 2f, 0), Quaternion.identity);
            playerName1.transform.parent = CanvasTranform;
            playerName1.GetComponentInChildren<Text>().text = "com1";
            //player�� ����
            GameObject playerClone1 = Instantiate(playerPrefab);
            house1 = playerClone1.GetComponent<Player>();
            house1.Initialise(-7f, 3.5f, true, false, "com1's turn", playerName1);
            turn.AddPlayer(house1);
            totalPlayer.Add(house1);


            //text�� ����
            playerName2 = Instantiate(text, new Vector3(0, 2f, 0), Quaternion.identity);
            playerName2.transform.parent = CanvasTranform;
            playerName2.GetComponentInChildren<Text>().text = "com2";
            //player�� ����
            GameObject playerClone2 = Instantiate(playerPrefab);
            house2 = playerClone2.GetComponent<Player>();
            house2.Initialise(0, 3.5f, true, false, "com2's turn", playerName2);
            turn.AddPlayer(house2);
            totalPlayer.Add(house2);


            //text�� ����
            playerName3 = Instantiate(text, new Vector3(7F, 2f, 0), Quaternion.identity);
            playerName3.transform.parent = CanvasTranform;
            playerName3.GetComponentInChildren<Text>().text = "com3";
            //player�� ����
            GameObject playerClone3 = Instantiate(playerPrefab);
            house3 = playerClone3.GetComponent<Player>();
            house3.Initialise(7f, 3.5f, true, false, "com3's turn", playerName3);
            turn.AddPlayer(house3);
            totalPlayer.Add(house3);


            //�������� ����
            GameObject trashClone = Instantiate(playerPrefab);
            trash = trashClone.GetComponent<Player>();
            trash.Initialise(0f, 0f, true, true, "trash's turn", null);
        }


        private void DealCards()
        {
            StartCoroutine(InitialDeal());
        }

        /// <summary>
        /// ������ ī�� ������
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

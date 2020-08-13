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
        public GameObject Main;
        public GameObject Sub;

        public Button start;
        public Button pass;

        public GameObject text;

        public GameObject deckPrefab;
        public GameObject playerPrefab;
        public GameObject buttonPrefab;

        public GameObject Canvas;
        public Transform CanvasTranform;

        private Deck deck;
        private Player player, house1, house2, house3;

        public Player trash;
        
        public bool isLoading = true;
        public Turn turn;

        public GameObject playingStr;
        public GameObject playerName1, playerName2, playerName3;

        public void PassClick()
        {
            passCnt += 1;
            turn.NextTurn();
            StartCoroutine(AI());
        }

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
 
        void Update()
        {
            var now = turn.GetNowTurn();
  


            pass.GetComponentInChildren<Text>().text = now.name;
            //Debug.Log(turn.TotalPlayer().ToString());
            if (Input.GetMouseButtonDown(0) && !isLoading)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Debug.Log("Pressed left click." + ray);

                Physics.Raycast(ray, out RaycastHit hit2);

                var clickedCard = hit2.transform.gameObject.GetComponent<Card>();
                if(clickedCard != null)
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

        private void CheckPass()
        {
            Debug.Log($"{passCnt + 1} {turn.TotalPlayer()}");
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
                    turn.NextTurn();
                    StartCoroutine(AI());

                }



                
            }
            
        }

        private IEnumerator AI()
        {
            yield return new WaitForSeconds(1);

            var p = turn.GetNowTurn();
            CheckPass();
            if (p.cards.Count == 0)
            {
                turn.DelPlayer(p);
                StartCoroutine(AI());
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


        private void InitPlayers()
        {
            player = playerPrefab.GetComponent<Player>();
            player.Initialise(0, -3.5f, false, false, "PASS");
            turn.AddPlayer(player);

            GameObject playerClone1 = Instantiate(playerPrefab);
            house1 = playerClone1.GetComponent<Player>();
            house1.Initialise(-7f, 3.5f, true, false, "com1's turn");
            turn.AddPlayer(house1);

            playerName1 = Instantiate(text, new Vector3(-7F, 2f, 0), Quaternion.identity);
            playerName1.transform.parent = CanvasTranform;
            playerName1.GetComponentInChildren<Text>().text = "com1";

            GameObject playerClone2 = Instantiate(playerPrefab);
            house2 = playerClone2.GetComponent<Player>();
            house2.Initialise(0, 3.5f, true, false, "com2's turn");
            turn.AddPlayer(house2);

            playerName2 = Instantiate(text, new Vector3(0, 2f, 0), Quaternion.identity);
            playerName2.transform.parent = CanvasTranform;
            playerName2.GetComponentInChildren<Text>().text = "com2";

            GameObject playerClone3 = Instantiate(playerPrefab);
            house3 = playerClone3.GetComponent<Player>();
            house3.Initialise(7f, 3.5f, true, false, "com3's turn");
            turn.AddPlayer(house3);

            playerName3 = Instantiate(text, new Vector3(7F, 2f, 0), Quaternion.identity);
            playerName3.transform.parent = CanvasTranform;
            playerName3.GetComponentInChildren<Text>().text = "com3";

            GameObject trashClone = Instantiate(playerPrefab);
            trash = trashClone.GetComponent<Player>();
            trash.Initialise(0f, 0f, true, true, "trash's turn");

        }

        private void DealCards()
        {
            StartCoroutine(InitialDeal());
        }

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

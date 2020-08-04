using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameControl : MonoBehaviour
    {
        public GameObject Main;
        public GameObject Sub;

        const int dealPhase = 0, playerPhase = 1, housePhase = 2, endPhase = 3, canvasScale = 50, padding = 30;

        public GameObject deckPrefab;
        public GameObject playerPrefab;
        public GameObject buttonPrefab;

        public GameObject Canvas;

        private Deck deck;
        private Player player, house1, house2, house3;
        
        private int currentPhase;


        public void StartAndChaneScreen()
        {
            Main.GetComponent<Camera>().enabled = true;
            Sub.GetComponent<Camera>().enabled = false;

            currentPhase = dealPhase;

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
            return;
            
        }

        private void InitPlayers()
        {

            player = playerPrefab.GetComponent<Player>();
            player.Initialise(0, -3.5f, false);

            GameObject playerClone1 = Instantiate(playerPrefab);
            house1 = playerClone1.GetComponent<Player>();
            house1.Initialise(7f, 3.5f, true);

            GameObject playerClone2 = Instantiate(playerPrefab);
            house2 = playerClone2.GetComponent<Player>();
            house2.Initialise(0, 3.5f, true);

            GameObject playerClone3 = Instantiate(playerPrefab);
            house3 = playerClone3.GetComponent<Player>();
            house3.Initialise(-7f, 3.5f, true);
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
            currentPhase = playerPhase;

        }

        private IEnumerator GameOver(int status)
        {
            yield return new WaitForSeconds(.5f);
            currentPhase = endPhase;
            string playerStatus = "";

            if (status == -1) playerStatus = "You Lose!";
            else if (status == -2) playerStatus = "You Win!";
            else if (status == -3) playerStatus = "Draw!";


        }

        public void HitPressed()
        {
            deck.Deal(player);
        }

        public void StandPressed()
        {
            currentPhase = housePhase;
            StartCoroutine(HouseMove());
        }

        private IEnumerator HouseMove()
        {
            while (house1.HouseHitting())
            {
                deck.Deal(house1);
                yield return new WaitForSeconds(1f);
            }
            int houseVal = house1.GetValue();
            int playerVal = player.GetValue();
            if (houseVal == -2 || (houseVal > playerVal)) StartCoroutine(GameOver(-1));// house gets blackjack or greater than player, the player loses 
            else if (houseVal == -1 || (houseVal < playerVal)) StartCoroutine(GameOver(-2)); //bust for less than player, player wins 
            else if (houseVal == playerVal) StartCoroutine(GameOver(-3)); ; //tie 
        }

        public void RestartPressed()
        {
            deck.Clear();
            player.Clear();
            house1.Clear();

            deck.Populate();
            deck.Shuffle();
            currentPhase = dealPhase;
            DealCards();
        }

        public void StartPressed()
        {
            DealCards();
        }

    }

}

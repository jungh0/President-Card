using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameControl : MonoBehaviour
    {
        //ī�޶�
        public GameObject Main, Sub;

        //ĵ���� ȭ��
        public GameObject Canvas;
        public Transform CanvasTranform;

        //�ؽ�Ʈ
        public GameObject text;

        //��ư
        public Button start, pass, cancel, submit;

        //�ε����ΰ�
        public bool isLoading = true;

        //���Ӿ˰���
        public GameAlgorithm turn;
        public GameObject deckPrefab, playerPrefab;
        private Deck deck;

        /// <summary>
        /// �н���ư ��������
        /// </summary>
        public void PassClick()
        {
            turn?.PassClick();
        }

        public void CancelClick()
        {
            turn?.CancelClick();
        }

        public void SubmitClick()
        {
            turn?.SubmitClick();
        }

        /// <summary>
        /// ó������
        /// </summary>
        public void StartAndChaneScreen()
        {
            CanvasTranform = Canvas.GetComponent<Transform>();
            Main.GetComponent<Camera>().enabled = true;
            Sub.GetComponent<Camera>().enabled = false;
            start.enabled = false;

            RealStart();
        }

        public void RealStart()
        {
            turn = gameObject.AddComponent<GameAlgorithm>();
            isLoading = true;
            InitPlayers();
            deck = deckPrefab.GetComponent<Deck>();
            deck.Initialise();
            deck.Populate();
            deck.Shuffle();
            StartCoroutine(InitialDeal());
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
            if (turn?.IsGameDone() ?? false)
            {
                RealStart();
            }

            //���� �������� ��ư���� ���ڹٲ��ְ� ��Ȱ��ȭ ����
            if (turn?.GetNowTurn() is Player now)
            {
                //��ư Ȱ��ȭ ��Ȱ��ȭ
                pass.interactable = !now.isHouse;
                cancel.interactable = !now.isHouse;
                submit.interactable = turn.IsCanSubmit();
                pass.GetComponentInChildren<Text>().text = now.name; //���� ����

                //�� �� ���� ī�� ���� �� ���� �ƴϸ� ����
                turn?.MakeBlackCard(now);
            }

            //�ε��϶� ��ư
            if (isLoading)
            {
                pass.interactable = false;
                cancel.interactable = false;
                pass.GetComponentInChildren<Text>().text = "Loading...";
            }

            //�÷��̾ Ŭ���Ѱ� ����
            if (Input.GetMouseButtonDown(0) && !isLoading)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hit2);
                if (hit2.transform?.gameObject?.GetComponent<Card>() is Card clickedCard)
                {
                    if (!clickedCard.owner.isHouse && turn.GetNowTurn() == clickedCard.owner)
                    {
                        turn?.SelectCard(clickedCard.owner, clickedCard);
                    }
                }
            }
        }


        /// <summary>
        /// �÷��̾� �ʱ�ȭ
        /// </summary>
        private void InitPlayers()
        {
            MakePlayer(x: 0, y: -3.5f, isHouse: false, playerName: "player", buttonName: "PASS");
            MakePlayer(x: -7f, y: 3.5f, isHouse: true, playerName: "com1", buttonName: "com1's turn");
            MakePlayer(x: 0, y: 3.5f, isHouse: true, playerName: "com2", buttonName: "com2's turn");
            MakePlayer(x: 7f, y: 3.5f, isHouse: true, playerName: "com3", buttonName: "com3's turn");
            MakePlayer(x: 0, y: 0, isHouse: true, playerName: "trash", buttonName: "trash's turn", isTrash: true);
        }


        /// <summary>
        /// �÷��̾� �ʱ�ȭ �ܼ�ȭ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isHouse"></param>
        /// <param name="playerName"></param>
        /// <param name="buttonName"></param>
        /// <param name="isTrash"></param>
        private void MakePlayer(float x, float y, bool isHouse, string playerName, string buttonName, bool isTrash = false)
        {
            //�÷��̾�� �̸����� ��� ���� �ϴ��̶� y��ǥ ��������
            float locationY = 1.5f;
            if (isHouse)
                locationY = -1.5f;

            //text�� ����
            GameObject playerText = null;
            if (!isTrash)
            {
                playerText = Instantiate(text, new Vector3(x, y + locationY, 0), Quaternion.identity);
                playerText.transform.parent = CanvasTranform;
                playerText.GetComponentInChildren<Text>().text = playerName;
            }

            //player�� ����
            GameObject playerClone = Instantiate(playerPrefab);
            Player player = playerClone.GetComponent<Player>();
            player.Initialise(x, y, isHouse, isTrash, buttonName, playerText);

            if (!isTrash)
            {
                turn.AddPlayer(player);
            }
            else
            {
                turn.trash = player;
            }
        }

        /// <summary>
        /// ������ ī�� ������
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitialDeal()
        {
            yield return new WaitForSeconds(1);
            //SoundManager.instance.PlaySound("soundShuffle");
            int n = 10;
            while (n > 0)
            {
                foreach (var tmp in turn.GetTotalPlayer())
                {
                    deck.Deal(tmp);
                }
                yield return new WaitForSecondsRealtime(0.2F);
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


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameControl : MonoBehaviour
    {
        /// <summary>
        /// pulbic
        /// </summary>
        public Canvas canvas_1, canvas_2, canvas_3, canvas_4;
        //�ؽ�Ʈ
        public GameObject text;

        public Text player1_cnt, player2_cnt, player3_cnt, status;

        public Text win1, win2, win3, win4;

        //��ư
        public Lean.Gui.LeanButton pass, submit;

        public GameObject deckPrefab, playerPrefab;

        public bool _8clear = true;

        /// <summary>
        /// private
        /// </summary>

        //���Ӿ˰���
        private GameAlgorithm turn;
        private Deck deck;

        //ĵ���� ȭ��
        private GameObject Canvas;
        private Transform CanvasTranform;

        //�ε����ΰ�
        private bool isLoading = true;
        private bool isStart = false;

        public void _8clearT()
        {
            _8clear = !_8clear;
        }

        public void SubmitClick()
        {
            if (submit.IsInteractable())
            {
                turn?.SubmitClick();
            }
        }
        public void PassClick()
        {
            if (pass.IsInteractable())
            {
                turn?.PassClick();
            }
        }

        public void RealEnd()
        {
            isLoading = false;
            isStart = false;
            turn?.EndGame();
            ReadyStart();
        }

        public void RealEndNormal(List<string> rankList)
        {
            isLoading = false;
            isStart = false;
            turn?.EndGame();
            GameResult(rankList);
        }

        public void RealStart()
        {
            isStart = true;
            canvas_1.gameObject.SetActive(true);
            canvas_2.gameObject.SetActive(false);
            canvas_4.gameObject.SetActive(false);

            turn = gameObject.AddComponent<GameAlgorithm>();
            turn.Initialise(status, Allow8Clear: !_8clear);
            isLoading = true;
            InitPlayers();
            deck = deckPrefab.GetComponent<Deck>();
            deck.Initialise();
            deck.Populate();
            deck.Shuffle();
            StartCoroutine(InitialDeal());
        }

        public void ReadyStart()
        {
            CloseMenu();
            canvas_1.gameObject.SetActive(false);
            canvas_2.gameObject.SetActive(true);
            canvas_4.gameObject.SetActive(false);
        }

        public void GameResult(List<string> rankList)
        {
            CloseMenu();

            win1.text = $"1. {rankList[0]} (WIN!! )";
            win2.text = $"2. {rankList[1]}";
            win3.text = $"3. {rankList[2]}";
            win4.text = $"4. {rankList[3]}";

            canvas_1.gameObject.SetActive(false);
            canvas_2.gameObject.SetActive(false);
            canvas_4.gameObject.SetActive(true);
        }

        public void RealKill()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void OpenMenu()
        {
            if (!isLoading)
            {
                canvas_3.gameObject.SetActive(true);
            }

        }

        public void CloseMenu()
        {
            canvas_3.gameObject.SetActive(false);
        }

        void Awake()
        {

        }

        void Start()
        {
            ReadyStart();
        }

        /// <summary>
        /// �����Ӹ��� ������Ʈ
        /// </summary>
        void Update()
        {
            if (!isStart)
            {
                return;
            }

            if (turn?.IsGameDone() ?? false)
            {
                var rankList = turn?.IsGameDoneList();
                RealEndNormal(rankList);
            }

            //���� �������� ��ư���� ���ڹٲ��ְ� ��Ȱ��ȭ ����
            if (turn?.GetNowTurn() is Player now)
            {
                if (turn?.isSelected() ?? false)
                {
                    pass.GetComponentInChildren<Text>().text = "CANCEL";
                }
                else
                {
                    pass.GetComponentInChildren<Text>().text = "PASS";
                }

                submit.enabled = turn?.SelectedCardCanSubmit(now) ?? false; //��ư Ȱ��ȭ ��Ȱ��ȭ
                pass.enabled = !now.isHouse; //pass ��ư Ȱ��ȭ ��Ȱ��ȭ
                //cancel.enabled = turn?.CanCancel(now) ?? false;

                turn?.MakeBlackCard(now);
                turn?.CheckPass();

                var remainCnt = turn?.GetPlayerCardsCnt();
                player1_cnt.text = $"Player1 ( {remainCnt[1]} )";
                player2_cnt.text = $"Player2 ( {remainCnt[2]} )";
                player3_cnt.text = $"Player3 ( {remainCnt[3]} )";

                
            }

            //�ε��϶� ��ư
            if (isLoading)
            {
                pass.enabled = false; //pass ��ư ��Ȱ��ȭ
                submit.enabled = false; //submit ��ư ��Ȱ��ȭ
                status.text = $"Loading...";
                //cancel.enabled = false; //cancel ��ư ��Ȱ��ȭ
                //pass.GetComponentInChildren<Text>().text = "Loading...";
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
            MakePlayer(x: 0, y: -3.5f, isHouse: false, playerName: "player", buttonName: "Your turn");
            MakePlayer(x: -7f, y: 6.8f, isHouse: true, playerName: "com1", buttonName: "Player1's turn");
            MakePlayer(x: 0, y: 6.8f, isHouse: true, playerName: "com2", buttonName: "Player2's turn");
            MakePlayer(x: 7f, y: 6.8f, isHouse: true, playerName: "com3", buttonName: "Player3's turn");
            MakePlayer(x: 0, y: 0, isHouse: true, playerName: "trash", buttonName: "Trash's turn", isTrash: true);
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

            //player�� ����
            GameObject playerClone = Instantiate(playerPrefab);
            Player player = playerClone.GetComponent<Player>();
            player.Initialise(x, y, isHouse, isTrash, buttonName);

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
            int n = 13;
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



    }

}

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
        //텍스트
        public GameObject text;

        public Text player1_cnt, player2_cnt, player3_cnt, status;

        public Text win1, win2, win3, win4;

        //버튼
        public Lean.Gui.LeanButton pass, submit;

        public GameObject deckPrefab, playerPrefab;

        public bool _8clear = true;

        /// <summary>
        /// private
        /// </summary>

        //게임알고리즘
        private GameAlgorithm turn;
        private Deck deck;

        //캔버스 화면
        private GameObject Canvas;
        private Transform CanvasTranform;

        //로딩중인가
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
        /// 프레임마다 업데이트
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

            //누구 차례인지 버튼에서 글자바꿔주고 비활성화 관리
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

                submit.enabled = turn?.SelectedCardCanSubmit(now) ?? false; //버튼 활성화 비활성화
                pass.enabled = !now.isHouse; //pass 버튼 활성화 비활성화
                //cancel.enabled = turn?.CanCancel(now) ?? false;

                turn?.MakeBlackCard(now);
                turn?.CheckPass();

                var remainCnt = turn?.GetPlayerCardsCnt();
                player1_cnt.text = $"Player1 ( {remainCnt[1]} )";
                player2_cnt.text = $"Player2 ( {remainCnt[2]} )";
                player3_cnt.text = $"Player3 ( {remainCnt[3]} )";

                
            }

            //로딩일때 버튼
            if (isLoading)
            {
                pass.enabled = false; //pass 버튼 비활성화
                submit.enabled = false; //submit 버튼 비활성화
                status.text = $"Loading...";
                //cancel.enabled = false; //cancel 버튼 비활성화
                //pass.GetComponentInChildren<Text>().text = "Loading...";
            }

            //플레이어가 클릭한거 선택
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
        /// 플레이어 초기화
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
        /// 플레이어 초기화 단순화
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isHouse"></param>
        /// <param name="playerName"></param>
        /// <param name="buttonName"></param>
        /// <param name="isTrash"></param>
        private void MakePlayer(float x, float y, bool isHouse, string playerName, string buttonName, bool isTrash = false)
        {
            //플레이어는 이름라벨이 상단 상대는 하단이라 y좌표 구분해줌
            float locationY = 1.5f;
            if (isHouse)
                locationY = -1.5f;

            //player를 생성
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
        /// 최초의 카드 나눠줌
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

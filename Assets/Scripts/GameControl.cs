using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameControl : MonoBehaviour
    {
        //카메라
        public GameObject Main, Sub;

        //캔버스 화면
        public GameObject Canvas;
        public Transform CanvasTranform;

        //텍스트
        public GameObject text;

        //버튼
        public Button start, pass, cancel, submit;

        //로딩중인가
        public bool isLoading = true;

        //게임알고리즘
        public GameAlgorithm turn;
        public GameObject deckPrefab, playerPrefab;
        private Deck deck;

        /// <summary>
        /// 패스버튼 눌렀을때
        /// </summary>
        public void PassClick()
        {
            if (pass.IsInteractable())
            {
                turn?.PassClick();
            }
            
        }

        public void CancelClick()
        {
            if (cancel.IsInteractable())
            {
                turn?.CancelClick();
            }
        }

        public void SubmitClick()
        {
            if (submit.IsInteractable())
            {
                turn?.SubmitClick();
            }
        }

        /// <summary>
        /// 처음시작
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
        /// 프레임마다 업데이트
        /// </summary>
        void Update()
        {
            if (turn?.IsGameDone() ?? false)
            {
                RealStart();
            }

            //누구 차례인지 버튼에서 글자바꿔주고 비활성화 관리
            if (turn?.GetNowTurn() is Player now)
            {
                submit.interactable = turn?.SelectedCardCanSubmit(now) ?? false; //버튼 활성화 비활성화
                pass.interactable = !now.isHouse; //pass 버튼 활성화 비활성화
                cancel.interactable = turn?.CanCancel(now) ?? false;

                turn?.MakeBlackCard(now);
                turn?.CheckPass();
            }

            //로딩일때 버튼
            if (isLoading)
            {
                pass.interactable = false; //pass 버튼 비활성화
                submit.interactable = false; //submit 버튼 비활성화
                cancel.interactable = false; //cancel 버튼 비활성화
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
            MakePlayer(x: 0, y: -3.5f, isHouse: false, playerName: "player", buttonName: "PASS");
            MakePlayer(x: -7f, y: 3.5f, isHouse: true, playerName: "com1", buttonName: "com1's turn");
            MakePlayer(x: 0, y: 3.5f, isHouse: true, playerName: "com2", buttonName: "com2's turn");
            MakePlayer(x: 7f, y: 3.5f, isHouse: true, playerName: "com3", buttonName: "com3's turn");
            MakePlayer(x: 0, y: 0, isHouse: true, playerName: "trash", buttonName: "trash's turn", isTrash: true);
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

            //text를 생성
            GameObject playerText = null;
            if (!isTrash)
            {
                playerText = Instantiate(text, new Vector3(x, y + locationY, 0), Quaternion.identity);
                playerText.transform.parent = CanvasTranform;
                playerText.GetComponentInChildren<Text>().text = playerName;
            }

            //player를 생성
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
        /// 최초의 카드 나눠줌
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitialDeal()
        {
            yield return new WaitForSeconds(1);
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



    }

}

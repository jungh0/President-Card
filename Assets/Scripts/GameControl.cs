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
                //버튼 활성화 비활성화
                pass.interactable = !now.isHouse;
                cancel.interactable = !now.isHouse;
                submit.interactable = turn.IsCanSubmit();
                pass.GetComponentInChildren<Text>().text = now.name; //글자 변경

                //낼 수 없는 카드 검정 및 차례 아니면 검정
                turn?.MakeBlackCard(now);
            }

            //로딩일때 버튼
            if (isLoading)
            {
                pass.interactable = false;
                cancel.interactable = false;
                pass.GetComponentInChildren<Text>().text = "Loading...";
            }

            //플레이어가 클릭한거 제출
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

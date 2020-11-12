using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameControl : MonoBehaviour
    {
        /// <summary>
        /// pulbic
        /// </summary>

        //�ؽ�Ʈ
        public GameObject text;

        public Text player1_cnt, player2_cnt, player3_cnt, status;

        //��ư
        public Lean.Gui.LeanButton pass, submit;

        public GameObject deckPrefab, playerPrefab;

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

        public void PassClick()
        {
            if (pass.IsInteractable())
            {
                turn?.PassClick();
            }
            
        }

/*        public void CancelClick()
        {
            if (cancel.IsInteractable())
            {
                turn?.CancelClick();
            }
        }*/

        public void SubmitClick()
        {
            if (submit.IsInteractable())
            {
                turn?.SubmitClick();
            }
        }

        /// <summary>
        /// ó������
        /// </summary>
        public void StartAndChaneScreen()
        {
            CanvasTranform = Canvas.GetComponent<Transform>();
            pass.gameObject.SetActive(true);
            submit.gameObject.SetActive(true);

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
            //RealStart();
        }

        void Start()
        {
            RealStart();
        }

        /// <summary>
        /// �����Ӹ��� ������Ʈ
        /// </summary>
        void Update()
        {
            if (turn?.IsGameDone() ?? false)
            {
                turn?.EndGame(deck);
                RealStart();
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

                status.text = $"{now.name}";
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

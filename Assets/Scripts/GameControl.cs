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
                submit.interactable = turn?.SelectedCardCanSubmit(now) ?? false; //��ư Ȱ��ȭ ��Ȱ��ȭ
                pass.interactable = !now.isHouse; //pass ��ư Ȱ��ȭ ��Ȱ��ȭ
                cancel.interactable = turn?.CanCancel(now) ?? false;

                turn?.MakeBlackCard(now);
                turn?.CheckPass();
            }

            //�ε��϶� ��ư
            if (isLoading)
            {
                pass.interactable = false; //pass ��ư ��Ȱ��ȭ
                submit.interactable = false; //submit ��ư ��Ȱ��ȭ
                cancel.interactable = false; //cancel ��ư ��Ȱ��ȭ
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

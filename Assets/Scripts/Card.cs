using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Card : MonoBehaviour
    {

        public enum Suits
        {
            Diamond, Club, Heart, Spade
        }

        public enum Ranks
        {
            Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace, Two
        }


        public Suits Suit { get; set; }
        public Ranks Rank { get; set; }
        public Sprite back;
        private Sprite face;
        private SpriteRenderer _spriteRenderer;
        public bool isFaceUp;

        public void Initialise(Suits s, Ranks r, Vector2 pos, Quaternion rot, Sprite cardFace)
        {
            Suit = s;
            Rank = r;
            transform.position = pos;
            transform.rotation = rot;
            face = cardFace;
            isFaceUp = false;
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            BoxCollider myBC = (BoxCollider)gameObject.AddComponent(typeof(BoxCollider));
            //gameObject.GetComponent<BoxCollider>().enabled = true;
            //myBC.isTrigger = true;
        }

        public void DestroyCard()
        {
            Destroy(gameObject);
        }

        public void Flip()
        {
            if (isFaceUp)
            {
                _spriteRenderer.sprite = back;
                isFaceUp = false;
            }
            else
            {
                _spriteRenderer.sprite = face;
                isFaceUp = true;
            }
        }

        public void Show()
        {
            _spriteRenderer.sprite = face;
            isFaceUp = true;
        }

        public void Hide()
        {
            _spriteRenderer.sprite = back;
            isFaceUp = false;
        }

    
        //void FixedUpdate()
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        CastRay();
        //        if (target == this.gameObject)
        //        {  //Ÿ�� ������Ʈ�� ��ũ��Ʈ�� ���� ������Ʈ���
        //            Debug.Log("touch obj name : " + target.name);
        //        }
        //    }
        //}

        //void CastRay() // ���� ��Ʈó�� �κ�.  ���̸� ���� ó���մϴ�. 

        //{
        //    target = null;

        //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        //    if (hit.collider != null)
        //    { //��Ʈ�Ǿ��ٸ� ���⼭ ����
        //        //Debug.Log (hit.collider.name);  //�� �κ��� Ȱ��ȭ �ϸ�, ���õ� ������Ʈ�� �̸��� ���� ���ɴϴ�. 
        //        target = hit.collider.gameObject;  //��Ʈ �� ���� ������Ʈ�� Ÿ������ ����
        //    }
        //}

        //private GameObject target;
        private static int order = 0;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.Log("Pressed left click." + ray);

                RaycastHit hit2;
                Physics.Raycast(ray, out hit2);

                Debug.Log(hit2.transform.gameObject);
                hit2.transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder = order++;

                iTween.MoveTo(hit2.transform.gameObject, new Vector2(0, 0), 1f);
                //hit2.transform.gameObject.transform.Translate(0, 0.01f, 0);
                //iTween.MoveTo(hit2, new Vector2(0,0), 1f);
                //hit2.transform.gameObject.transform.localPosition = new Vector3(0, 0, 0); // ī�� ����

            }


        }

    }
}

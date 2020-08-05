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
        //        {  //타겟 오브젝트가 스크립트가 붙은 오브젝트라면
        //            Debug.Log("touch obj name : " + target.name);
        //        }
        //    }
        //}

        //void CastRay() // 유닛 히트처리 부분.  레이를 쏴서 처리합니다. 

        //{
        //    target = null;

        //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        //    if (hit.collider != null)
        //    { //히트되었다면 여기서 실행
        //        //Debug.Log (hit.collider.name);  //이 부분을 활성화 하면, 선택된 오브젝트의 이름이 찍혀 나옵니다. 
        //        target = hit.collider.gameObject;  //히트 된 게임 오브젝트를 타겟으로 지정
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
                //hit2.transform.gameObject.transform.localPosition = new Vector3(0, 0, 0); // 카드 내기

            }


        }

    }
}

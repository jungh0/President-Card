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
        public Player owner;
        public bool isDisabled = false;

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

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="status"></param>
        public void Disable(bool status)
        {
            isDisabled = status;
            if (status)
            {
                _spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            else
            {
                _spriteRenderer.color = new Color(1, 1, 1, 1f);
                //_spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, f);
            }
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


    }
}

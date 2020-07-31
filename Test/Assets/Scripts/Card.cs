using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Card : MonoBehaviour
    {

        public enum Suits
        {
            Diamond,Club, Heart, Spade
        }

        public enum Ranks
        {
            Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
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

    }

}

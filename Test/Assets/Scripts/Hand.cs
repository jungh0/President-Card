using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Hand : MonoBehaviour
    {

        public GameObject cardPrefab;
        public Sprite[] faces;
        protected List<Card> cards;

        public void Initialise()
        {
            cards = new List<Card>();
        }

        public virtual void Add(Card c)
        {
            cards.Add(c);
        }

        public void Clear()
        {
            foreach (Card c in cards)
            {
                c.DestroyCard();
            }
            cards.Clear();
        }

        public void FlipFaceUp()
        {
            foreach(Card c in cards)
            {
                if (!c.isFaceUp) c.Flip();
            }
        }

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Hand : MonoBehaviour
    {

        public GameObject cardPrefab;
        public Sprite[] faces;
        public List<Card> cards;

        public void Initialise()
        {
            cards = new List<Card>();
        }

        public void Sort()
        {
            cards.Sort((a, b) => {
                if ((int)a.Rank > (int)b.Rank)
                {
                    return 1;
                }
                else if ((int)a.Rank < (int)b.Rank)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }

            }
           );
        }

        public virtual void Add(Card c, Player owner)
        {
            c.owner = owner;
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


using UnityEngine;
using Game;
using System.Collections;
using System.Collections.Generic;

namespace Game
{

    public class Player : Hand
    {
        public float xPos, yPos;
        public bool isHouse;
        public bool isTrash;
        public string name;
        public GameObject text;
        private int trashOrder = 1;

        public void Initialise(float x, float y, bool houseOrPlayer, bool isTrash, string name, GameObject text)
        {
            base.Initialise();
            xPos = x;
            yPos = y;
            isHouse = houseOrPlayer;
            this.isTrash = isTrash;
            this.name = name;
            this.text = text;
        }

        public Card PlayCard(Card c)
        {
            foreach (var tmp in this.cards)
            {
                if (c == null || tmp.Rank > c.Rank)
                {
                    return tmp;
                }
            }
            return null;
        }

        public void CardTrash(List<Card> cards)
        {
            int size = cards.Count;
            for (int i = 0; i < size; i++)
            {
                float cardDistance = 1.2f;
                cards[i].GetComponent<SpriteRenderer>().sortingOrder = trashOrder++;

                iTween.MoveTo(cards[i].gameObject, new Vector2(cardDistance * (i - (size / 2)) + 0, 0.15f), 1f);
                cards[i].Show();
            }
        }

        public void CardDisable(bool status)
        {
            foreach (var tmp in this.cards)
            {
                tmp.Disable(status);
            }
        }

        public void SortCard()
        {
            if (!isTrash)
            {
                Sort();
            }

            int size = cards.Count;
            for (int i = 0; i < size; i++)
            {
                float cardDistance = 1f;
                if (isHouse && !isTrash)
                {
                    cardDistance = 0.3f;
                }

                float goLeft = 1.5f;
                if (isHouse && !isTrash)
                {
                    goLeft = 0;
                }

                cards[i].GetComponent<SpriteRenderer>().sortingOrder = i;

                if (!isTrash)
                {
                    if (size / 2 <= i - 1)
                    {
                        iTween.MoveTo(cards[i].gameObject, new Vector2(cardDistance * (i - (size / 2)) + xPos- goLeft, yPos), 1f);
                    }
                    else
                    {
                        iTween.MoveTo(cards[i].gameObject, new Vector2(-1 * cardDistance * ((size / 2) - i) + xPos-goLeft, yPos), 1f);
                    }
                }
                else
                {
                    iTween.MoveTo(cards[i].gameObject, new Vector2(0, 0), 1f);
                }


                if (isHouse && !isTrash)
                {
                    cards[i].Hide();
                }
                else
                {
                    cards[i].Show();
                }
                //if(!cards[i].isFaceUp && (!isHouse || i != 1)) ;
            }
        }

        public void Add(Card c)
        {
            base.Add(c, this);

            if (!isTrash)
            {
                SortCard();
            }
            
        }

    }
}

